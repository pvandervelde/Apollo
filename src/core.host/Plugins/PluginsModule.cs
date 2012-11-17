//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
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
                    c.Resolve<IPluginRepository>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IAssemblyScanner>();

            builder.Register<Func<IPluginRepository, IAssemblyScanner>>(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return r => ctx.Resolve<IAssemblyScanner>();
                    });

            builder.Register(c => new PluginDetector(
                    c.Resolve<IPluginRepository>(),
                    c.Resolve<Func<IPluginRepository, IAssemblyScanner>>(),
                    c.Resolve<IVirtualizeFileSystems>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<PluginDetector>();
        }
    }
}
