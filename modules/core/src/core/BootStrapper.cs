﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Apollo.Utils;
using Apollo.Utils.ExceptionHandling;
using Apollo.Utils.Fusion;
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
    /// <serviceType>
    ///   Link the assembly resolver into the current AppDomain. This AppDomain will later 
    ///   be used for the User Interface (UI) because it is possible that the Apollo system
    ///   gets started as plug-in to another application, which means that there is no
    ///   control over where and how the first AppDomain is created. Furthermore the
    ///   hosting application will expect that all action takes place in the first AppDomain.
    /// </serviceType>
    /// <serviceType>
    ///   Create a new AppDomain in which the kernel objects will reside. When creating this
    ///   AppDomain it is also necessary to initialize the security levels, set the search
    ///   paths, attach the assembly loaders and deal with the exception handlers
    /// </serviceType>
    /// <serviceType>
    ///   Inject a remote object loader into the new AppDomain. This object loader is used
    ///   to create the kernel objects and initialize them.
    /// </serviceType>
    /// <serviceType>
    ///   Load the kernel object and start it
    /// </serviceType>
    /// <serviceType>
    ///   Once the kernel is up and running the bootstrapper can be discarded since it is no
    ///   longer useful.
    /// </serviceType>
    /// </list>
    /// </design>
    public abstract partial class BootStrapper : IBootstrapper
    {
        /// <summary>
        /// Gets all the service types stored in the current assembly.
        /// </summary>
        /// <returns>
        /// A collection containing all the <see cref="Type"/> objects that derive from <see cref="KernelService"/>.
        /// </returns>
        /// <todo>
        /// Perform the search for the services via a IOC container.
        /// </todo>
        private static IEnumerable<Type> GetServiceTypes()
        {
            var serviceTypes = from type in Assembly.GetExecutingAssembly().GetTypes()
                               where typeof(KernelService).IsAssignableFrom(type) &&
                                     type.GetCustomAttributes(typeof(AutoLoadAttribute), true).Length == 0
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
            return (existingSequence != null) ? existingSequence.Concat(newSequence) : newSequence;
        }

        /// <summary>
        /// The <c>AppDomain</c> builder that is used to create the application domains.
        /// </summary>
        private readonly AppDomainBuilder m_Builder = new AppDomainBuilder();

        /// <summary>
        /// The collection that contains the base and private path information
        /// for the <c>AppDomain</c>s that need to be created.
        /// </summary>
        private readonly IKernelStartInfo m_StartInfo;

        /// <summary>
        /// The factory used to create <c>IExceptionHandler</c> objects.
        /// </summary>
        private readonly Func<IExceptionHandler> m_ExceptionHandlerFactory;

        /// <summary>
        /// Tracks the progress of the bootstrapping process.
        /// </summary>
        private readonly ITrackProgress m_Progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootStrapper"/> class.
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
        protected BootStrapper(
            IKernelStartInfo startInfo, 
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
                m_Progress.StartTracking((progress, mark) => RaiseStartupProgress(progress, mark));
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
            var serviceTypes = GetServiceTypes();

            // Create all the services and pass them to the kernel
            foreach (var serviceType in serviceTypes)
            {
                CreateService(serviceType, coreBasePath, kernel);
            }

            // Mark progress to starting core
            {
                m_Progress.Mark(new CoreStartingProgressMark());
            }

            // Finally start the kernel and wait for it to finish starting
            kernel.Start();

            // Indicate core startup is done
            {
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
            var fusionHelper = new FusionHelper(() =>
                {
                    // Concatenate the two sequences and then turn the FileInfo objects into the
                    // string representation of the file fullname
                    var totalSequence = m_StartInfo.CoreAssemblies.Concat(m_StartInfo.UserInterfaceAssemblies);
                    return from file in totalSequence select file.FullName;
                });
            currentDomain.AssemblyResolve += new ResolveEventHandler(fusionHelper.LocateAssemblyOnAssemblyLoadFailure);

            // Set the exception handler. Adding the event ensures that
            // the exceptionHandler cannot be collected until the AppDomain
            // is killed.
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(m_ExceptionHandlerFactory().OnUnhandledException);
        }

        /// <summary>
        /// Creates the kernel AppDomain and injects the appropriate kernel objects..
        /// </summary>
        /// <param name="coreBasePath">The core base path.</param>
        /// <returns>
        /// The proxy to the injector.
        /// </returns>
        private IInjectKernels CreateKernel(DirectoryInfo coreBasePath)
        {
            // Create the kernel appdomain. 
            var kernelDomain = m_Builder.AssembleWithFilePaths(
                coreBasePath,
                () =>
                    {
                        return from file in m_StartInfo.CoreAssemblies select file.FullName;
                    },
                m_ExceptionHandlerFactory());
            var kernel = kernelDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(KernelInjector).FullName) as IInjectKernels;

            // And then create the kernel
            kernel.CreateKernel();
            return kernel;
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

            Func<IEnumerable<string>> filePaths;
            Func<IEnumerable<string>> directoryPaths;
            SelectPaths(serviceType, out filePaths, out directoryPaths);

            AppDomain serviceDomain = m_Builder.AssembleWithFileAndDirectoryPaths(
                string.Format(CultureInfo.InvariantCulture, "AppDomain for {0}", serviceType.Name),
                coreBasePath,
                filePaths,
                directoryPaths,
                m_ExceptionHandlerFactory());
            var injector = serviceDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ServiceInjector).FullName) as IInjectServices;

            // Prepare the appdomain
            KernelService service = injector.CreateService(serviceType);
            kernel.InstallService(service);
        }

        /// <summary>
        /// Selects the assembly resolution paths necessary for the given service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="files">The files.</param>
        /// <param name="directories">The directories.</param>
        private void SelectPaths(Type serviceType, out Func<IEnumerable<string>> files, out Func<IEnumerable<string>> directories)
        {
            // Determine which log paths should be applied. 
            // Note that the plugins path requires a directory resolver, the others
            // require file resolvers!
            var options = from option in serviceType.GetCustomAttributes(typeof(PrivateBinPathRequirementsAttribute), true)
                          select (option as PrivateBinPathRequirementsAttribute).Option;

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
                    case PrivateBinPathOption.Plugins:
                        directoryPaths = m_StartInfo.PluginDirectories;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            files = null;
            if (filePaths != null)
            {
                files = () => from file in filePaths select file.FullName;
            }

            directories = null;
            if (directoryPaths != null)
            {
                directories = () => from directory in directoryPaths select directory.FullName;
            }
        }

        /// <summary>
        /// Occurs when there is a change in the progress of the system
        /// startup.
        /// </summary>
        public event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Raises the startup progress event with the specified values.
        /// </summary>
        /// <param name="progress">The progress percentage. Should be between 0 and 100.</param>
        /// <param name="currentlyProcessing">The description of what is currently being processed.</param>
        private void RaiseStartupProgress(int progress, IProgressMark currentlyProcessing)
        {
            EventHandler<StartupProgressEventArgs> local = StartupProgress;
            if (local != null)
            { 
                local(this, new StartupProgressEventArgs(progress, currentlyProcessing));
            }
        }
    }
}
