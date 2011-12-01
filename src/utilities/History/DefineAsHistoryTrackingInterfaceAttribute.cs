//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines an attribute that is used to mark a specific interface as the client side interface for 
    /// types that implement history tracking of values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    [ExcludeFromCodeCoverage]
    internal sealed class DefineAsHistoryTrackingInterfaceAttribute : Attribute
    {
    }
}
