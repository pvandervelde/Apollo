//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Projects;
using Apollo.Utilities.Commands;
using Lokad;
using ICommand = Apollo.Utilities.Commands.ICommand;

namespace Apollo.Core.UserInterfaces.Projects
{
    /// <summary>
    /// Defines a command that creates a new project.
    /// </summary>
    internal sealed class CreateProjectCommand : ICommand
    {
        /// <summary>
        /// Defines the Id for the <c>CreateProjectCommand</c>.
        /// </summary>
        public static readonly CommandId CommandId = new CommandId(@"CreateProject");

        /// <summary>
        /// The function that is used to create the new project.
        /// </summary>
        private readonly Func<IProject> m_Creator;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProjectCommand"/> class.
        /// </summary>
        /// <param name="projectCreator">The function that is used to create the new project.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="projectCreator"/> is <see langword="null"/>.
        /// </exception>
        internal CreateProjectCommand(Func<IProject> projectCreator, TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => projectCreator);
            }

            m_Creator = projectCreator;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
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

            commandContext.Result = Task<IProject>.Factory.StartNew(
                m_Creator,
                new CancellationToken(),
                TaskCreationOptions.None,
                m_Scheduler);
        }

        #endregion
    }
}
