//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Security;
using System.Security.Permissions;
using Apollo.Core.Messaging;
using Apollo.Core.Utils;
using Apollo.Core.Utils.Licensing;
using Apollo.Utils;
using Apollo.Utils.Commands;
using Apollo.Utils.Licensing;
using Autofac;
using AutofacContrib.Startable;
using Lokad;

namespace Apollo.Core
{
    /// <content>
    /// Contains the definition of the <see cref="KernelInjector"/> class.
    /// </content>
    public abstract partial class Bootstrapper
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
        [ExcludeFromCoverage("This class is used to handle startup for Apollo. Integration testing is more suitable.")]
        private sealed class KernelInjector : MarshalByRefObject, IInjectKernels
        {
            /// <summary>
            /// Builds the IOC container.
            /// </summary>
            /// <returns>
            /// The DI container that is used to create the kernel.
            /// </returns>
            private static IContainer BuildContainer()
            {
                var builder = new ContainerBuilder();
                {
                    builder.RegisterModule(new UtilsModule());
                    builder.RegisterModule(new KernelModule());
                    builder.RegisterModule(new MessagingModule());
                    builder.RegisterModule(new LicensingModule());

                    // Register the cache channel. Should live in the kernel appdomain because:
                    // - It has to live somewhere and the kernel appdomain is the least likely to die while the app survies
                    //   (if the kernel domain dies then everything pretty much falls over)
                    // - The kernel domain has pretty tight security so we can easily control what gets loaded and what doesn't
                    //
                    // Register the channel separately because that doesn't get
                    // registered with the other licensing components to prevent
                    // the creation of multiple components.
                    builder.Register(c => new CacheConnectorChannel())
                        .As<ICacheConnectorChannel>()
                        .SingleInstance();
                }

                return builder.Build();
            }

            /// <summary>
            /// The kernel object.
            /// </summary>
            private Kernel m_Kernel;

            /// <summary>
            /// The channel that is used to connect the license validation caches.
            /// </summary>
            private ICacheConnectorChannel m_CacheChannel;

            /// <summary>
            /// Gets the channel that is used to connect the <see cref="ILicenseValidationCache"/> objects.
            /// </summary>
            public ICacheConnectorChannel CacheConnectionChannel
            {
                get
                {
                    return m_CacheChannel;
                }
            }

            /// <summary>
            /// Creates the kernel.
            /// </summary>
            public void CreateKernel()
            {
                var container = BuildContainer();
                if (container.IsRegistered<IStarter>())
                {
                    SecurityHelpers.Elevate(
                        new PermissionSet(PermissionState.Unrestricted), 
                        () => 
                            {
                                var startable = container.Resolve<IStarter>();
                                startable.Start();
                            });
                }

                m_Kernel = new Kernel(
                    container.Resolve<ICommandContainer>(), 
                    container.Resolve<IHelpMessageProcessing>(), 
                    container.Resolve<IDnsNameConstants>());
                m_CacheChannel = container.Resolve<ICacheConnectorChannel>();
            }

            /// <summary>
            /// Installs the specified service in the kernel.
            /// </summary>
            /// <param name="serviceToInstall">The service which should be installed.</param>
            /// <param name="serviceDomain">The <see cref="AppDomain"/> in which the service resides.</param>
            public void InstallService(KernelService serviceToInstall, AppDomain serviceDomain)
            {
                {
                    Enforce.Argument(() => serviceToInstall);
                }

                m_Kernel.Install(serviceToInstall, serviceDomain);
            }

            /// <summary>
            /// Starts the kernel.
            /// </summary>
            /// <design>
            /// The <see cref="Kernel.Start"/> method will not be called on a
            /// separate thread because the <c>Kernel.Start</c> method 
            /// creates its own threads which power the kernel.
            /// </design>
            public void Start()
            {
                m_Kernel.Start();
            }
        }
    }
}
