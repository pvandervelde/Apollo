﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Properties;
using Apollo.Utilities;
using Castle.DynamicProxy;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines an <see cref="IInterceptor"/> for <see cref="ICommandSet"/> methods that do not return a value.
    /// </summary>
    internal sealed class CommandSetMethodWithoutResultInterceptor : IInterceptor
    {
        /// <summary>
        /// Returns a task with a specific return type based on an expected <see cref="CommandInvokedResponseMessage"/> object
        /// which is delivered by another task.
        /// </summary>
        /// <param name="inputTask">The task which will deliver the <see cref="ICommunicationMessage"/> that contains the return value.</param>
        /// <param name="scheduler">The scheduler that is used to run the task.</param>
        /// <returns>
        /// A task returning the desired return type.
        /// </returns>
        private static Task CreateTask(Task<ICommunicationMessage> inputTask, TaskScheduler scheduler)
        {
            Action action = () =>
            {
                try
                {
                    inputTask.Wait();
                }
                catch (AggregateException e)
                {
                    throw new CommandInvocationFailedException(
                        Resources.Exceptions_Messages_CommandInvocationFailed,
                        e);
                }

                var successMsg = inputTask.Result as SuccessMessage;
                if (successMsg != null)
                {
                    return;
                }

                // var failureMsg = inputTask.Result as FailureMessage;
                throw new CommandInvocationFailedException();
            };

            return Task.Factory.StartNew(
                action, 
                new CancellationToken(),
                TaskCreationOptions.LongRunning,
                scheduler);
        }

        private static string MethodToText(MethodInfo method)
        {
            return method.ToString();
        }

        /// <summary>
        /// The function which sends the <see cref="CommandInvokedMessage"/> to the owning endpoint.
        /// </summary>
        private readonly Func<ISerializedMethodInvocation, Task<ICommunicationMessage>> m_SendMessageWithResponse;

        /// <summary>
        /// The function used to write log messages.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSetMethodWithoutResultInterceptor"/> class.
        /// </summary>
        /// <param name="sendMessageWithResponse">
        ///     The function used to send the information about the method invocation to the owning endpoint.
        /// </param>
        /// <param name="logger">The function that is used to log messages.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessageWithResponse"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public CommandSetMethodWithoutResultInterceptor(
            Func<ISerializedMethodInvocation, Task<ICommunicationMessage>> sendMessageWithResponse,
            Action<LogSeverityProxy, string> logger,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => sendMessageWithResponse);
                Enforce.Argument(() => logger);
            }

            m_SendMessageWithResponse = sendMessageWithResponse;
            m_Logger = logger;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
        }

        /// <summary>
        /// Called when a method or property call is intercepted.
        /// </summary>
        /// <param name="invocation">Information about the call that was intercepted.</param>
        public void Intercept(IInvocation invocation)
        {
            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Invoking {0}",
                    MethodToText(invocation.Method)));

            Task<ICommunicationMessage> result = null;
            try
            {
                result = m_SendMessageWithResponse(
                    ProxyExtensions.FromMethodInfo(
                        invocation.Method,
                        invocation.Arguments));
            }
            catch (EndpointNotContactableException e)
            {
                m_Logger(
                LogSeverityProxy.Error,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Tried to invoke {0}, but failed to contact the remote endpoint.",
                    MethodToText(invocation.Method)));

                throw new CommandInvocationFailedException(Resources.Exceptions_Messages_CommandInvocationFailed, e);
            }
            catch (FailedToSendMessageException e)
            {
                m_Logger(
                LogSeverityProxy.Error,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Tried to invoke {0}, but failed to send the message.",
                    MethodToText(invocation.Method)));

                throw new CommandInvocationFailedException(Resources.Exceptions_Messages_CommandInvocationFailed, e);
            }

            invocation.ReturnValue = CreateTask(result, m_Scheduler);
        }
    }
}