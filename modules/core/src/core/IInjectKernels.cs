//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

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
        /// Prepares the <c>AppDomain</c> for use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Preparation of an <c>AppDomain</c> means that the assembly resolution
        /// methods and the top level exception handlers are attached to the
        /// correct events on the <c>AppDomain</c>.
        /// </para>
        /// </remarks>
        /// <param name="assemblyDirectories">
        ///     The collection of files that might 
        ///     need to be loaded into the <c>AppDomain</c>.
        /// </param>
        /// <param name="exceptionHandler">
        ///     The exception handler which forms the last defence against errors
        ///     in the application.
        /// </param>
        void PrepareAppDomain(IEnumerable<FileInfo> assemblyDirectories,
            IExceptionHandler exceptionHandler);

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
    }
}
