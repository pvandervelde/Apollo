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
    /// Defines a command that loads an existing project.
    /// </summary>
    internal sealed class LoadProjectCommand : ICommand
    {
        #region Static members

        /// <summary>
        /// Defines the Id for the <c>LoadProjectCommand</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A CommandId reference is immutable")]
        public static readonly CommandId CommandId = new CommandId(@"LoadProject");

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
        /// Initializes a new instance of the <see cref="LoadProjectCommand"/> class.
        /// </summary>
        /// <param name="projectName">The <c>DnsName</c> of the project sub-system.</param>
        /// <param name="messageSender">The function used to send a message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="projectName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="messageSender"/> is <see langword="null"/>.
        /// </exception>
        internal LoadProjectCommand(DnsName projectName, SendMessageWithResponse messageSender)
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
            var commandContext = context as LoadProjectContext;
            Debug.Assert(commandContext != null, "Incorrect command context provided.");

            var future = m_MessageSender(m_ProjectName, new LoadProjectMessage(commandContext.LoadFrom), MessageId.None);
            var body = future.Result() as ProjectRequestResponseMessage;
            Debug.Assert(body != null, "Incorrect message response received.");

            commandContext.Result = body.ProjectReference;
        }

        #endregion
    }
}
