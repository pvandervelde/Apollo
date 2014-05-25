//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Apollo.Core.Host.Plugins;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces;
using Autofac;
using Autofac.Core;
using Lokad;
using Nuclei.Diagnostics;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Defines the base actions and methods used to start the Apollo system.
    /// </summary>
    /// <design>
    /// The bootstrapper will load the kernel objects and provide them with starting
    /// data. The loading takes place in the following order:
    /// <list type="number">
    /// <item>
    ///   Link the assembly resolver into the current AppDomain. This AppDomain will later 
    ///   be used for the User Interface (UI) because it is possible that the Apollo system
    ///   gets started as plug-in to another application, which means that there is no
    ///   control over where and how the first AppDomain is created. Furthermore the
    ///   hosting application will expect that all action takes place in the first AppDomain.
    /// </item>
    /// <item>
    ///   Create a new AppDomain in which the kernel objects will reside. When creating this
    ///   AppDomain it is also necessary to initialize the security levels, set the search
    ///   paths, attach the assembly loaders and deal with the exception handlers
    /// </item>
    /// <item>
    ///   Inject a remote object loader into the new AppDomain. This object loader is used
    ///   to create the kernel objects and initialize them.
    /// </item>
    /// <item>
    ///   Load the kernel object and start it
    /// </item>
    /// <item>
    ///   Once the kernel is up and running the bootstrapper can be discarded since it is no
    ///   longer useful.
    /// </item>
    /// </list>
    /// </design>
    public abstract class Bootstrapper
    {
        /// <summary>
        /// Finds all the service types stored in the current assembly.
        /// </summary>
        /// <returns>
        /// A collection containing all the <see cref="Type"/> objects that derive from <see cref="KernelService"/>.
        /// </returns>
        /// <todo>
        /// Perform the search for the services via a IOC container.
        /// </todo>
        private static IEnumerable<Type> FindServiceTypes()
        {
            var serviceTypes = from type in Assembly.GetExecutingAssembly().GetTypes()
                               where typeof(KernelService).IsAssignableFrom(type) &&
                                     type.GetCustomAttributes(typeof(AutoLoadAttribute), true).Length == 0 &&
                                     !type.IsAbstract
                               select type;

            return serviceTypes;
        }

        /// <summary>
        /// Builds the IOC container.
        /// </summary>
        /// <param name="additionalModules">The additional modules that need to be registered with the container.</param>
        /// <returns>
        /// The DI container that is used to create the service.
        /// </returns>
        private static IContainer CreateIocContainer(IEnumerable<IModule> additionalModules)
        {
            // Set up the IOC container for the core. This does NOT
            // contain any of the user interface stuff, meaning that no
            // code can easily call the user interface.
            // Any service that needs a link to the UI service will get
            // that link once the Kernel starts handing out references
            // to the other services.
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new UtilitiesModule());
                builder.RegisterModule(new KernelModule());
                builder.RegisterModule(new ProjectModule());
                builder.RegisterModule(new PluginsModule());

                foreach (var module in additionalModules)
                {
                    builder.RegisterModule(module);
                }
            }

            // Building the container automatically starts all the startables.
            var container = builder.Build();
            return container;
        }

        /// <summary>
        /// Creates the kernel.
        /// </summary>
        /// <param name="shutdownAction">The action that should be executed just before shutdown.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <returns>
        /// The newly created kernel.
        /// </returns>
        private static IKernel CreateKernel(Action shutdownAction, SystemDiagnostics diagnostics)
        {
            return new Kernel(shutdownAction, diagnostics);
        }

        /// <summary>
        /// Creates the given service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="container">The IOC container that contains all the references.</param>
        private static void CreateService(Type serviceType, IKernel kernel, IContainer container)
        {
            var service = container.Resolve(serviceType) as KernelService;
            kernel.Install(service);
        }

        /// <summary>
        /// The event that is used to signal the application that it is safe to shut down.
        /// </summary>
        private readonly AutoResetEvent m_ShutdownEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        /// <param name="shutdownEvent">The event that signals to the application that it is safe to shut down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="shutdownEvent"/> is <see langword="null" />.
        /// </exception>
        protected Bootstrapper(AutoResetEvent shutdownEvent)
        {
            {
                Enforce.Argument(() => shutdownEvent);
            }

            m_ShutdownEvent = shutdownEvent;
        }

        /// <summary>
        /// Loads the Apollo system and starts the kernel.
        /// </summary>
        public void Load()
        {
            // Load up the IOC container
            var container = CreateIocContainer(AdditionalCoreModules());
            var diagnostics = container.Resolve<SystemDiagnostics>();

            Action shutdownAction = () =>
                {
                    // get rid of all the objects (e.g. communication etc.)
                    container.Dispose();

                    // indicate that we're done with the shutdown
                    m_ShutdownEvent.Set();
                };
            var kernel = CreateKernel(shutdownAction, diagnostics);

            var serviceTypes = FindServiceTypes();
            foreach (var serviceType in serviceTypes)
            {
                CreateService(serviceType, kernel, container);
            }

            // Load the UI service
            var userInterfaceService = CreateUserInterfaceService(container, diagnostics);
            kernel.Install(userInterfaceService);

            kernel.Start();
        }

        /// <summary>
        /// Creates the user interface service.
        /// </summary>
        /// <param name="container">The IOC container that contains all the references.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <returns>
        /// The newly created user interface service.
        /// </returns>
        private KernelService CreateUserInterfaceService(IContainer container, SystemDiagnostics diagnostics)
        {
            var userInterface = new UserInterfaceService(container, StartUserInterface, diagnostics);
            return userInterface;
        }

        /// <summary>
        /// Returns a collection containing additional IOC modules that are
        /// required to start the core.
        /// </summary>
        /// <returns>
        ///     The collection containing additional IOC modules necessary
        ///     to start the core.
        /// </returns>
        protected abstract IEnumerable<IModule> AdditionalCoreModules();

        /// <summary>
        /// Stores the IOC module that contains the references that will be used by 
        /// the User Interface and then starts all the user interface elements.
        /// </summary>
        /// <param name="container">
        ///     The IOC container that contains the references the entire application.
        /// </param>
        protected abstract void StartUserInterface(IContainer container);
    }
}
