//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Lokad;

namespace Apollo.Utils.Commands
{
    /// <summary>
    /// Defines methods used to locate and create specific commands.
    /// </summary>
    public sealed class CommandFactory : ICommandContainer
    {
        /// <summary>
        /// The collection that holds the command activators.
        /// </summary>
        private readonly Dictionary<CommandId, Func<ICommand>> m_Commands = new Dictionary<CommandId, Func<ICommand>>();

        #region Implementation of ICommandContainer

        /// <summary>
        /// Adds a new command activator to the collection.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <param name="activator">The activator which is used to create instances of the command.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="activator"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="DuplicateCommandException">
        /// Thrown if a command with <paramref name="id"/> is already stored in the collection.
        /// </exception>
        public void Add(CommandId id, Func<ICommand> activator)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => activator);
            }

            if (m_Commands.ContainsKey(id))
            {
                throw new DuplicateCommandException(id);
            }

            m_Commands.Add(id, activator);
        }

        /// <summary>
        /// Removes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownCommandException">
        /// Thrown if no command with <paramref name="id"/> can be found in the collection.
        /// </exception>
        public void Remove(CommandId id)
        {
            {
                Enforce.Argument(() => id);
            }

            if (!m_Commands.ContainsKey(id))
            {
                throw new UnknownCommandException(id);
            }

            m_Commands.Remove(id);
        }

        #endregion

        #region Implementation of IInvokeCommands

        /// <summary>
        /// Invokes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownCommandException">
        /// Thrown if no command with <paramref name="id"/> can be found in the collection.
        /// </exception>
        public void Invoke(CommandId id)
        {
            Invoke(id, new EmptyCommandContext());
        }

        /// <summary>
        /// Invokes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <param name="context">The context that will be passed to the command as it is invoked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownCommandException">
        /// Thrown if no command with <paramref name="id"/> can be found in the collection.
        /// </exception>
        public void Invoke(CommandId id, ICommandContext context)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => context);
            }

            if (!m_Commands.ContainsKey(id))
            {
                throw new UnknownCommandException(id);
            }

            var command = m_Commands[id]();
            command.Invoke(context);
        }

        #endregion

        #region Implementation of IHaveCommands

        /// <summary>
        /// Determines whether a command with the specified ID is stored.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <returns>
        ///     <see langword="true"/> if a command with the specified IIDd is stored; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Contains(CommandId id)
        {
            return id != null && m_Commands.ContainsKey(id);
        }

        #endregion
    }
}
