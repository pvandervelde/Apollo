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
using Apollo.Utilities;
using Apollo.Utilities.Commands;
using Lokad;
using ICommand = Apollo.Utilities.Commands.ICommand;

namespace Apollo.Core.UserInterfaces.Projects
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
        public static readonly CommandId CommandId = new CommandId(@"LoadProject");

        #endregion

        /// <summary>
        /// The function that is used to load the project.
        /// </summary>
        private readonly Func<IPersistenceInformation, IProject> m_Loader;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadProjectCommand"/> class.
        /// </summary>
        /// <param name="loader">The function that is used to load the project.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="loader"/> is <see langword="null"/>.
        /// </exception>
        internal LoadProjectCommand(Func<IPersistenceInformation, IProject> loader, TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => loader);
            }

            m_Loader = loader;
            m_Scheduler = scheduler;
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

            commandContext.Result = Task<IProject>.Factory.StartNew(
                () => m_Loader(commandContext.LoadFrom),
                new CancellationToken(),
                TaskCreationOptions.LongRunning,
                m_Scheduler);
        }

        #endregion
    }
}
