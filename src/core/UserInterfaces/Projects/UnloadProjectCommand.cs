//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Utilities.Commands;
using Lokad;
using ICommand = Apollo.Utilities.Commands.ICommand;

namespace Apollo.Core.UserInterfaces.Projects
{
    /// <summary>
    /// Defines a command that unloads the current project.
    /// </summary>
    internal sealed class UnloadProjectCommand : ICommand
    {
        /// <summary>
        /// Defines the Id for the <c>UnloadProjectCommand</c>.
        /// </summary>
        public static readonly CommandId CommandId = new CommandId(@"UnloadProject");

        /// <summary>
        /// The method used to unload the project.
        /// </summary>
        private readonly Action m_UnloadMethod;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadProjectCommand"/> class.
        /// </summary>
        /// <param name="unloadMethod">The method used to unload the project.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="unloadMethod"/> is <see langword="null"/>.
        /// </exception>
        internal UnloadProjectCommand(Action unloadMethod, TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => unloadMethod);
            }

            m_UnloadMethod = unloadMethod;
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
            var commandContext = context as UnloadProjectContext;
            Debug.Assert(commandContext != null, "Incorrect command context provided.");

            commandContext.Result = Task.Factory.StartNew(
                () => m_UnloadMethod(),
                new CancellationToken(),
                TaskCreationOptions.None,
                m_Scheduler);
        }

        #endregion
    }
}
