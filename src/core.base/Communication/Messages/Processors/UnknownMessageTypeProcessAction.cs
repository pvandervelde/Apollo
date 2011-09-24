//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an any kind of <see cref="ICommunicationMessage"/>
    /// for which no filter has been registered.
    /// </summary>
    [LastChanceMessageHandler]
    internal sealed class UnknownMessageTypeProcessAction : IMessageProcessAction
    {
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
        /// Initializes a new instance of the <see cref="UnknownMessageTypeProcessAction"/> class.
        /// </summary>
        /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
        /// <param name="sendMessage">The action that is used to send messages.</param>
        /// <param name="logger">The function that is used to write messages to the log.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public UnknownMessageTypeProcessAction(
            EndpointId localEndpoint,
            Action<EndpointId, ICommunicationMessage> sendMessage,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => localEndpoint);
                Enforce.Argument(() => sendMessage);
                Enforce.Argument(() => logger);
            }

            m_Current = localEndpoint;
            m_SendMessage = sendMessage;
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
                return typeof(ICommunicationMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as UnknownMessageTypeMessage;
            if (msg != null)
            {
                // We don't want to respond to our own messages otherwise we get a nasty
                // feedback loop that just keeps going, and going and going and going and
                // .... well I think you get the point.
                return;
            }

            try
            {
                var returnMessage = new UnknownMessageTypeMessage(m_Current, message.Id);
                m_SendMessage(message.OriginatingEndpoint, returnMessage);
            }
            catch (Exception e)
            {
                try
                {
                    m_Logger(
                        LogSeverityProxy.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Error while sending a message indicating that received message was of an unknown type. Exception is: {0}",
                            e));
                    m_SendMessage(message.OriginatingEndpoint, new FailureMessage(m_Current, message.Id));
                }
                catch (Exception errorSendingException)
                {
                    m_Logger(
                        LogSeverityProxy.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Error while trying to send unknown message response. Exception is: {0}",
                            errorSendingException));
                }
            }
        }
    }
}
