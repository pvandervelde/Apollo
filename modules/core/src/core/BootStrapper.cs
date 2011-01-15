//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.Projects;
using Apollo.Core.UserInterfaces;
using Apollo.Core.Utils;
using Apollo.Core.Utils.Licensing;
using Apollo.Utils;
using Apollo.Utils.Commands;
using Apollo.Utils.ExceptionHandling;
using Apollo.Utils.Fusion;
using Autofac;
using Autofac.Core;
using AutofacContrib.Startable;
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
    [ExcludeFromCoverage("This class is used to handle startup for Apollo. Integration testing is more suitable.")]
    public abstract class Bootstrapper
    {
        /// <summary>
        /// The DI container.
        /// </summary>
        private static readonly IContainer s_Container = InitializeContainer();

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
        /// Concatenates two IEnumerable sequences.
        /// </summary>
        /// <typeparam name="T">The object type in the sequence.</typeparam>
        /// <param name="existingSequence">The existing sequence.</param>
        /// <param name="newSequence">The new sequence.</param>
        /// <returns>
        /// The concatenation of the two sequences.
        /// </returns>
        private static IEnumerable<T> ConcatSequences<T>(IEnumerable<T> existingSequence, IEnumerable<T> newSequence)
        {
            return (existingSequence != null) ? existingSequence.Union(newSequence) : newSequence;
        }

        /// <summary>
        /// Builds the IOC container.
        /// </summary>
        /// <param name="additionalModules">The collection of additional modules that should be loaded.</param>
        /// <returns>
        /// The DI container that is used to create the service.
        /// </returns>
        private static IContainer InitializeContainer(params IModule[] additionalModules)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new UtilsModule());
                builder.RegisterModule(new KernelModule());
                builder.RegisterModule(new MessagingModule());
                builder.RegisterModule(new LoggerModule());
                builder.RegisterModule(new ProjectModule());
                builder.RegisterModule(new LicensingModule());
            }

            var container = builder.Build();
            if (container.IsRegistered<IStarter>())
            {
                var startable = container.Resolve<IStarter>();
                startable.Start();
            }

            return container;
        }

        /// <summary>
        /// The collection that contains the base and private path information
        /// for the <c>AppDomain</c>s that need to be created.
        /// </summary>
        private readonly KernelStartInfo m_StartInfo;

        /// <summary>
        /// The factory used to create <c>IExceptionHandler</c> objects.
        /// </summary>
        private readonly Func<IExceptionHandler> m_ExceptionHandlerFactory;

        /// <summary>
        /// Tracks the progress of the bootstrapping process.
        /// </summary>
        private readonly ITrackProgress m_Progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        /// <param name="startInfo">The collection of <c>AppDomain</c> base and private paths.</param>
        /// <param name="exceptionHandlerFactory">The factory used for the creation of <see cref="IExceptionHandler"/> objects.</param>
        /// <param name="progress">The object used to track the progress of the bootstrapping process.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="startInfo"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="exceptionHandlerFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="progress"/> is <see langword="null"/>.
        /// </exception>
        protected Bootstrapper(
            KernelStartInfo startInfo, 
            Func<IExceptionHandler> exceptionHandlerFactory, 
            ITrackProgress progress)
        {
            {
                Enforce.Argument(() => startInfo);
                Enforce.Argument(() => exceptionHandlerFactory);
                Enforce.Argument(() => progress);
            }

            m_StartInfo = startInfo;
            m_ExceptionHandlerFactory = exceptionHandlerFactory;
            m_Progress = progress;
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

            // Link assembly resolver to current AppDomain
            // Link exception handlers to current AppDomain
            PrepareAppDomain();

            // Mark progress from UI to core
            {
                m_Progress.Mark(new CoreLoadingProgressMark());
            }

            var kernel = CreateKernel();

            // Scan the current assembly for all exported parts.
            var serviceTypes = FindServiceTypes();

            // Create all the services and pass them to the kernel
            foreach (var serviceType in serviceTypes)
            {
                CreateService(serviceType, kernel);
            }

            // Load the UI service
            var userInterfaceService = CreateUserInterfaceService();
            kernel.Install(userInterfaceService);

            // Mark progress to starting core
            {
                m_Progress.Mark(new CoreStartingProgressMark());
            }

            // Finally start the kernel and wait for it to finish starting
            kernel.Start();

            // Indicate core startup is done
            {
                m_Progress.Mark(new ApplicationStartupFinishedProgressMark());
                m_Progress.StopTracking();
            }
        }

        /// <summary>
        /// Prepares the <c>AppDomain</c> for use.
        /// </summary>
        private void PrepareAppDomain()
        {
            // Grab the current AppDomain
            var currentDomain = AppDomain.CurrentDomain;

            // Set the assembly resolver.
            var fusionHelper = new FusionHelper(
                () =>
                    {
                        // Concatenate the two sequences and then turn the FileInfo objects into the
                        // string representation of the file fullname
                        var totalSequence = m_StartInfo.CoreAssemblies.Concat(m_StartInfo.UserInterfaceAssemblies);
                        return from file in totalSequence select file.FullName;
                    },
                new FileConstants(new ApplicationConstants()));
            currentDomain.AssemblyResolve += fusionHelper.LocateAssemblyOnAssemblyLoadFailure;

            // Set the exception handler. Adding the event ensures that
            // the exceptionHandler cannot be collected until the AppDomain
            // is killed.
            currentDomain.UnhandledException += m_ExceptionHandlerFactory().OnUnhandledException;
        }

        /// <summary>
        /// Creates the kernel.
        /// </summary>
        /// <returns>
        /// The newly created kernel.
        /// </returns>
        private IKernel CreateKernel()
        {
            var kernel = new Kernel(
                s_Container.Resolve<ICommandContainer>(),
                s_Container.Resolve<IHelpMessageProcessing>(),
                s_Container.Resolve<IDnsNameConstants>());

            return kernel;
        }

        /// <summary>
        /// Creates the given service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="kernel">The kernel.</param>
        private void CreateService(Type serviceType, IKernel kernel)
        {
            // Mark progress for service 'serviceType'
            {
                // Get the marker
                var markers = serviceType.GetCustomAttributes(typeof(ProgressMarkerTypeAttribute), false);
                if (markers.Length == 1)
                {
                    // Create the marker
                    var marker = Activator.CreateInstance(((ProgressMarkerTypeAttribute)markers[0]).MarkerType) as IProgressMark;
                    m_Progress.Mark(marker);
                }
            }

            var service = s_Container.Resolve(serviceType) as KernelService;
            kernel.Install(service);
        }

        /// <summary>
        /// Creates the user interface service.
        /// </summary>
        /// <returns>
        /// The newly created user interface service.
        /// </returns>
        private KernelService CreateUserInterfaceService()
        {
            var userInterface = new UserInterfaceService(
                s_Container.Resolve<ICommandContainer>(),
                s_Container.Resolve<IDnsNameConstants>(),
                s_Container.Resolve<INotificationNameConstants>(),
                s_Container.Resolve<IHelpMessageProcessing>(),
                s_Container.Resolve<IValidationResultStorage>(),
                StoreContainer);

            return userInterface;
        }

        /// <summary>
        /// Stores the dependency injection container.
        /// </summary>
        /// <param name="container">The DI container.</param>
        protected abstract void StoreContainer(IModule container);
    }
}
