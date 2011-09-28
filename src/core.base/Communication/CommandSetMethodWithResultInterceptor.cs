﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    /// Defines an <see cref="IInterceptor"/> for <see cref="ICommandSet"/> methods that return a value.
    /// </summary>
    internal sealed class CommandSetMethodWithResultInterceptor : IInterceptor
    {
        /// <summary>
        /// Returns a task with a specific return type based on an expected <see cref="CommandInvokedResponseMessage"/> object
        /// which is delivered by another task.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the object carried by the <see cref="CommandInvokedResponseMessage"/> object in the input task.
        /// </typeparam>
        /// <param name="inputTask">
        ///     The task which will deliver the <see cref="ICommunicationMessage"/> that contains the return value.
        /// </param>
        /// <param name="scheduler">The scheduler that runs the return task.</param>
        /// <returns>
        /// A task returning the desired return type.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method is called via reflection in order to generate the correct return value for a command method.")]
        private static Task<T> CreateTask<T>(Task<ICommunicationMessage> inputTask, TaskScheduler scheduler)
        {
            Func<T> action = () =>
            {
                inputTask.Wait();

                var resultMsg = inputTask.Result as CommandInvokedResponseMessage;
                if (resultMsg != null)
                {
                    return (T)resultMsg.Result;
                }

                var successMsg = inputTask.Result as SuccessMessage;
                if (successMsg != null)
                {
                    return default(T);
                }

                throw new CommandInvocationFailedException();
            };

            return Task<T>.Factory.StartNew(
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
        /// Initializes a new instance of the <see cref="CommandSetMethodWithResultInterceptor"/> class.
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
        public CommandSetMethodWithResultInterceptor(
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

            // Now that we have the result we need to create the return value of the correct type
            // The only catch is that we don't know (at compile time) what the return value must
            // be so we'll have to do this the nasty way
            //
            // First get the return value for the proxied method
            // This should be Task<T> for some value of T
            Type invocationReturn = invocation.Method.ReturnType;
            Debug.Assert(!invocationReturn.ContainsGenericParameters, "The return type should be a closed constructed type.");

            var genericArguments = invocationReturn.GetGenericArguments();
            Debug.Assert(genericArguments.Length == 1, "There should be exactly one generic argument.");

            // Now 'build' a method that can create the Task<T> object. We'll have to do this
            // through reflection because we don't know the typeof(T) at compile time.
            // Unfortunatly we can't do this with the 'dynamic
            // keyword because the generic parameters for the method are NOT determined by the input parameters
            // so if we create a 'dynamic' object and call the method with the desired name then the runtime
            // won't be able to figure out what the typed parameter has to be. So we'll do it the 
            // old-fashioned way, with reflection.
            var taskBuilder = GetType()
                .GetMethod(
                    "CreateTask", 
                    BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new Type[] { typeof(Task<ICommunicationMessage>), typeof(TaskScheduler) },
                    null)
                .MakeGenericMethod(genericArguments[0]);

            // Create the return value. This is invoking a MethodInfo object which is
            // slow but we don't expect it to cause too much trouble given that we're getting
            // the result from another application which lives on the other side of a named pipe
            // (best case) or TCP connection (worst case)
            invocation.ReturnValue = taskBuilder.Invoke(null, new object[] { result, m_Scheduler });
        }
    }
}