﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO.Abstractions;
using Apollo.Utilities;
using Autofac;
using Nuclei.Configuration;
using Nuclei.Diagnostics;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Handles the component registrations for the plugins part 
    /// of the core.
    /// </summary>
    internal sealed class PluginsModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults).
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) => new AppDomainOwningPluginScanner(
                    c.Resolve<Func<string, AppDomainPaths, AppDomain>>(),
                    p.TypedAs<IPluginRepository>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IAssemblyScanner>();

            builder.Register<Func<IPluginRepository, IAssemblyScanner>>(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return r => ctx.Resolve<IAssemblyScanner>(new TypedParameter(typeof(IPluginRepository), r));
                    });

            builder.Register(c => new PluginDetector(
                    c.Resolve<IPluginRepository>(),
                    c.Resolve<Func<IPluginRepository, IAssemblyScanner>>(),
                    c.Resolve<IFileSystem>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<PluginDetector>();

            builder.Register(c => new PluginService(
                    c.Resolve<IConfiguration>(),
                    c.Resolve<PluginDetector>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<PluginService>();

            builder.Register(c => new GroupImportEngine( 
                    c.Resolve<ISatisfyPluginRequests>(),
                    c.Resolve<IConnectParts>()))
                .As<IConnectGroups>();

            builder.Register(c => new PartImportEngine(
                    c.Resolve<ISatisfyPluginRequests>()))
                .As<IConnectParts>();

            builder.Register(c => new PluginRepository())
                .As<IPluginRepository>()
                .As<ISatisfyPluginRequests>()
                .SingleInstance();
        }
    }
}
