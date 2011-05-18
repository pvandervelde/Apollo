//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
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
        /// <returns>
        /// A task returning the desired return type.
        /// </returns>
        private static Task CreateTask(Task<ICommunicationMessage> inputTask)
        {
            Action action = () =>
            {
                inputTask.Wait();

                var successMsg = inputTask.Result as SuccessMessage;
                if (successMsg != null)
                {
                    return;
                }

                // var failureMsg = inputTask.Result as FailureMessage;
                throw new RemoteOperationFailedException();
            };

            return Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// The function which sends the <see cref="CommandInvokedMessage"/> to the owning endpoint.
        /// </summary>
        private readonly Func<ISerializedMethodInvocation, Task<ICommunicationMessage>> m_SendMessageWithResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSetMethodWithoutResultInterceptor"/> class.
        /// </summary>
        /// <param name="sendMessageWithResponse">The function used to send the information about the method invocation to the owning endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessageWithResponse"/> is <see langword="null" />.
        /// </exception>
        public CommandSetMethodWithoutResultInterceptor(Func<ISerializedMethodInvocation, Task<ICommunicationMessage>> sendMessageWithResponse)
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

            invocation.ReturnValue = CreateTask(result);
        }
    }
}
