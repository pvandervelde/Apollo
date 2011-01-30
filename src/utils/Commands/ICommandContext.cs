//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utils.Commands
{
    /// <summary>
    /// Defines the interface for objects that defines a 'context' for
    /// <see cref="ICommand"/> invokations.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "For now we don't need any methods on this interface, but we may do later.")]
    public interface ICommandContext
    {
    }
}
