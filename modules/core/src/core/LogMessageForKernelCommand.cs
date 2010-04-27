//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using Lokad;
using Lokad.Rules;
using ICommand = Apollo.Utils.Commands.ICommand;

namespace Apollo.Core
{
    /// <summary>
    /// Defines a command that sends a log message to the logger.
    /// </summary>
    internal sealed class LogMessageForKernelCommand : ICommand
    {
        #region internal class - LogMessageForKernelContext

        /// <summary>
        /// Defines and <see cref="ICommandContext"/> for the <see cref="LogMessageForKernelCommand"/>.
        /// </summary>
        internal sealed class LogMessageForKernelContext : ICommandContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LogMessageForKernelContext"/> class.
            /// </summary>
            /// <param name="level">The log level.</param>
            /// <param name="message">The log message.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="message"/> is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentException">
            ///     Thrown if <paramref name="message"/> is an empty string.
            /// </exception>
            public LogMessageForKernelContext(LevelToLog level, string message)
            {
                {
                    Enforce.Argument(() => message);
                    Enforce.Argument(() => message, StringIs.NotEmpty);
                }

                Level = level;
                Message = message;
            }

            /// <summary>
            /// Gets the log level.
            /// </summary>
            /// <value>The log level.</value>
            public LevelToLog Level
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            /// <value>The message.</value>
            public string Message
            {
                get;
                private set;
            }
        }

        #endregion
        
        #region Static members

        /// <summary>
        /// Defines the Id for the <c>CheckApplicationCanShutdownCommand</c>.
        /// </summary>
        internal static readonly CommandId CommandId = new CommandId(@"LogMessageForKernel");

        #endregion

        /// <summary>
        /// The delegate used to send a message for which a response is not expected.
        /// </summary>
        private readonly SendMessageWithoutResponseDelegate m_MessageSender;

        /// <summary>
        /// The name of the logsink.
        /// </summary>
        private readonly DnsName m_LogSinkName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageForKernelCommand"/> class.
        /// </summary>
        /// <param name="logSinkName">The <see cref="DnsName"/> of the kernel.</param>
        /// <param name="messageSender">The function used to send a message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="logSinkName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="messageSender"/> is <see langword="null"/>.
        /// </exception>
        internal LogMessageForKernelCommand(DnsName logSinkName, SendMessageWithoutResponseDelegate messageSender)
        {
            {
                Enforce.Argument(() => logSinkName);
                Enforce.Argument(() => messageSender);
            }

            m_LogSinkName = logSinkName;
            m_MessageSender = messageSender;
        }

        #region Implementation of ICommand

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID for the command.</value>
        public CommandId Id
        {
            get
            {
                return CommandId;
            }
        }

        /// <summary>
        /// Invokes the current command with the specified context as input.
        /// </summary>
        /// <param name="context">The context for the command.</param>
        public void Invoke(ICommandContext context)
        {
            var commandContext = context as LogMessageForKernelContext;
            Debug.Assert(commandContext != null, "Incorrect command context specified.");

            m_MessageSender(
                m_LogSinkName, 
                new LogEntryRequestMessage(
                    new LogMessage(
                        m_LogSinkName.ToString(),
                        commandContext.Level,
                        commandContext.Message),
                    LogType.Debug), 
                MessageId.None);
        }

        #endregion
    }
}
