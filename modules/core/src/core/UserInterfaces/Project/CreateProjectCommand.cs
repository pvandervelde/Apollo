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

namespace Apollo.Core.UserInterfaces.Project
{
    /// <summary>
    /// Defines a command that creates a new project.
    /// </summary>
    public sealed class CreateProjectCommand : ICommand
    {
        #region Static members

        /// <summary>
        /// Defines the Id for the <c>CreateProjectCommand</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A CommandId reference is immutable")]
        public static readonly CommandId CommandId = new CommandId(@"CreateProject");

        #endregion

        /// <summary>
        /// The delegate used to send a message for which a response is expected.
        /// </summary>
        private readonly SendMessageWithResponse m_MessageSender;

        /// <summary>
        /// The name of the project system.
        /// </summary>
        private readonly DnsName m_ProjectName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProjectCommand"/> class.
        /// </summary>
        /// <param name="projectName">The <c>DnsName</c> of the project sub-system.</param>
        /// <param name="messageSender">The function used to send a message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="projectName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="messageSender"/> is <see langword="null"/>.
        /// </exception>
        internal CreateProjectCommand(DnsName projectName, SendMessageWithResponse messageSender)
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
            var commandContext = context as CreateProjectContext;
            Debug.Assert(commandContext != null, "Incorrect command context provided.");

            var future = m_MessageSender(m_ProjectName, new CreateNewProjectMessage(), MessageId.None);
            var body = future.Result() as ProjectRequestResponseMessage;
            Debug.Assert(body != null, "Incorrect message response received.");

            commandContext.Result = body.ProjectReference;
        }

        #endregion
    }
}
