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
        /// A class used to inject kernel services into a specially
        /// created <c>AppDomain</c>.
        /// </summary>
        /// <design>
        ///     The <c>Injector</c> assumes that the services which are
        ///     loaded all come from the assembly from which the <c>Bootstrapper</c>
        ///     class comes. This means that it is safe to work with types and not
        ///     strings.
        /// </design>
        internal sealed class ServiceInjector : MarshalByRefObject, IInjectServices
        {
            /// <summary>
            /// Creates the kernel service and returns a proxy to the service.
            /// </summary>
            /// <param name="typeToLoad">
            ///     The type of the kernel service which must be created.
            /// </param>
            /// <returns>
            ///     A proxy to the kernel service.
            /// </returns>
            public KernelService CreateService(Type typeToLoad)
            {
                throw new NotImplementedException();
            }
        }
    }
}
