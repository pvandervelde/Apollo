//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Utilities.Commands;
using Lokad;
using ICommand = Apollo.Utilities.Commands.ICommand;

namespace Apollo.Core.Host
{
    /// <summary>
    /// A command used to request a shut down from the application.
    /// </summary>
    internal sealed class ShutdownApplicationCommand : ICommand
    {
        /// <summary>
        /// Defines the Id for the <c>CheckApplicationCanShutdownCommand</c>.
        /// </summary>
        public static readonly CommandId CommandId = new CommandId(@"ShutdownApplication");

        /// <summary>
        /// The action that performs the shutdown.
        /// </summary>
        private Action m_Action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownApplicationCommand"/> class.
        /// </summary>
        /// <param name="shutdownAction">The <see cref="Action"/> that performs the shutdown.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="shutdownAction"/> is <see langword="null"/>.
        /// </exception>
        internal ShutdownApplicationCommand(Action shutdownAction)
        {
            {
                Enforce.Argument(() => shutdownAction);
            }

            m_Action = shutdownAction;
        }

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

            m_Action();
        }
    }
}
