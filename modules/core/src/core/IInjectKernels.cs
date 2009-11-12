//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
        /// <param name="serviceToInstall">
        ///     The service which should be installed.
        /// </param>
        void InstallService(KernelService serviceToInstall);

        /// <summary>
        /// Starts the kernel.
        /// </summary>
        void Start();
    }
}
