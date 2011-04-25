//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Utils.Commands;
using Lokad;
using ICommand = Apollo.Utils.Commands.ICommand;

namespace Apollo.Core.UserInterfaces.Projects
{
    /// <summary>
    /// Defines a command that unloads the current project.
    /// </summary>
    internal sealed class UnloadProjectCommand : ICommand
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
        /// The method used to unload the project.
        /// </summary>
        private Action m_UnloadMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadProjectCommand"/> class.
        /// </summary>
        /// <param name="unloadMethod">The method used to unload the project.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="unloadMethod"/> is <see langword="null"/>.
        /// </exception>
        internal UnloadProjectCommand(Action unloadMethod)
        {
            {
                Enforce.Argument(() => unloadMethod);
            }

            m_UnloadMethod = unloadMethod;
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

            commandContext.Result = Task.Factory.StartNew(() => m_UnloadMethod());
        }

        #endregion
    }
}
