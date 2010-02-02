//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utils.Commands
{
    /// <summary>
    /// Defines the interface for objects that invoke <see cref="ICommand"/> objects.
    /// </summary>
    public interface IInvokeCommands : IHaveCommands
    {
        /// <summary>
        /// Invokes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        void Invoke(CommandId id);

        /// <summary>
        /// Invokes the command with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <param name="context">The context that will be passed to the command as it is invoked.</param>
        void Invoke(CommandId id, ICommandContext context);
    }
}
