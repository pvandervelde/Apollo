﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="NotificationInformationRequestMessage"/>.
    /// </summary>
    internal sealed class NotificationInformationRequestProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The collection that contains all the avaible notifications.
        /// </summary>
        private readonly INotificationSendersCollection m_Notifications;

        /// <summary>
        /// The action that is used to send a message to a remote endpoint.
        /// </summary>
        private readonly Action<EndpointId, ICommunicationMessage> m_SendMessage;

        /// <summary>
        /// The endpoint ID of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Current;

        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInformationRequestProcessAction"/> class.
        /// </summary>
        /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
        /// <param name="sendMessage">The action that is used to send messages.</param>
        /// <param name="availableNotifications">The collection that holds all the registered notifications.</param>
        /// <param name="logger">The function that is used to write messages to the log.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="availableNotifications"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public NotificationInformationRequestProcessAction(
            EndpointId localEndpoint,
            Action<EndpointId, ICommunicationMessage> sendMessage,
            INotificationSendersCollection availableNotifications,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => localEndpoint);
                Enforce.Argument(() => sendMessage);
                Enforce.Argument(() => availableNotifications);
                Enforce.Argument(() => logger);
            }

            m_Current = localEndpoint;
            m_SendMessage = sendMessage;
            m_Notifications = availableNotifications;
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
                return typeof(NotificationInformationRequestMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "If the exception escapes then the channel dies but we won't know what happened, so we log and move on.")]
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as NotificationInformationRequestMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            var notifications = new List<Type>(m_Notifications.Select(p => p.Key));
            try
            {
                var returnMessage = new EndpointProxyTypesResponseMessage(m_Current, msg.Id, notifications.ToArray());
                m_SendMessage(msg.OriginatingEndpoint, returnMessage);
            }
            catch (Exception e)
            {
                try
                {
                    m_Logger(
                        LogSeverityProxy.Error, 
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Error while sending endpoint information. Exception is: {0}",
                            e));
                    m_SendMessage(msg.OriginatingEndpoint, new FailureMessage(m_Current, msg.Id));
                }
                catch (Exception errorSendingException)
                {
                    m_Logger(
                        LogSeverityProxy.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Error while trying to send process failure. Exception is: {0}",
                            errorSendingException));
                }
            }
        }
    }
}
