//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for processing messages from the network.
    /// </summary>
    /// <design>
    /// This class is meant to be able to handle many messages being send at the same time, 
    /// however there should only be one instance of this class so that we can create it
    /// ourselves when we want to.
    /// </design>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    internal sealed class ReceivingEndpoint : IMessagePipe
    {
        /// <summary>
        /// Accepts the messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AcceptMessage(ICommunicationMessage message)
        {
            try
            {
                RaiseOnNewMessage(message);
            }
            catch (Exception)
            {
                // Should we send this back through the channel? We can't really can we??
                // We should probably log something here ....
            }
        }

        /// <summary>
        /// An event raised when a new message is available in the pipe.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnNewMessage;

        private void RaiseOnNewMessage(ICommunicationMessage message)
        {
            var local = OnNewMessage;
            if (local != null)
            {
                local(this, new MessageEventArgs(message));
            }
        }
    }
}
