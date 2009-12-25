﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Messaging;
using Autofac;
using Autofac.Builder;
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
        internal sealed class KernelInjector : MarshalByRefObject, IInjectKernels
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
                    builder.Register(c => new MessageProcessingAssistance())
                        .As<IHelpMessageProcessing>();
                }

                return builder.Build();
            }

            /// <summary>
            /// The kernel object.
            /// </summary>
            private Kernel m_Kernel;

            /// <summary>
            /// Creates the kernel.
            /// </summary>
            public void CreateKernel()
            {
                var container = BuildContainer();
                m_Kernel = new Kernel(container.Resolve<IHelpMessageProcessing>());
            }

            /// <summary>
            /// Installs the specified service in the kernel.
            /// </summary>
            /// <param name="serviceToInstall">
            ///     The service which should be installed.
            /// </param>
            public void InstallService(KernelService serviceToInstall)
            {
                {
                    Enforce.Argument(() => serviceToInstall);
                }

                m_Kernel.Install(serviceToInstall);
            }

            /// <summary>
            /// Uninstalls the service.
            /// </summary>
            /// <param name="serviceToUninstall">The service which should be to uninstalled.</param>
            public void UninstallService(KernelService serviceToUninstall)
            {
                {
                    Enforce.Argument(() => serviceToUninstall);
                }

                m_Kernel.Uninstall(serviceToUninstall);
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
