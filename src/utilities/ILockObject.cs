//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects on which synchronization locks are taken.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "For now we don't need any methods on this interface, but we may do later.")]
    public interface ILockObject
    {
        // Does this need an identifier
        // Does this need a description? --> Explaining why the lock
        // Does this need a 'target'? --> Explaining what gets locked?
    }
}
