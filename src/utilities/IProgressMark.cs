//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for classes that define progress points in an action taken by the application.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "For now we don't need any methods on this interface, but we may do later.")]
    public interface IProgressMark
    {
        // Process ID?
        // Time
        // Description
        // Started / Finished
    }
}
