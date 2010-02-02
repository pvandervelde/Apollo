//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utils.Commands
{
    /// <summary>
    /// Defines the base interface for objects that hold <see cref="ICommand"/> objects
    /// by Id.
    /// </summary>
    public interface IHaveCommands
    {
        /// <summary>
        /// Determines whether a command with the specified Id is stored.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <returns>
        ///     <see langword="true"/> if a command with the specified ID is stored; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool Contains(CommandId id);
    }
}
