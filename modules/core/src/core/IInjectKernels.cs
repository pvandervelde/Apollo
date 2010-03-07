//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the interface for classes that are used to
    /// inject <c>Kernel</c> objects into an
    /// <c>AppDomain</c>.
    /// </summary>
    internal interface IInjectKernels
    {
        /// <summary>
        /// Creates the kernel.
        /// </summary>
        void CreateKernel();

        /// <summary>
        /// Installs the specified service in the kernel.
        /// </summary>
        /// <param name="serviceToInstall">The service which should be installed.</param>
        /// <param name="serviceDomain">The <see cref="AppDomain"/> in which the service resides.</param>
        void InstallService(KernelService serviceToInstall, AppDomain serviceDomain);

        /// <summary>
        /// Starts the kernel.
        /// </summary>
        void Start();
    }
}
