//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.Commands
{
    /// <summary>
    /// Defines an interface for objects that store <see cref="ICommand"/> objects by
    /// name.
    /// </summary>
    public interface ICommandContainer : IInvokeCommands, IHaveCommands
    {
        /// <summary>
        /// Adds a new command activator to the collection.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <param name="activator">The activator which is used to create instances of the command.</param>
        void Add(CommandId id, Func<ICommand> activator);

        /// <summary>
        /// Removes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        void Remove(CommandId id);
    }
}
