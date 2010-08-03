//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// Defines an attribute that is placed on <c>KernelService</c> classes to indicate
    /// that the specific service type requires the licensing components to be loaded.
    /// </summary>
    [ExcludeFromCoverage("Attributes do not need to be tested")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal sealed class IncludeLicensingAttribute : Attribute
    {
    }
}
