//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="NotificationRaisedMessage"/>.
    /// </summary>
    internal sealed class NotificationRaisedProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The object that stores all the notification proxies.
        /// </summary>
        private readonly INotifyOfRemoteEndpointEvents m_AvailableProxies;

        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRaisedProcessAction"/> class.
        /// </summary>
        /// <param name="availableNotificationProxies">The collection that holds all the registered notification proxies.</param>
        /// <param name="logger">The function that is used to write messages to the log.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="availableNotificationProxies"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public NotificationRaisedProcessAction(
            INotifyOfRemoteEndpointEvents availableNotificationProxies,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => availableNotificationProxies);
                Enforce.Argument(() => logger);
            }

            m_AvailableProxies = availableNotificationProxies;
            m_Logger = logger;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(NotificationRaisedMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Letting the exception escape will just kill the channel then we won't know what happened, so we log and move on.")]
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as NotificationRaisedMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            var invocation = msg.Notification;
            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Received request to raise event: {0}.{1}",
                    invocation.Type,
                    invocation.MemberName));

            try
            {
                var type = ProxyExtensions.ToType(invocation.Type);
                var notificationSet = m_AvailableProxies.NotificationsFor(msg.OriginatingEndpoint, type);
                Debug.Assert(notificationSet != null, "There should be a proxy for this notification set.");

                var proxyObj = notificationSet as NotificationSetProxy;
                Debug.Assert(proxyObj != null, "The object should be a NotificationSetProxy.");

                proxyObj.RaiseEvent(invocation.MemberName, msg.Arguments);
            }
            catch (Exception e)
            {
                m_Logger(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while raising event {0}.{1}. Exception is: {2}",
                        msg.Notification.Type,
                        msg.Notification.MemberName,
                        e));
            }
        }
    }
}
