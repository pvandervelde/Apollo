//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Messages
{
    /// <summary>
    /// An <see cref="EventArgs"/> class for message events.
    /// </summary>
    [Serializable]
    public sealed class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        /// <param name="deliveryFailureReason">The delivery failure reason.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="messageId"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="deliveryFailureReason"/> is <see langword="null"/>.
        /// </exception>
        public MessageEventArgs(MessageId messageId, IDeliveryFailureReason deliveryFailureReason)
        {
            {
                Enforce.Argument(() => messageId);
                Enforce.Argument(() => deliveryFailureReason);
            }

            Id = messageId;
            FailureReason = deliveryFailureReason;
        }

        /// <summary>
        /// Gets the ID of the message for which delivery failed.
        /// </summary>
        /// <value>The ID of the message.</value>
        public MessageId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the reason for the delivery failure.
        /// </summary>
        /// <value>The failure delivery reason.</value>
        public IDeliveryFailureReason FailureReason
        {
            get;
            private set;
        }
    }
}
