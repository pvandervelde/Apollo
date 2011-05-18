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
using Apollo.Utils.Logging;
using Autofac;

namespace Apollo.Utils
{
    /// <summary>
    /// Handles the component registrations for the utilities part 
    /// of the core.
    /// </summary>
    [ExcludeFromCodeCoverage()]
    internal sealed partial class UtilsModule : Autofac.Module
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string s_DefaultErrorFileName = "loaderapplication.error.log";

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
                // Utils
                builder.Register(c => new ApplicationConstants())
                   .As<IApplicationConstants>()
                   .As<ICompanyConstants>();

                builder.Register(c => new FileConstants(c.Resolve<IApplicationConstants>()))
                    .As<IFileConstants>();

                // Register the loggers
                builder.Register(c => LoggerBuilder.ForFile(
                        Path.Combine(c.Resolve<IFileConstants>().LogPath(), s_DefaultErrorFileName),
                        new DebugLogTemplate(() => DateTimeOffset.Now)))
                    .As<ILogger>()
                    .SingleInstance();

                builder.Register(c => LoggerBuilder.ForEventLog(
                        Assembly.GetExecutingAssembly().GetName().Name,
                        new DebugLogTemplate(() => DateTimeOffset.Now)))
                    .As<ILogger>()
                    .SingleInstance();

                builder.Register<Action<LogSeverityProxy, string>>(c =>
                        {
                            var loggers = c.Resolve<IEnumerable<ILogger>>();
                            Action<LogSeverityProxy, string> action = (p, s) =>
                            {
                                var msg = new LogMessage(
                                    LogSeverityProxyToLogLevelMap.FromLogSeverityProxy(p),
                                    s);

                                foreach (var logger in loggers)
                                {
                                    logger.Log(msg);
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
