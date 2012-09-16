//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Extensions.Plugins
{
    /// <summary>
    /// Defines the attribute that indicates that a method is an action that can be placed
    /// in a schedule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ScheduleActionAttribute : Attribute
    {
        // Can only be placed on certain methods. Will be checked in the scanner
        //
        // Provide:
        // * ID (string)
        // * Meta-data:
        //   * Name
        //   * Description
    }
}
