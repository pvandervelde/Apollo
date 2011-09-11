//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="RegisterForNotificationMessage"/>.
    /// </summary>
    internal sealed class RegisterForNotificationProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The object that stores the notification registrations.
        /// </summary>
        private readonly ISendNotifications m_NotificationSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterForNotificationProcessAction"/> class.
        /// </summary>
        /// <param name="notificationSender">The object that stores the registrations.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationSender"/> is <see langword="null" />.
        /// </exception>
        public RegisterForNotificationProcessAction(ISendNotifications notificationSender)
        {
            {
                Enforce.Argument(() => notificationSender);
            }

            m_NotificationSender = notificationSender;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(RegisterForNotificationMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as RegisterForNotificationMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            m_NotificationSender.RegisterForNotification(msg.OriginatingEndpoint, msg.Notification);
        }
    }
}
