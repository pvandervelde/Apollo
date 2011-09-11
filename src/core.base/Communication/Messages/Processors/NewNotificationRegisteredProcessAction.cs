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
    /// Defines the action that processes an <see cref="NewNotificationRegisteredMessage"/>.
    /// </summary>
    internal sealed class NewNotificationRegisteredProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The object that forwards information about the newly registered commands.
        /// </summary>
        private readonly IAcceptExternalProxyInformation m_NotificationRegistrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewNotificationRegisteredProcessAction"/> class.
        /// </summary>
        /// <param name="notificationRegistrator">
        ///     The object that forwards information about the newly registered notifications.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationRegistrator"/> is <see langword="null" />.
        /// </exception>
        public NewNotificationRegisteredProcessAction(
            IAcceptExternalProxyInformation notificationRegistrator)
        {
            {
                Enforce.Argument(() => notificationRegistrator);
            }

            m_NotificationRegistrator = notificationRegistrator;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(NewNotificationRegisteredMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as NewNotificationRegisteredMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            m_NotificationRegistrator.RecentlyRegisteredProxy(msg.OriginatingEndpoint, msg.Notification);
        }
    }
}
