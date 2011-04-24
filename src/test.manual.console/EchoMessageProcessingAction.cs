//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Base.Communication;
using Test.Manual.Console.Models;

namespace Test.Manual.Console
{
    /// <summary>
    /// A <see cref="IMessageProcessAction"/> that handles <see cref="EchoMessage"/>.
    /// </summary>
    internal sealed class EchoMessageProcessingAction : IMessageProcessAction
    {
        /// <summary>
        /// The collection that stores all the messages.
        /// </summary>
        private readonly ConnectionViewModel m_Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoMessageProcessingAction"/> class.
        /// </summary>
        /// <param name="model">The collection that holds the connection events.</param>
        public EchoMessageProcessingAction(ConnectionViewModel model)
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
                return typeof(EchoMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as EchoMessage;
            Debug.Assert(msg != null, "Incorrect message type found.");

            m_Model.AddNewMessage(msg.OriginatingEndpoint, msg.Text);
        }
    }
}
