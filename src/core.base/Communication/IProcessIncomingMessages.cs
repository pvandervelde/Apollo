//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Processes incoming messages and invokes the required functions based on the 
    /// contents of the message.
    /// </summary>
    internal interface IProcessIncomingMessages
    {
        /// <summary>
        /// Processes the message and invokes the desired functions based on the 
        /// message contents or type.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        void ProcessMessage(ICommunicationMessage message);

        /// <summary>
        /// Handles the case that the local channel, from which the input messages are send,
        /// is closed.
        /// </summary>
        void OnLocalChannelClosed();
    }
}
