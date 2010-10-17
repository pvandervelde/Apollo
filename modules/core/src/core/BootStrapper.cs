//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
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
    public abstract partial class Bootstrapper
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
        /// Determines the relevant security level.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// The <see cref="SecurityLevel"/> specified by the <see cref="ServiceSecurityLevelAttribute"/> or
        /// <see cref="SecurityLevel.Minimum"/> if nothing is specified.
        /// </returns>
        private static SecurityLevel DetermineRelevantSecurityLevel(Type serviceType)
        {
            Debug.Assert(typeof(KernelService).IsAssignableFrom(serviceType), "The input type is not a derivative of KernelService.");

            // Get the marker
            var attributes = serviceType.GetCustomAttributes(typeof(ServiceSecurityLevelAttribute), false);
            if (attributes.Length == 1)
            {
                // Create the marker
                var level = ((ServiceSecurityLevelAttribute)attributes[0]).SecurityLevel;
                return level;
            }

            return SecurityLevel.Minimum;
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
            PrepareUserInterfaceAppDomain();

            // Mark progress from UI to core
            {
                m_Progress.Mark(new CoreLoadingProgressMark());
            }

            var coreBasePath = new DirectoryInfo(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath));
            var kernel = CreateKernel(coreBasePath);

            // Scan the current assembly for all exported parts.
            var serviceTypes = FindServiceTypes();

            // Create all the services and pass them to the kernel
            foreach (var serviceType in serviceTypes)
            {
                CreateService(serviceType, coreBasePath, kernel);
            }

            // Load the UI service
            var userInterfaceService = CreateUserInterfaceService(kernel.CacheConnectionChannel);
            kernel.InstallService(userInterfaceService, AppDomain.CurrentDomain);

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
        /// Prepares the user interface app domain for use.
        /// </summary>
        private void PrepareUserInterfaceAppDomain()
        {
            // Grab the current AppDomain
            var currentDomain = AppDomain.CurrentDomain;

            // Set the assembly resolver. Because we are inside the
            // UI appdomain it doesn't matter how we set this up.
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
        /// Creates the kernel AppDomain and injects the appropriate kernel objects..
        /// </summary>
        /// <param name="coreBasePath">The core base path.</param>
        /// <returns>
        /// The proxy to the injector.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "The coreBasePath cannot be a file, it must be a directory.")]
        private IInjectKernels CreateKernel(DirectoryInfo coreBasePath)
        {
            // Create the kernel appdomain. 
            var kernelDomain = AppDomainBuilder.Assemble(
                "Kernel AppDomain",
                new AppDomainSandboxData(SecurityLevel.Kernel, m_StartInfo.FullTrustAssemblies), 
                AppDomainResolutionPaths.WithFiles(
                    coreBasePath.FullName,
                    new List<string>(from file in m_StartInfo.CoreAssemblies select file.FullName)),
                m_ExceptionHandlerFactory(),
                new FileConstants(new ApplicationConstants()));

            Debug.Assert(!string.IsNullOrEmpty(typeof(KernelInjector).Assembly.FullName), "The assembly name does not exist. Cannot create a type from this assembly.");
            var kernelInjector = Activator.CreateInstanceFrom(
                    kernelDomain,
                    typeof(KernelInjector).Assembly.LocalFilePath(),
                    typeof(KernelInjector).FullName)
                .Unwrap() as KernelInjector;

            // And then create the kernel
            Debug.Assert(kernelInjector != null, "The kernel injector is null, this means we couldn't find the proper assembly or type.");
            kernelInjector.CreateKernel();
            return kernelInjector;
        }

        /// <summary>
        /// Creates the AppDomain for the given service type and then injects the specific service into 
        /// that AppDomain.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="coreBasePath">The core base path.</param>
        /// <param name="kernel">The kernel.</param>
        /// <todo>
        /// We're currently passing a function that returns the IEnumerable. Maybe we should push that idea
        /// all the way up?
        /// </todo>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "The coreBasePath cannot be a file, it must be a directory.")]
        private void CreateService(Type serviceType, DirectoryInfo coreBasePath, IInjectKernels kernel)
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

            var securityLevel = DetermineRelevantSecurityLevel(serviceType);

            List<string> filePaths;
            List<string> directoryPaths;
            SelectPaths(serviceType, out filePaths, out directoryPaths);

            var serviceDomain = AppDomainBuilder.Assemble(
                string.Format(CultureInfo.InvariantCulture, "AppDomain for {0}", serviceType.Name),
                new AppDomainSandboxData(securityLevel, m_StartInfo.FullTrustAssemblies), 
                AppDomainResolutionPaths.WithFilesAndDirectories(
                    coreBasePath.FullName, 
                    filePaths, 
                    directoryPaths),
                m_ExceptionHandlerFactory(),
                new FileConstants(new ApplicationConstants()));

            Debug.Assert(!string.IsNullOrEmpty(typeof(ServiceInjector).Assembly.FullName), "The assembly name does not exist. Cannot create a type from this assembly.");
            var injector = Activator.CreateInstanceFrom(
                    serviceDomain,
                    typeof(ServiceInjector).Assembly.LocalFilePath(), 
                    typeof(ServiceInjector).FullName)
                .Unwrap() as IInjectServices;

            // Prepare the appdomain
            Debug.Assert(injector != null, "Could not load the ServiceInjector.");
            var service = injector.CreateService(serviceType, kernel.CacheConnectionChannel);
            kernel.InstallService(service, serviceDomain);
        }

        /// <summary>
        /// Selects the assembly resolution paths necessary for the given service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="files">The files.</param>
        /// <param name="directories">The directories.</param>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", 
            Justification = "It seems overkill to define a custom type to encapsulate the return values.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "The use of the Type class indicates better what we want to achieve.")]
        private void SelectPaths(Type serviceType, out List<string> files, out List<string> directories)
        {
            // Determine which log paths should be applied. 
            // Note that the plugins path requires a directory resolver, the others
            // require file resolvers!
            var options = from option in serviceType.GetCustomAttributes(typeof(PrivateBinPathRequirementsAttribute), true)
                          select ((PrivateBinPathRequirementsAttribute)option).Option;

            IEnumerable<FileInfo> filePaths = null;
            IEnumerable<DirectoryInfo> directoryPaths = null;
            foreach (var option in options)
            {
                switch (option)
                {
                    case PrivateBinPathOption.Core:
                        filePaths = ConcatSequences(filePaths, m_StartInfo.CoreAssemblies);
                        break;
                    case PrivateBinPathOption.Log:
                        filePaths = ConcatSequences(filePaths, m_StartInfo.LogAssemblies);
                        break;
                    case PrivateBinPathOption.Persistence:
                        filePaths = ConcatSequences(filePaths, m_StartInfo.PersistenceAssemblies);
                        break;
                    case PrivateBinPathOption.Project:
                        filePaths = ConcatSequences(filePaths, m_StartInfo.ProjectAssemblies);
                        break;
                    case PrivateBinPathOption.UserInterface:
                        filePaths = ConcatSequences(filePaths, m_StartInfo.UserInterfaceAssemblies);
                        break;
                    case PrivateBinPathOption.PlugIns:
                        directoryPaths = m_StartInfo.PlugInDirectories;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            files = null;
            if (filePaths != null)
            {
                files = new List<string>(from file in filePaths select file.FullName);
            }

            directories = null;
            if (directoryPaths != null)
            {
                // This will only iterate over the directories in the directoryPaths collection, not all the sub-directories
                // symbolic links etc. etc.
                directories = new List<string>(from directory in directoryPaths select directory.FullName);
            }
        }

        /// <summary>
        /// Creates the user interface service.
        /// </summary>
        /// <param name="channel">The channel that is used by the licensing system to pass information between the service AppDomains.</param>
        /// <returns>
        /// The newly created user interface service.
        /// </returns>
        private KernelService CreateUserInterfaceService(ICacheConnectorChannel channel)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new UtilsModule());
                builder.RegisterModule(new KernelModule());
                builder.RegisterModule(new MessagingModule());
                builder.RegisterModule(new LoggerModule());
                builder.RegisterModule(new LicensingModule());

                // Register the proxy to the cache channel
                builder.Register(c => channel)
                    .As<ICacheConnectorChannel>()
                    .ExternallyOwned();
            }

            var container = builder.Build();
            if (container.IsRegistered<IStarter>())
            {
                container.Resolve<IStarter>().Start();
            }

            var userInterface = new UserInterfaceService(
                container.Resolve<ICommandContainer>(),
                container.Resolve<IDnsNameConstants>(), 
                container.Resolve<INotificationNameConstants>(), 
                container.Resolve<IHelpMessageProcessing>(),
                container.Resolve<IValidationResultStorage>(),
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
