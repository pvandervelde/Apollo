//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Utilities.Commands;
using Autofac;
using Utilities.Communication;
using Utilities.Configuration;
using Utilities.FileSystem;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Handles the component registrations for the kernel part 
    /// of the core.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class KernelModule : Module
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
            base.Load(builder);

            builder.Register(c => new NotificationNameConstants())
                .As<INotificationNameConstants>();

            builder.Register(c => new CommandFactory())
                .As<ICommandContainer>();

            builder.Register(c => new HostApplicationCommands(
                    c.Resolve<IVirtualizeFileSystems>(),
                    c.Resolve<IStoreUploads>(),
                    c.Resolve<IConfiguration>()))
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(IDatasetApplicationCommands), a.Instance);
                    })
                .As<IHostApplicationCommands>()
                .As<ICommandSet>()
                .SingleInstance();
        }
    }
}
