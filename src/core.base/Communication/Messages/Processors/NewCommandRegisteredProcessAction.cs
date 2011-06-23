//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="NewCommandRegisteredMessage"/>.
    /// </summary>
    internal sealed class NewCommandRegisteredProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The object that forwards information about the newly registered commands.
        /// </summary>
        private readonly IAceptExternalCommandInformation m_CommandRegistrator;

        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewCommandRegisteredProcessAction"/> class.
        /// </summary>
        /// <param name="commandRegistrator">
        ///     The object that forwards information about the newly registered commands.
        /// </param>
        /// <param name="logger">The function used to write messages to the log.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandRegistrator"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public NewCommandRegisteredProcessAction(
            IAceptExternalCommandInformation commandRegistrator,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => commandRegistrator);
                Enforce.Argument(() => logger);
            }

            m_CommandRegistrator = commandRegistrator;
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
                return typeof(NewCommandRegisteredMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as NewCommandRegisteredMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            m_CommandRegistrator.RecentlyRegisteredCommand(msg.OriginatingEndpoint, msg.Command);
        }
    }
}
