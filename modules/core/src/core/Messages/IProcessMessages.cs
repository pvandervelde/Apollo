//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// Defines the interface for <see cref="KernelService"/> objects that need
    /// to be able to process messages.
    /// </summary>
    public interface IProcessMessages : IDnsNameObject
    {
        /// <summary>
        /// Processes a single message that is directed at the current service.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        void ProcessMessage(KernelMessage message);

        /// <summary>
        /// Processes a set of messages which are directed at the current service.
        /// </summary>
        /// <param name="messages">The set of messages which should be processed.</param>
        void ProcessMessages(IEnumerable<KernelMessage> messages);
    }
}
