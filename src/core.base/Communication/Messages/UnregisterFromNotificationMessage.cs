//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that is used to request unregistration from a notification.
    /// </summary>
    [Serializable]
    internal sealed class UnregisterFromNotificationMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisterFromNotificationMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="notificationToUnsubscribeFrom">The notification from which the sender wants to unsubscribe.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationToUnsubscribeFrom"/> is <see langword="null" />.
        /// </exception>
        public UnregisterFromNotificationMessage(EndpointId origin, ISerializedEventRegistration notificationToUnsubscribeFrom)
            : base(origin)
        {
            {
                Enforce.Argument(() => notificationToUnsubscribeFrom);
            }

            Notification = notificationToUnsubscribeFrom;
        }

        /// <summary>
        /// Gets the notification from which the sender of the current message wants to unsubscribe.
        /// </summary>
        public ISerializedEventRegistration Notification 
        { 
            get; 
            private set; 
        }
    }
}
