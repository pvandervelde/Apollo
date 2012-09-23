//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Extensions.Plugins
{
    /// <summary>
    /// Defines the attribute that indicates that a method or property is a condition that can be placed
    /// in a schedule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [ExcludeFromCodeCoverage]
    public sealed class ScheduleConditionAttribute : Attribute
    {
        // Can only be placed on certain methods / properties. Will be checked in the scanner
        //
        // Provide:
        // * ID (string)
        // * Meta-data:
        //   * Name
        //   * Description
    }
}
