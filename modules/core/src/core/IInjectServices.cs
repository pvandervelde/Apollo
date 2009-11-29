//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for classes that are used to
    /// inject <c>KernelService</c> objects into an
    /// <c>AppDomain</c>.
    /// </summary>
    internal interface IInjectServices
    {
        /// <summary>
        /// Creates the kernel service and returns a proxy to the service.
        /// </summary>
        /// <param name="typeToLoad">The type of the kernel service which must be created.</param>
        /// <param name="unloadAction">The action which should be invoked when the service <c>AppDomain</c> is unloaded.</param>
        /// <returns>A proxy to the kernel service.</returns>
        KernelService CreateService(Type typeToLoad, Action<KernelService> unloadAction);
    }
}
