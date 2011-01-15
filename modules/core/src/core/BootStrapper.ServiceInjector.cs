//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.Projects;
using Apollo.Core.Utils;
using Apollo.Core.Utils.Licensing;
using Apollo.Utils;
using Apollo.Utils.Licensing;
using Autofac;
using Autofac.Core;
using AutofacContrib.Startable;

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
        [ExcludeFromCoverage("This class is used to handle startup for Apollo. Integration testing is more suitable.")]
        private sealed class ServiceInjector : MarshalByRefObject, IInjectServices
        {
            /// <summary>
            /// Builds the IOC container.
            /// </summary>
            /// <param name="channel">The channel that is used to connect the <see cref="ILicenseValidationCache"/> objects.</param>
            /// <param name="additionalModules">The collection of additional modules that should be loaded.</param>
            /// <returns>
            /// The DI container that is used to create the service.
            /// </returns>
            private static IContainer BuildContainer(ICacheConnectorChannel channel, IModule[] additionalModules)
            {
                var builder = new ContainerBuilder();
                {
                    builder.RegisterModule(new UtilsModule());
                    builder.RegisterModule(new KernelModule());
                    builder.RegisterModule(new MessagingModule());
                    builder.RegisterModule(new LoggerModule());
                    builder.RegisterModule(new ProjectModule());

                    foreach (var module in additionalModules)
                    {
                        builder.RegisterModule(module);
                    }

                    // Register the proxy to the cache channel
                    builder.Register(c => channel)
                        .As<ICacheConnectorChannel>()
                        .ExternallyOwned();
                }

                return builder.Build();
            }

            /// <summary>
            /// Creates the kernel service and returns a proxy to the service.
            /// </summary>
            /// <param name="typeToLoad">The type of the kernel service which must be created.</param>
            /// <param name="channel">The channel that is used to connect the <see cref="ILicenseValidationCache"/> objects.</param>
            /// <returns>A proxy to the kernel service.</returns>
            public KernelService CreateService(Type typeToLoad, ICacheConnectorChannel channel)
            {
                Debug.Assert(typeof(KernelService).IsAssignableFrom(typeToLoad), "The service type does not derive from KernelService.");

                // Check if we need the licensing components
                var attributes = typeToLoad.GetCustomAttributes(typeof(IncludeLicensingAttribute), false);
                IModule[] modules = (attributes.Length == 1) ? 
                    new IModule[] { new LicensingModule() } : 
                    new IModule[0];

                var container = BuildContainer(channel, modules);

                // Load up any components registered as self-starting (i.e. IStarter)
                // this can be the license components or other components that need to 
                // be loaded on system start-up.
                if (container.IsRegistered<IStarter>())
                {
                    var startable = container.Resolve<IStarter>();
                    startable.Start();
                }

                var service = container.Resolve(typeToLoad) as KernelService;
                return service;
            }
        }
    }
}
