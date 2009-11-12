//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

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

            /// <summary>
            /// Starts the kernel.
            /// </summary>
            public void Start()
            {
                throw new NotImplementedException();
            }
        }
    }
}
