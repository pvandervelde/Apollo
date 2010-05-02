//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using Autofac;

namespace Apollo.Core
{
    /// <summary>
    /// Handles the component registrations for the kernel part 
    /// of the core.
    /// </summary>
    internal sealed class KernelModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="moduleBuilder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults.)
        /// </remarks>
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            base.Load(moduleBuilder);

            moduleBuilder.Register(c => new DnsNameConstants())
                .As<IDnsNameConstants>();

            moduleBuilder.Register(c => new CommandFactory())
                .As<ICommandContainer>();
        }
    }
}
