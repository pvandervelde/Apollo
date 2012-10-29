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
using Apollo.Core.Base;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Plugins.Definitions;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Autofac;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Handles the component registrations for the plugins part 
    /// of the core.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class PluginsModule : Module
    {
        /// <summary>
        /// Defines methods to load an <see cref="IAssemblyScanner"/> object into a remote <c>AppDomain</c>.
        /// </summary>
        private sealed class AppDomainPluginClassLoader : MarshalByRefObject
        {
            /// <summary>
            /// The container that is used to get the scheduling.
            /// </summary>
            private readonly IContainer m_Container;

            /// <summary>
            /// Initializes a new instance of the <see cref="AppDomainPluginClassLoader"/> class.
            /// </summary>
            public AppDomainPluginClassLoader()
            {
                var builder = new ContainerBuilder();
                {
                    builder.RegisterModule(new BaseModuleForScheduling());
                }

                m_Container = builder.Build();
            }

            /// <summary>
            /// Loads the <see cref="IAssemblyScanner"/> object into the <c>AppDomain</c> in which the current
            /// object is currently loaded.
            /// </summary>
            /// <param name="logger">The object that provides the logging for the remote <c>AppDomain</c>.</param>
            /// <returns>The newly created <see cref="IAssemblyScanner"/> object.</returns>
            public IAssemblyScanner Load(ILogMessagesFromRemoteAppDomains logger)
            {
                try
                {
                    Func<IBuildFixedSchedules> scheduleBuilder = () => m_Container.Resolve<IBuildFixedSchedules>();
                    return new RemoteAssemblyScanner(logger, scheduleBuilder);
                }
                catch (Exception e)
                {
                    logger.Log(LogSeverityProxy.Error, e.ToString());
                    throw;
                }
            }
        }

        /// <summary>
        /// Provides an <see cref="IAssemblyScanner"/> wrapper that loads the actual scanner into a <c>AppDomain</c>, provides the data
        /// to that scanner and then unloads the <c>AppDomain</c> when the scanning process is complete.
        /// </summary>
        private sealed class AppDomainOwningPluginScanner : IAssemblyScanner
        {
            /// <summary>
            /// The function that builds an <c>AppDomain</c> when requested.
            /// </summary>
            private readonly Func<string, AppDomainPaths, AppDomain> m_AppDomainBuilder;

            /// <summary>
            /// The object that provides the diagnostics for the system.
            /// </summary>
            private readonly SystemDiagnostics m_Diagnostics;

            /// <summary>
            /// Initializes a new instance of the <see cref="AppDomainOwningPluginScanner"/> class.
            /// </summary>
            /// <param name="appDomainBuilder">The function that is used to create a new <c>AppDomain</c> which will be used to scan plugins.</param>
            /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
            public AppDomainOwningPluginScanner(Func<string, AppDomainPaths, AppDomain> appDomainBuilder, SystemDiagnostics diagnostics)
            {
                {
                    Debug.Assert(appDomainBuilder != null, "The AppDomain building function should not be a null reference.");
                    Debug.Assert(diagnostics != null, "The diagnostics object should not be a null reference.");
                }

                m_AppDomainBuilder = appDomainBuilder;
                m_Diagnostics = diagnostics;
            }

            /// <summary>
            /// Scans the assemblies for which the given file paths have been provided and 
            /// returns the plugin description information.
            /// </summary>
            /// <param name="assemblyFilesToScan">
            /// The collection that contains the file paths to all the assemblies to be scanned.
            /// </param>
            /// <param name="plugins">The collection that describes the plugin information in the given assembly files.</param>
            /// <param name="types">
            /// The collection that provides information about all the types which are required to complete the type hierarchy
            /// for the plugin types.
            /// </param>
            public void Scan(
                IEnumerable<string> assemblyFilesToScan,
                out IEnumerable<PluginInfo> plugins,
                out IEnumerable<SerializedTypeDefinition> types)
            {
                var domain = m_AppDomainBuilder(Resources.Plugins_PluginScanDomainName, AppDomainPaths.Plugins);
                try
                {
                    // Inject the actual scanner
                    var loader = domain.CreateInstanceAndUnwrap(
                        typeof(AppDomainPluginClassLoader).Assembly.FullName, 
                        typeof(AppDomainPluginClassLoader).FullName) as AppDomainPluginClassLoader;

                    var logger = new LogForwardingPipe(m_Diagnostics);
                    var scannerProxy = loader.Load(logger);
                    scannerProxy.Scan(assemblyFilesToScan, out plugins, out types);
                }
                finally
                {
                    if (domain != null)
                    {
                        AppDomain.Unload(domain);
                    }
                }
            }
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults.)
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new AppDomainOwningPluginScanner(
                    c.Resolve<Func<string, AppDomainPaths, AppDomain>>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IAssemblyScanner>();

            builder.Register<Func<IAssemblyScanner>>(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return () => ctx.Resolve<IAssemblyScanner>();
                    });

            builder.Register(c => new PluginDetector(
                    c.Resolve<IPluginRepository>(),
                    c.Resolve<Func<IAssemblyScanner>>(),
                    c.Resolve<IVirtualizeFileSystems>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<PluginDetector>();
        }
    }
}
