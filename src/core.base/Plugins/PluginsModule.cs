//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Autofac;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Handles the component registrations for the plug-in components.
    /// </summary>
    public sealed class PluginsModule : Module
    {
        private static void RegisterConnector(ContainerBuilder builder)
        {
            builder.Register(c => new PluginRepositoryConnector())
                .As<IProvideConnectionToRepositories>()
                .SingleInstance();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterConnector(builder);
        }
    }
}
