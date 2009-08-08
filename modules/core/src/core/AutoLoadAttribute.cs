//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Defines an attribute that is placed on <c>KernelService</c> classes to indicate
    /// that the specific service type is automatically loaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class AutoLoadAttribute : Attribute
    { }
}
