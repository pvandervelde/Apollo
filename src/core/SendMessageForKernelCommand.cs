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
    /// Defines a command that sends a message to the rest of the system.
    /// </summary>
    internal sealed class SendMessageForKernelCommand : ICommand
    {
        #region Static members

        /// <summary>
        /// Defines the Id for the <c>SendMessageForKernelCommand</c>.
        /// </summary>
        internal static readonly CommandId CommandId = new CommandId(@"SendMessageForKernel");

        #endregion

        /// <summary>
        /// The delegate used to send a message for which a response is not expected.
        /// </summary>
        private readonly SendMessageWithoutResponse m_MessageSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessageForKernelCommand"/> class.
        /// </summary>
        /// <param name="messageSender">The function used to send a message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="messageSender"/> is <see langword="null"/>.
        /// </exception>
        internal SendMessageForKernelCommand(SendMessageWithoutResponse messageSender)
        {
            {
                Enforce.Argument(() => messageSender);
            }

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
            var commandContext = context as SendMessageForKernelContext;
            Debug.Assert(commandContext != null, "Incorrect command context specified.");

            m_MessageSender(
                commandContext.Recipient, 
                commandContext.Message, 
                MessageId.None);
        }

        #endregion
    }
}
