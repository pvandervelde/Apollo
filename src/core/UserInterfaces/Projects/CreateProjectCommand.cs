//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Projects;
using Apollo.Utils.Commands;
using Lokad;
using ICommand = Apollo.Utils.Commands.ICommand;

namespace Apollo.Core.UserInterfaces.Projects
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
        /// The function that is used to create the new project.
        /// </summary>
        private Func<IProject> m_Creator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProjectCommand"/> class.
        /// </summary>
        /// <param name="projectCreator">The function that is used to create the new project.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="projectCreator"/> is <see langword="null"/>.
        /// </exception>
        internal CreateProjectCommand(Func<IProject> projectCreator)
        {
            {
                Enforce.Argument(() => projectCreator);
            }

            m_Creator = projectCreator;
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

            commandContext.Result = Task<IProject>.Factory.StartNew(m_Creator);
        }

        #endregion
    }
}
