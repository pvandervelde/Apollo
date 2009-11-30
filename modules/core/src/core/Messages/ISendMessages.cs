//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// The interface for objects which use the <see cref="MessagePipeline"/> to send messages
    /// to the services running in the kernel.
    /// </summary>
    public interface ISendMessages : IDnsNameObject
    {
        /// <summary>
        /// Called when a message could not be delivered.
        /// </summary>
        /// <param name="messageId">The message ID of the failed message.</param>
        /// <param name="recipient">The recipient to which the failed message was to be send.</param>
        /// <param name="information">The information.</param>
        /// <param name="failureReason">The reason the message delivery failed.</param>
        void OnMessageDeliveryFailure(MessageId messageId, DnsName recipient, MessageBody information, IDeliveryFailureReason failureReason);

        /// <summary>
        /// Called when a message could not be delivered.
        /// </summary>
        /// <param name="messageId">The message ID of the failed message.</param>
        /// <param name="recipient">The recipient to which the failed message was to be send.</param>
        /// <param name="information">The information.</param>
        /// <param name="exception">The exception which occurred during the message sending.</param>
        void OnMessageDeliveryFailure(MessageId messageId, DnsName recipient, MessageBody information, Exception exception);
    }
}
