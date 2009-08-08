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
        ///     The collection of directories that contain assemblies that might 
        ///     need to be loaded into the <c>AppDomain</c>.
        /// </param>
        /// <param name="exceptionHandler">
        ///     The exception handler which forms the last defence against errors
        ///     in the application.
        /// </param>
        void PrepareAppDomain(IEnumerable<DirectoryInfo> assemblyDirectories, 
            IExceptionHandler exceptionHandler);

        /// <summary>
        /// Creates the kernel service and returns a proxy to the service.
        /// </summary>
        /// <param name="typeToLoad">
        ///     The type of the kernel service which must be created.
        /// </param>
        /// <returns>
        ///     A proxy to the kernel service.
        /// </returns>
        KernelService CreateService(Type typeToLoad);
    }
}
