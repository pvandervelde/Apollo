﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
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
        /// <typeparam name="T">The type of the object carried by the <see cref="CommandInvokedResponseMessage"/> object in the input task.</typeparam>
        /// <param name="inputTask">The task which will deliver the <see cref="ICommunicationMessage"/> that contains the return value.</param>
        /// <returns>
        /// A task returning the desired return type.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method is called via reflection in order to generate the correct return value for a command method.")]
        private static Task<T> CreateTask<T>(Task<ICommunicationMessage> inputTask)
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

                // var failureMsg = inputTask.Result as FailureMessage;
                throw new RemoteOperationFailedException();
            };

            return Task<T>.Factory.StartNew(action, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// The function which sends the <see cref="CommandInvokedMessage"/> to the owning endpoint.
        /// </summary>
        private readonly Func<ISerializedMethodInvocation, Task<ICommunicationMessage>> m_SendMessageWithResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSetMethodWithResultInterceptor"/> class.
        /// </summary>
        /// <param name="sendMessageWithResponse">The function used to send the information about the method invocation to the owning endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessageWithResponse"/> is <see langword="null" />.
        /// </exception>
        public CommandSetMethodWithResultInterceptor(Func<ISerializedMethodInvocation, Task<ICommunicationMessage>> sendMessageWithResponse)
        {
            {
                Enforce.Argument(() => sendMessageWithResponse);
            }

            m_SendMessageWithResponse = sendMessageWithResponse;
        }

        /// <summary>
        /// Called when a method or property call is intercepted.
        /// </summary>
        /// <param name="invocation">Information about the call that was intercepted.</param>
        public void Intercept(IInvocation invocation)
        {
            var result = m_SendMessageWithResponse(
                CommandSetProxyExtensions.FromMethodInfo(
                    invocation.Method,
                    invocation.Arguments));

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
            // through reflection because we don't know the typeof(T) at compile time
            var taskBuilder = GetType()
                .GetMethod("CreateTask")
                .MakeGenericMethod(genericArguments[0]);

            // Create the return value. This is invoking a MethodInfo object which is
            // slow but we don't expect it to cause too much trouble given that we're getting
            // the result from another application which lives on the other side of a named pipe
            // (best case) or TCP connection (worst case)
            invocation.ReturnValue = taskBuilder.Invoke(null, new object[] { result });
        }
    }
}
