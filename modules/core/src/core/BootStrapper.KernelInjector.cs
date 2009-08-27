﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace Apollo.Core
{
    public abstract partial class BootStrapper
    {
        /// <summary>
        /// A class used to inject kernel objects into a specially
        /// created <c>AppDomain</c>.
        /// </summary>
        /// <design>
        ///     The <c>Injector</c> assumes that the objects which are
        ///     loaded all come from the assembly from which the <c>Bootstrapper</c>
        ///     class comes. This means that it is safe to work with types and not
        ///     strings.
        /// </design>
        internal sealed class KernelInjector : MarshalByRefObject, IInjectKernels
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
            public void PrepareAppDomain(IEnumerable<FileInfo> assemblyDirectories,
                IExceptionHandler exceptionHandler)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Creates the kernel.
            /// </summary>
            public void CreateKernel()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Installs the specified service in the kernel.
            /// </summary>
            /// <param name="serviceToInstall">
            ///     The service which should be installed.
            /// </param>
            public void InstallService(KernelService serviceToInstall)
            {
                throw new NotImplementedException();
            }
        }
    }
}