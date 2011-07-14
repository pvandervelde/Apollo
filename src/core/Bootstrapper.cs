//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Apollo.Core.Projects;
using Apollo.Core.UserInterfaces;
using Apollo.Utilities;
using Apollo.Utilities.Commands;
using Autofac;
using Autofac.Core;
using Lokad;

namespace Apollo.Core
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
    [ExcludeFromCodeCoverage]
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

                foreach (var module in additionalModules)
                {
                    builder.RegisterModule(module);
                }
            }

            var container = builder.Build();
            if (container.IsRegistered<Autofac.IStartable>())
            {
                var startable = container.Resolve<Autofac.IStartable>();
                startable.Start();
            }

            return container;
        }

        /// <summary>
        /// Creates the kernel.
        /// </summary>
        /// <param name="shutdownAction">The action that should be executed just before shutdown.</param>
        /// <returns>
        /// The newly created kernel.
        /// </returns>
        private static IKernel CreateKernel(Action shutdownAction)
        {
            return new Kernel(shutdownAction);
        }

        /// <summary>
        /// The collection that contains the base and private path information
        /// for the <c>AppDomain</c>s that need to be created.
        /// </summary>
        private readonly KernelStartInfo m_StartInfo;

        /// <summary>
        /// Tracks the progress of the bootstrapping process.
        /// </summary>
        private readonly ITrackProgress m_Progress;

        /// <summary>
        /// The event that is used to signal the application that it is safe to shut down.
        /// </summary>
        private readonly AutoResetEvent m_ShutdownEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        /// <param name="startInfo">The collection of <c>AppDomain</c> base and private paths.</param>
        /// <param name="progress">The object used to track the progress of the bootstrapping process.</param>
        /// <param name="shutdownEvent">The event that signals to the application that it is safe to shut down.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="startInfo"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="progress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="shutdownEvent"/> is <see langword="null" />.
        /// </exception>
        protected Bootstrapper(
            KernelStartInfo startInfo, 
            ITrackProgress progress,
            AutoResetEvent shutdownEvent)
        {
            {
                Enforce.Argument(() => startInfo);
                Enforce.Argument(() => progress);
                Enforce.Argument(() => shutdownEvent);
            }

            m_StartInfo = startInfo;
            m_Progress = progress;
            m_ShutdownEvent = shutdownEvent;
        }

        /// <summary>
        /// Loads the Apollo system and starts the kernel.
        /// </summary>
        public void Load()
        {
            // Mark beginning of startup.
            {
                m_Progress.Mark(new ApplicationStartingProgressMark());
                m_Progress.StartTracking();
            }

            // Load up the IOC container
            var container = CreateIocContainer(AdditionalCoreModules());

            // Mark progress from UI to core
            {
                m_Progress.Mark(new CoreLoadingProgressMark());
            }

            Action shutdownAction = () =>
                {
                    // get rid of all the objects (e.g. communication etc.)
                    container.Dispose();

                    // indicate that we're done with the shutdown
                    m_ShutdownEvent.Set();
                };
            var kernel = CreateKernel(shutdownAction);

            var serviceTypes = FindServiceTypes();
            foreach (var serviceType in serviceTypes)
            {
                CreateService(serviceType, kernel, container);
            }

            // Load the UI service
            var userInterfaceService = CreateUserInterfaceService(container);
            kernel.Install(userInterfaceService);

            // Mark progress to starting core
            {
                m_Progress.Mark(new CoreStartingProgressMark());
            }

            kernel.Start();

            // Indicate core startup is done
            {
                m_Progress.Mark(new ApplicationStartupFinishedProgressMark());
                m_Progress.StopTracking();
            }
        }

        /// <summary>
        /// Creates the given service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="container">The IOC container that contains all the references.</param>
        private void CreateService(Type serviceType, IKernel kernel, IContainer container)
        {
            // Mark progress for service 'serviceType'
            {
                var markers = serviceType.GetCustomAttributes(typeof(ProgressMarkerTypeAttribute), false);
                if (markers.Length == 1)
                {
                    var marker = Activator.CreateInstance(((ProgressMarkerTypeAttribute)markers[0]).MarkerType) as IProgressMark;
                    m_Progress.Mark(marker);
                }
            }

            var service = container.Resolve(serviceType) as KernelService;
            kernel.Install(service);
        }

        /// <summary>
        /// Creates the user interface service.
        /// </summary>
        /// <param name="container">The IOC container that contains all the references.</param>
        /// <returns>
        /// The newly created user interface service.
        /// </returns>
        private KernelService CreateUserInterfaceService(IContainer container)
        {
            var userInterface = new UserInterfaceService(
                container.Resolve<ICommandContainer>(),
                container.Resolve<INotificationNameConstants>(),
                container.Resolve<Action<LogSeverityProxy, string>>(),
                StoreContainer);

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
        /// the User Interface.
        /// </summary>
        /// <param name="container">
        ///     The IOC module that contains the references for the User Interface.
        /// </param>
        protected abstract void StoreContainer(IModule container);
    }
}
