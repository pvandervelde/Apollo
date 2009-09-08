//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public abstract partial class BootStrapper
    {
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
        /// Initializes a new instance of the <see cref="BootStrapper"/> class.
        /// </summary>
        /// <param name="startInfo">
        ///     The collection of <c>AppDomain</c> base and private paths.
        /// </param>
        /// <param name="exceptionHandlerFactory">
        ///     The factory used for the creation of <see cref="IExceptionHandler"/> objects.
        /// </param>
        protected BootStrapper(KernelStartInfo startInfo, Func<IExceptionHandler> exceptionHandlerFactory)
        {
            {
                Enforce.Argument(() => startInfo);
                Enforce.Argument(() => exceptionHandlerFactory);
            }

            m_StartInfo = startInfo;
            m_ExceptionHandlerFactory = exceptionHandlerFactory;
        }

        /// <summary>
        /// Loads the Apollo system and starts the kernel.
        /// </summary>
        public void Load()
        { 
            // Link assembly resolver to current AppDomain
            // Link exception handlers to current domain
            //PrepareUserInterfaceAppDomain(xx, yy);

            // Create the kernel appdomain. 
            var kernel = CreateKernelAppDomain();

            // Prepare the appdomain, set:
            // - Security levels
            // - search paths
            // - assembly resolver
            // - exception handlers
            // And then create the kernel
            //kernel.PrepareAppDomain(aa, yy);
            kernel.CreateKernel();

            // Scan the current assembly for all exported parts.
            var serviceTypes = GetServiceTypes();

            // Create all the services and pass them to the kernel
            foreach (var serviceType in serviceTypes)
            {
                //AppDomain serviceDomain = CreateAppDomain(bb);
                //var injector = serviceDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ServiceInjector).FullName) as IInjectServices;

                // Prepare the appdomain, set:
                // - Security levels
                // - search paths
                // - assembly resolver
                // - exception handlers
                // And then create the kernel
                //injector.PrepareAppDomain(cc, yy);
                //KernelService service = injector.CreateService(serviceType);
                //kernel.InstallService(service);
            }

            // Finally start the kernel and wait for it to finish starting
            //kernel.Start();
        }

        private void PrepareUserInterfaceAppDomain(IEnumerable<FileInfo> assemblyDirectories,
                IExceptionHandler exceptionHandler)
        {
            throw new NotImplementedException();
        }

        private IInjectKernels CreateKernelAppDomain()
        {
            // Create the kernel appdomain. 
            //AppDomain kernelDomain = CreateAppDomain(zz);
            //return kernelDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(KernelInjector).FullName) as IInjectKernels;

            throw new NotImplementedException();
        }

        private AppDomain CreateAppDomain(IEnumerable<DirectoryInfo> searchPaths)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = string.Empty;
            
            // Shadow copy files
            setup.ShadowCopyFiles = "true";

            // Do not load any assemblies from HTTP connections
            setup.DisallowCodeDownload = true;

            throw new NotImplementedException();
        }

        private IEnumerable<Type> GetServiceTypes()
        {
            // Find all Kernel services that are not auto loaded.
            var serviceTypes = (from type in Assembly.GetExecutingAssembly().GetTypes()
                                where typeof(KernelService).IsAssignableFrom(type) &&
                                      type.GetCustomAttributes(typeof(AutoLoadAttribute),true).Length == 0
                                select type);

            return serviceTypes;
        }
    }
}
