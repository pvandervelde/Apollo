//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.ServiceModel;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for processing messages from the network.
    /// </summary>
    /// <design>
    /// This class is meant to be able to handle many messages being send at the same time, 
    /// however there should only be one instance of this class so that we can create it
    /// ourselves when we want to.
    /// </design>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    internal sealed class ReceivingEndpoint : IMessagePipe
    {
        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingEndpoint"/> class.
        /// </summary>
        /// <param name="logger">The function that logs messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public ReceivingEndpoint(Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => logger);
            }

            m_Logger = logger;
        }

        /// <summary>
        /// Accepts the messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We don't really want the channel to die just because the other side didn't behave properly.")]
        public void AcceptMessage(ICommunicationMessage message)
        {
            try
            {
                m_Logger(
                    LogSeverityProxy.Trace,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Received message of type {0}.",
                        message.GetType()));

                RaiseOnNewMessage(message);
            }
            catch (Exception e)
            {
                m_Logger(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception occurred during the handling of a message of type {0}. Exception was: {1}",
                        message.GetType(),
                        e));
            }
        }

        /// <summary>
        /// An event raised when a new message is available in the pipe.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnNewMessage;

        private void RaiseOnNewMessage(ICommunicationMessage message)
        {
            var local = OnNewMessage;
            if (local != null)
            {
                local(this, new MessageEventArgs(message));
            }
        }
    }
}
