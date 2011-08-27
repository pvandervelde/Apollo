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
using Apollo.Utilities.Configuration;
using Apollo.Utilities.ExceptionHandling;
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
        private const string DefaultInfoFileName = "projectexplorer.info.log";

        private static AppDomainResolutionPaths AppDomainResolutionPathsFor(AppDomainPaths paths)
        {
            List<string> filePaths = new List<string>();
            List<string> directoryPaths = new List<string>();
            if ((paths & AppDomainPaths.Core) == AppDomainPaths.Core)
            {
                // Get the paths from the config
                // throw new NotImplementedException();
            }

            if ((paths & AppDomainPaths.Plugins) == AppDomainPaths.Plugins)
            {
                // Get the paths from the config
                // throw new NotImplementedException();
            }

            return AppDomainResolutionPaths.WithFilesAndDirectories(
                Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
                filePaths,
                directoryPaths);
        }

        private static void RegisterAppDomainBuilder(ContainerBuilder builder)
        {
            builder.Register<Func<string, AppDomainPaths, AppDomain>>(
                c =>
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    Func<string, AppDomainPaths, AppDomain> result = (name, paths) =>
                    {
                        return AppDomainBuilder.Assemble(
                            name,
                            AppDomainResolutionPathsFor(paths),
                            ctx.Resolve<IExceptionHandler>(),
                            ctx.Resolve<IFileConstants>());
                    };

                    return result;
                })
                .As<Func<string, AppDomainPaths, AppDomain>>()
                .SingleInstance();
        }

        private static void RegisterLoggers(ContainerBuilder builder)
        {
            builder.Register(c => LoggerBuilder.ForFile(
                    Path.Combine(c.Resolve<IFileConstants>().LogPath(), DefaultInfoFileName),
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

                builder.Register((c, p) => new ExceptionHandler(
                        p.Positional<string>(0),
                        p.Positional<string>(1)))
                    .As<IExceptionHandler>();

                builder.Register(c => new XmlConfiguration())
                    .As<IConfiguration>();

                // Register the loggers
                RegisterLoggers(builder);
                RegisterAppDomainBuilder(builder);
            }
        }
    }
}
