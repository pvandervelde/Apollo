﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages
{
    /// <summary>
    /// Defines a message that is used to request registration to a notification.
    /// </summary>
    [Serializable]
    internal sealed class RegisterForNotificationMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterForNotificationMessage"/> class.
        /// </summary>
        /// <param name="origin">
        /// The ID of the endpoint that send the message.
        /// </param>
        /// <param name="notificationToSubscribeTo">The notification to which the sender wants to subscribe.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationToSubscribeTo"/> is <see langword="null" />.
        /// </exception>
        public RegisterForNotificationMessage(EndpointId origin, ISerializedEventRegistration notificationToSubscribeTo)
            : base(origin)
        {
            {
                Enforce.Argument(() => notificationToSubscribeTo);
            }

            Notification = notificationToSubscribeTo;
        }

        /// <summary>
        /// Gets the notification to which the sender of the current message wants to subscribe.
        /// </summary>
        public ISerializedEventRegistration Notification 
        { 
            get; 
            private set; 
        }
    }
}
