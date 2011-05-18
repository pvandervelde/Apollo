//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Communication;
using Test.Manual.Console.Models;

namespace Test.Manual.Console
{
    /// <summary>
    /// A <see cref="IMessageProcessAction"/> that handles any kind of <see cref="ICommunicationMessage"/>.
    /// </summary>
    internal sealed class AllMessageProcessingAction : IMessageProcessAction
    {
        /// <summary>
        /// The collection that stores all the messages.
        /// </summary>
        private readonly ConnectionViewModel m_Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllMessageProcessingAction"/> class.
        /// </summary>
        /// <param name="model">The collection that holds the connection events.</param>
        public AllMessageProcessingAction(ConnectionViewModel model)
        {
            m_Model = model;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get 
            {
                return typeof(ICommunicationMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            m_Model.AddNewMessage(message.OriginatingEndpoint, string.Format("Message of type {0}", message.GetType()));
        }
    }
}
