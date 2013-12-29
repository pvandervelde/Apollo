//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Base.Plugins;
using Autofac;

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
            builder.Register(c => new GroupImportEngine( 
                    c.Resolve<ISatisfyPluginRequests>(),
                    c.Resolve<IConnectParts>()))
                .As<IConnectGroups>();

            builder.Register(c => new PartImportEngine(
                    c.Resolve<ISatisfyPluginRequests>()))
                .As<IConnectParts>();

            builder.Register(c => new RemotePluginRepositoryProxy())
                .As<ISatisfyPluginRequests>()
                .SingleInstance();
        }
    }
}
