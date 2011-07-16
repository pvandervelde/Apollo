//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Apollo.Utilities.Logging;
using Autofac;
using NLog;

namespace Apollo.Utilities
{
    /// <summary>
    /// Handles the component registrations for the utilities part 
    /// of the core.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed partial class UtilitiesModule : Autofac.Module
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "loaderapplication.info.log";

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
                // Utilities
                builder.Register(c => new ApplicationConstants())
                   .As<IApplicationConstants>()
                   .As<ICompanyConstants>();

                builder.Register(c => new FileConstants(c.Resolve<IApplicationConstants>()))
                    .As<IFileConstants>();

                // Register the loggers
                builder.Register(c => LoggerBuilder.ForFile(
                        Path.Combine(c.Resolve<IFileConstants>().LogPath(), DefaultErrorFileName),
                        new DebugLogTemplate(() => DateTimeOffset.Now)))
                    .As<ILogger>()
                    .SingleInstance();

                builder.Register(c => LoggerBuilder.ForEventLog(
                        Assembly.GetExecutingAssembly().GetName().Name,
                        new DebugLogTemplate(() => DateTimeOffset.Now)))
                    .As<ILogger>()
                    .SingleInstance();

                builder.Register<Action<LogSeverityProxy, string>>(
                    c =>
                    {
                        var loggers = c.Resolve<IEnumerable<ILogger>>();
                        Action<LogSeverityProxy, string> action = (p, s) =>
                        {
                            var msg = new LogMessage(
                                LogSeverityProxyToLogLevelMap.FromLogSeverityProxy(p),
                                s);

                            foreach (var logger in loggers)
                            {
                                try
                                {
                                    logger.Log(msg);
                                }
                                catch (NLogRuntimeException)
                                {
                                    // Ignore it and move on to the next logger.
                                }
                            }
                        };

                        return action;
                    })
                    .As<Action<LogSeverityProxy, string>>()
                    .SingleInstance();
            }
        }
    }
}
