﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="CommandInvokedMessage"/>.
    /// </summary>
    internal sealed class CommandInvokedProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// An internal class that holds the method used to process <see cref="Task{T}"/> objects at runtime.
        /// </summary>
        /// <remarks>
        /// The method is wrapped in a class so that we can assign it to a variable typed as <c>dynamic</c>.
        /// By using that kind of typing we don't need to specify the exact method signature, which we don't
        /// know at compile time due to the missing information about the generic type.
        /// </remarks>
        private sealed class TaskReturn
        {
            /// <summary>
            /// The action used to write information to the log.
            /// </summary>
            private readonly Action<LogSeverityProxy, string> m_Logger;

            /// <summary>
            /// Initializes a new instance of the <see cref="TaskReturn"/> class.
            /// </summary>
            /// <param name="logger">The action used to write information to the log.</param>
            public TaskReturn(Action<LogSeverityProxy, string> logger)
            {
                {
                    Debug.Assert(logger != null, "The logger variable should not be null.");
                }

                m_Logger = logger;
            }

            /// <summary>
            /// Provides a typed way of creating a return message based on the outcome of the invocation of a given command. This method 
            /// will only be invoked through reflection.
            /// </summary>
            /// <param name="local">The endpoint ID of the local endpoint.</param>
            /// <param name="originalMsg">The message that was send to invoke a given command.</param>
            /// <param name="returnValue">The task that will, eventually, return the desired result.</param>
            /// <returns>The communication message that should be send if the task finishes successfully.</returns>
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
                Justification = "Will not make this method static so that the signature is consistent with HandleTypedTaskReturnValue<T>.")]
            public ICommunicationMessage HandleTaskReturnValue(EndpointId local, ICommunicationMessage originalMsg, Task returnValue)
            {
                if (returnValue.IsCanceled || returnValue.IsFaulted)
                {
                    m_Logger(
                        LogSeverityProxy.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The task has failed. Exception is: {0}",
                            returnValue.Exception));

                    return new FailureMessage(local, originalMsg.Id);
                }

                return new SuccessMessage(local, originalMsg.Id);
            }

            /// <summary>
            /// Provides a typed way of creating a return message based on the outcome of the invocation of a given command. This method 
            /// will only be invoked through reflection.
            /// </summary>
            /// <typeparam name="T">The type of the return value.</typeparam>
            /// <param name="local">The endpoint ID of the local endpoint.</param>
            /// <param name="originalMsg">The message that was send to invoke a given command.</param>
            /// <param name="returnValue">The task that will, eventually, return the desired result.</param>
            /// <returns>The communication message that should be send if the task finishes successfully.</returns>
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
                Justification = "Cannot make this method static because then we cannot use 'dynamic' anymore to get to it.")]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                Justification = "This code is called via reflection.")]
            public ICommunicationMessage HandleTypedTaskReturnValue<T>(EndpointId local, ICommunicationMessage originalMsg, Task<T> returnValue)
            {
                if (returnValue.IsCanceled || returnValue.IsFaulted)
                {
                    m_Logger(
                        LogSeverityProxy.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The task has failed. Exception is: {0}",
                            returnValue.Exception));

                    return new FailureMessage(local, originalMsg.Id);
                }

                return new CommandInvokedResponseMessage(local, originalMsg.Id, returnValue.Result);
            }
        }

        /// <summary>
        /// The collection that holds all the registered commands.
        /// </summary>
        private readonly ICommandCollection m_Commands;

        /// <summary>
        /// The action that is used to send a message to a remote endpoint.
        /// </summary>
        private readonly Action<EndpointId, ICommunicationMessage> m_SendMessage;

        /// <summary>
        /// The endpoint ID of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Current;

        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvokedProcessAction"/> class.
        /// </summary>
        /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
        /// <param name="sendMessage">The action that is used to send messages.</param>
        /// <param name="availableCommands">The collection that holds all the registered commands.</param>
        /// <param name="logger">The function that is used to write messages to the log.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="availableCommands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public CommandInvokedProcessAction(
            EndpointId localEndpoint,
            Action<EndpointId, ICommunicationMessage> sendMessage,
            ICommandCollection availableCommands,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => localEndpoint);
                Enforce.Argument(() => sendMessage);
                Enforce.Argument(() => availableCommands);
                Enforce.Argument(() => logger);
            }

            m_Current = localEndpoint;
            m_SendMessage = sendMessage;
            m_Commands = availableCommands;
            m_Logger = logger;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(CommandInvokedMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Letting the exception escape will just kill the channel then we won't know what happened, so we log and move on.")]
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as CommandInvokedMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            var invocation = msg.Invocation;
            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Received request to execute command: {0}.{1}",
                    invocation.Type,
                    invocation.MemberName));

            Task result = null;
            try
            {
                var type = ProxyExtensions.ToType(invocation.Type);
                var commandSet = m_Commands.CommandsFor(type);

                var parameterTypes = from pair in invocation.Parameters
                                     select ProxyExtensions.ToType(pair.Item1);
                var method = type.GetMethod(invocation.MemberName, parameterTypes.ToArray());
                
                var parameterValues = from pair in invocation.Parameters
                                      select pair.Item2;
                result = method.Invoke(commandSet, parameterValues.ToArray()) as Task;
                
                Debug.Assert(result != null, "The command return result was not a Task or Task<T>.");
                result.ContinueWith(
                    t => ProcessTaskReturnResult(msg, t),
                    TaskContinuationOptions.ExecuteSynchronously);
            }
            catch (Exception e)
            {
                HandleCommandExecutionFailure(msg, e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "There is no point crashing the current app without being able to notify the other side of the channel.")]
        private void ProcessTaskReturnResult(CommandInvokedMessage msg, Task result)
        {
            try
            {
                ICommunicationMessage returnMsg = null;
                if (result == null)
                {
                    returnMsg = new FailureMessage(m_Current, msg.Id);
                }
                else
                {
                    // The resulting type can either be Task or Task<T>
                    var resultType = result.GetType();
                    Debug.Assert(!resultType.ContainsGenericParameters, "The return type should be a closed constructed type.");

                    var genericArguments = resultType.GetGenericArguments();
                    Debug.Assert(
                        genericArguments.Length == 0 || genericArguments.Length == 1, 
                        "There should either be zero or one generic argument.");
                    if (genericArguments.Length == 0)
                    {
                        m_Logger(
                        LogSeverityProxy.Trace,
                        "Returning Task value.");

                        returnMsg = new TaskReturn(m_Logger).HandleTaskReturnValue(m_Current, msg, result);
                    }
                    else
                    {
                        m_Logger(
                        LogSeverityProxy.Trace,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Returning Task<T> value. T is {0}",
                            genericArguments[0]));

                        // The result is Task<T>. This is where things are about to get very messy
                        // We need to use the HandleTaskReturnValue(EndpointId, MessageId, Task<T>) method to get our message
                        //
                        // So 'build' a method that can process the Task<T> object. We can do this with the 'dynamic
                        // keyword because the generic parameters for the method are determined by the input parameters
                        // so if we create a 'dynamic' object and call the method with the desired name then the runtime
                        // will determine what the type parameter has to be.
                        dynamic taskBuilder = new TaskReturn(m_Logger);

                        // Call the desired method, making sure that we force the runtime to use the runtime type of the result
                        // variable, not the compile time one.
                        returnMsg = (ICommunicationMessage)taskBuilder.HandleTypedTaskReturnValue(m_Current, msg, (dynamic)result);
                    }
                }

                m_SendMessage(msg.OriginatingEndpoint, returnMsg);
            }
            catch (Exception e)
            {
                HandleCommandExecutionFailure(msg, e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "There is no point crashing the current app without being able to notify the other side of the channel.")]
        private void HandleCommandExecutionFailure(CommandInvokedMessage msg, Exception e)
        {
            try
            {
                m_Logger(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while invoking command {0}.{1}. Exception is: {2}",
                        msg.Invocation.Type,
                        msg.Invocation.MemberName,
                        e));
                m_SendMessage(msg.OriginatingEndpoint, new FailureMessage(m_Current, msg.Id));
            }
            catch (Exception errorSendingException)
            {
                m_Logger(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while trying to send process failure. Exception is: {0}",
                        errorSendingException));
            }
        }
    }
}