//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Apollo.Core
{
    /// <content>
    /// Contains the definition of the <see cref="ServiceInjector"/> class.
    /// </content>
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
            /// <param name="typeToLoad">The type of the kernel service which must be created.</param>
            /// <param name="unloadAction">The action which should be invoked when the service <c>AppDomain</c> is unloaded.</param>
            /// <returns>A proxy to the kernel service.</returns>
            public KernelService CreateService(Type typeToLoad, Action<KernelService> unloadAction)
            {
                Debug.Assert(typeof(KernelService).IsAssignableFrom(typeToLoad), "The service type does not derive from KernelService.");
                var service = Activator.CreateInstance(typeToLoad) as KernelService;
                {
                    // Setup the appdomain so that when it unloads, it uninstalls the service
                    AppDomain.CurrentDomain.DomainUnload += (s, e) => unloadAction(service);
                }

                return service;
            }
        }
    }
}
