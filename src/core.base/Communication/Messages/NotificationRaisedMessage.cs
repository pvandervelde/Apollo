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
    /// Defines a message that indicates that the an <see cref="INotificationSet"/> event was
    /// raised.
    /// </summary>
    [Serializable]
    internal sealed class NotificationRaisedMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRaisedMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="eventNotification">The information about the <see cref="INotificationSet"/> event that was raised.</param>
        /// <param name="args">The event arguments that were provided by the event.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="eventNotification"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="args"/> is <see langword="null" />.
        /// </exception>
        public NotificationRaisedMessage(EndpointId origin, ISerializedEventRegistration eventNotification, EventArgs args)
            : base(origin)
        {
            {
                Enforce.Argument(() => eventNotification);
                Enforce.Argument(() => args);
            }

            Notification = eventNotification;
            Arguments = args;
        }

        /// <summary>
        /// Gets information about the <see cref="INotificationSet"/> event that was raised.
        /// </summary>
        public ISerializedEventRegistration Notification
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the event arguments that were provided by the event.
        /// </summary>
        public EventArgs Arguments
        {
            get;
            private set;
        }
    }
}
