//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Apollo.Utils;
using Apollo.Utils.Logging;
using Autofac;

namespace Apollo.Core.Utils
{
    /// <summary>
    /// Handles the component registrations for the utilities part 
    /// of the core.
    /// </summary>
    [ExcludeFromCoverage("Modules are used for dependency injection purposes. Testing is done through integration testing.")]
    internal sealed partial class UtilsModule : Module
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

                builder.Register(c => LoggerBuilder.ForFile(
                        Path.Combine(c.Resolve<IFileConstants>().LogPath(), c.Resolve<IApplicationConstants>().ApplicationName + c.Resolve<IFileConstants>().LogExtension),
                        new DebugLogTemplate(() => DateTimeOffset.Now)))
                    .As<ILogger>()
                    .SingleInstance();
            }
        }
    }
}
