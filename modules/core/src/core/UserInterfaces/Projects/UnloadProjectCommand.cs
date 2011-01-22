//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Core.Projects;
using Apollo.Utils.Commands;
using Lokad;
using ICommand = Apollo.Utils.Commands.ICommand;

namespace Apollo.Core.UserInterfaces.Projects
{
    /// <summary>
    /// Defines a command that unloads the current project.
    /// </summary>
    public sealed class UnloadProjectCommand : ICommand
    {
        #region Static members

        /// <summary>
        /// Defines the Id for the <c>UnloadProjectCommand</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A CommandId reference is immutable")]
        public static readonly CommandId CommandId = new CommandId(@"UnloadProject");

        #endregion

        /// <summary>
        /// The delegate used to send a message for which a response is not expected.
        /// </summary>
        private readonly SendMessageWithoutResponse m_MessageSender;

        /// <summary>
        /// The name of the project system.
        /// </summary>
        private readonly DnsName m_ProjectName;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadProjectCommand"/> class.
        /// </summary>
        /// <param name="projectName">The <c>DnsName</c> of the project sub-system.</param>
        /// <param name="messageSender">The function used to send a message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="projectName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="messageSender"/> is <see langword="null"/>.
        /// </exception>
        internal UnloadProjectCommand(DnsName projectName, SendMessageWithoutResponse messageSender)
        {
            {
                Enforce.Argument(() => projectName);
                Enforce.Argument(() => messageSender);
            }

            m_ProjectName = projectName;
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
            var commandContext = context as UnloadProjectContext;
            Debug.Assert(commandContext != null, "Incorrect command context provided.");

            m_MessageSender(m_ProjectName, new UnloadProjectMessage(), MessageId.None);
        }

        #endregion
    }
}
