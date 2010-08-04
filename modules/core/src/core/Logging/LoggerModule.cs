//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Messaging;
using Apollo.Utils;
using Autofac;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Handles the component registrations for the logger part 
    /// of the core.
    /// </summary>
    [ExcludeFromCoverage("Modules are used for dependency injection purposes. Testing is done through integration testing.")]
    internal sealed class LoggerModule : Module
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

            // Utilities
            builder.Register<Func<DateTimeOffset>>(c => () => DateTimeOffset.Now)
                .As<Func<DateTimeOffset>>();

            // Log templates
            builder.Register(c => new DebugLogTemplate(c.Resolve<Func<DateTimeOffset>>()))
                .As<DebugLogTemplate>();

            builder.Register(c => new CommandLogTemplate(c.Resolve<Func<DateTimeOffset>>()))
                .As<CommandLogTemplate>();

            // Logger configuration
            builder.Register(c => new LoggerConfiguration(
                    c.Resolve<IFileConstants>().LogPath(), 
                    10,
                    500))
                .As<ILoggerConfiguration>();

            // LogSink
            builder.Register(c => new LogSink(
                    c.Resolve<IHelpMessageProcessing>(),
                    c.Resolve<ILoggerConfiguration>(),
                    c.Resolve<DebugLogTemplate>(),
                    c.Resolve<CommandLogTemplate>(),
                    c.Resolve<IFileConstants>()))
                .As<LogSink>();
        }
    }
}
