//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using Autofac;
using Utilities;
using Utilities.FileSystem;
using Utilities.Progress;

namespace Apollo.Core.Host
{
    /// <summary>
    /// Handles the component registrations for the utilities part 
    /// of the core.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed partial class UtilitiesModule : Module
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

            // Register the global application objects
            {
                builder.Register(c => new ApplicationConstants())
                    .As<IApplicationConstants>()
                    .As<ICompanyConstants>();

                builder.Register(c => new FileConstants(c.Resolve<IApplicationConstants>()))
                    .As<IFileConstants>();

                builder.Register((c, p) => new ProgressTimer(
                        p.TypedAs<TimeSpan>()))
                    .As<IProgressTimer>()
                    .InstancePerDependency();

                builder.Register(c => new VirtualFileSystem())
                    .As<IVirtualizeFileSystems>();
            }
        }
    }
}
