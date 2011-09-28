//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines the interface for objects that hold notification arguments.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "Using this as a marker interface because we need to have a type for notification arguments.")]
    public interface INotificationArguments
    {
    }
}
