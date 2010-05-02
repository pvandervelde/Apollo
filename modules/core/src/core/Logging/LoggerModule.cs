//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;
using Autofac;
using Apollo.Core.Messaging;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Handles the component registrations for the logger part 
    /// of the core.
    /// </summary>
    internal sealed class LoggerModule : Module
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

            // Utilities
            moduleBuilder.Register<Func<DateTimeOffset>>(c => () => DateTimeOffset.Now)
                .As<Func<DateTimeOffset>>();

            // Log templates
            moduleBuilder.Register(c => new DebugLogTemplate(c.Resolve<Func<DateTimeOffset>>()))
                .As<DebugLogTemplate>();

            moduleBuilder.Register(c => new CommandLogTemplate(c.Resolve<Func<DateTimeOffset>>()))
                .As<CommandLogTemplate>();

            // Logger configuration
            moduleBuilder.Register(c => new LoggerConfiguration(
                    c.Resolve<IFileConstants>().LogPath(), 
                    10,
                    500))
                .As<ILoggerConfiguration>();

            // LogSink
            moduleBuilder.Register(c => new LogSink(
                    c.Resolve<IHelpMessageProcessing>(),
                    c.Resolve<ILoggerConfiguration>(),
                    c.Resolve<DebugLogTemplate>(),
                    c.Resolve<CommandLogTemplate>(),
                    c.Resolve<IFileConstants>()))
                .As<LogSink>();
        }
    }
}
