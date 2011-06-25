//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Apollo.Core.Base;
using Apollo.Utilities;
using Apollo.Utilities.Logging;
using Autofac;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Initializes the dependency injection system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class DependencyInjection
    {
        /// <summary>
        /// The template for the name of the information log.
        /// </summary>
        private const string DefaultLogFileNameTemplate = "dataset.info.{0}.log";

        /// <summary>
        /// Creates the DI container.
        /// </summary>
        /// <param name="context">The application context that controls the life time of the application.</param>
        /// <returns>The DI container.</returns>
        public static IContainer Load(ApplicationContext context)
        {
            var builder = new ContainerBuilder();
            {
                // @todo: we don't want to register ourselves for discovery
                builder.RegisterModule(new BaseModule());

                builder.Register(c => context)
                    .As<ApplicationContext>()
                    .ExternallyOwned();

                // Register the loggers
                builder.Register(c => LoggerBuilder.ForFile(
                        Path.Combine(
                            c.Resolve<IFileConstants>().LogPath(), 
                            string.Format(CultureInfo.InvariantCulture, DefaultLogFileNameTemplate, Process.GetCurrentProcess().Id)),
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

            return builder.Build();
        }
    }
}
