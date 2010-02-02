//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using Lokad;
using ICommand = Apollo.Utils.Commands.ICommand;

namespace Apollo.Core
{
    /// <summary>
    /// Defines a command that checks if the application can be shut down.
    /// </summary>
    public sealed class CheckApplicationCanShutdownCommand : ICommand
    {
        #region internal class - CheckApplicationCanShutdownContext

        /// <summary>
        /// Defines an <see cref="ICommandContext"/> for the <see cref="CheckApplicationCanShutdownCommand"/>.
        /// </summary>
        public sealed class CheckApplicationCanShutdownContext : ICommandContext
        {
            /// <summary>
            /// Gets or sets a value indicating whether the application can be shutdown or not.
            /// </summary>
            /// <value>
            /// The command result.
            /// </value>
            public bool Result
            {
                get;
                set;
            }
        }

        #endregion

        #region Static members

        /// <summary>
        /// Defines the Id for the <c>CheckApplicationCanShutdownCommand</c>.
        /// </summary>
        public static readonly CommandId CommandId = new CommandId(@"CheckApplicationCanShutdown");

        #endregion

        /// <summary>
        /// The delegate used to send a message for which a response is expected.
        /// </summary>
        private readonly SendMessageWithResponseDelegate m_MessageSender;

        /// <summary>
        /// The name of the kernel.
        /// </summary>
        private readonly DnsName m_KernelName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckApplicationCanShutdownCommand"/> class.
        /// </summary>
        /// <param name="kernelName">The <see cref="DnsName"/> of the kernel.</param>
        /// <param name="messageSender">The function used to send a message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="kernelName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="messageSender"/> is <see langword="null"/>.
        /// </exception>
        internal CheckApplicationCanShutdownCommand(DnsName kernelName, SendMessageWithResponseDelegate messageSender)
        {
            {
                Enforce.Argument(() => kernelName);
                Enforce.Argument(() => messageSender);
            }

            m_KernelName = kernelName;
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
            var commandContext = context as CheckApplicationCanShutdownContext;
            Debug.Assert(commandContext != null, "Incorrect command context specified.");

            var future = m_MessageSender(m_KernelName, new ApplicationShutdownCapabilityRequestMessage(), MessageId.None);
            var body = future.Result() as ApplicationShutdownCapabilityResponseMessage;
            Debug.Assert(body != null, "Incorrect message response received.");

            commandContext.Result = body.CanShutdown;
        }

        #endregion
    }
}
