//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using Lokad;
using ICommand = Apollo.Utils.Commands.ICommand;

namespace Apollo.Core
{
    /// <summary>
    /// A command used to request a shut down from the application.
    /// </summary>
    public sealed class ShutdownApplicationCommand : ICommand
    {
        #region internal class - ShutdownApplicationContext

        /// <summary>
        /// Defines an <see cref="ICommandContext"/> for the <see cref="ShutdownApplicationCommand"/>.
        /// </summary>
        public sealed class ShutdownApplicationContext : ICommandContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ShutdownApplicationContext"/> class.
            /// </summary>
            /// <param name="isForced">If set to <see langword="true"/> then a forced shutdown is requested.</param>
            public ShutdownApplicationContext(bool isForced)
            {
                IsShutdownForced = isForced;
            }

            /// <summary>
            /// Gets a value indicating whether this shutdown is forced.
            /// </summary>
            /// <value>
            ///     <see langword="true"/> if this shutdown is forced; otherwise, <see langword="false"/>.
            /// </value>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsShutdownForced
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the application is shutting down or not.
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
        public static readonly CommandId CommandId = new CommandId(@"ShutdownApplication");

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
        /// Initializes a new instance of the <see cref="ShutdownApplicationCommand"/> class.
        /// </summary>
        /// <param name="kernelName">The <see cref="DnsName"/> of the kernel.</param>
        /// <param name="messageSender">The function used to send a message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="kernelName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="messageSender"/> is <see langword="null"/>.
        /// </exception>
        internal ShutdownApplicationCommand(DnsName kernelName, SendMessageWithResponseDelegate messageSender)
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
            var commandContext = context as ShutdownApplicationContext;
            Debug.Assert(commandContext != null, "Incorrect command context specified.");

            var future = m_MessageSender(m_KernelName, new ShutdownRequestMessage(commandContext.IsShutdownForced), MessageId.None);
            var body = future.Result() as ShutdownResponseMessage;
            Debug.Assert(body != null, "Incorrect message response received.");

            commandContext.Result = body.WasRequestGranted;
        }

        #endregion
    }
}
