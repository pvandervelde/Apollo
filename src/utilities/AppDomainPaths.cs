//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Indicates which assembly resolution paths should be provided 
    /// for the <see cref="AppDomain.AssemblyResolve"/> event.
    /// </summary>
    [Flags]
    public enum AppDomainPaths
    {
        /// <summary>
        /// No special paths needs to be available for the resolve event.
        /// </summary>
        None = 0,

        /// <summary>
        /// The paths for the core need to be available for the resolve event.
        /// </summary>
        Core = 1,

        /// <summary>
        /// The paths for the plugins need to be available for the resolve event.
        /// </summary>
        Plugins = 2,
    }
}
