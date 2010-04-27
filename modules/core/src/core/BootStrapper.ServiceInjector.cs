//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Logging;
using Apollo.Core.Utils;
using Autofac;

namespace Apollo.Core
{
    /// <content>
    /// Contains the definition of the <see cref="ServiceInjector"/> class.
    /// </content>
    public abstract partial class Bootstrapper
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
        private sealed class ServiceInjector : MarshalByRefObject, IInjectServices
        {
            /// <summary>
            /// Builds the IOC container.
            /// </summary>
            /// <param name="serviceType">Type of the service.</param>
            /// <returns>
            /// The DI container that is used to create the service.
            /// </returns>
            private static IContainer BuildContainer(Type serviceType)
            {
                var builder = new ContainerBuilder();
                {
                    builder.RegisterModule(new UtilsModule());
                    builder.RegisterModule(new KernelModule());
                    builder.RegisterModule(new LoggerModule());

                    builder.RegisterType(serviceType);
                }

                return builder.Build();
            }

            /// <summary>
            /// Creates the kernel service and returns a proxy to the service.
            /// </summary>
            /// <param name="typeToLoad">The type of the kernel service which must be created.</param>
            /// <returns>A proxy to the kernel service.</returns>
            public KernelService CreateService(Type typeToLoad)
            {
                Debug.Assert(typeof(KernelService).IsAssignableFrom(typeToLoad), "The service type does not derive from KernelService.");

                var container = BuildContainer(typeToLoad);
                var service = container.Resolve(typeToLoad) as KernelService;

                return service;
            }
        }
    }
}
