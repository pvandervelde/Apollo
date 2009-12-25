//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Defines a <see cref="IDeliveryFailureReason"/> object that indicates that
    /// the sending of the <see cref="KernelMessage"/> object failed.
    /// </summary>
    [Serializable]
    public sealed class FailedToSendMessageReason : IDeliveryFailureReason
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToSendMessageReason"/> class.
        /// </summary>
        /// <param name="failedWith">The failed with.</param>
        public FailedToSendMessageReason(Exception failedWith)
        {
            ExceptionOnFailure = failedWith;
        }

        /// <summary>
        /// Gets the exception which was thrown when the failure occurred.
        /// </summary>
        /// <value>The exception on failure.</value>
        public Exception ExceptionOnFailure
        {
            get;
            private set;
        }
    }
}
