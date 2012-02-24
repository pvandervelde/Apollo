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
using System.Security.Cryptography;
using Apollo.ProjectExplorer.Utilities;
using Apollo.Utilities.Configuration;
using Apollo.Utilities.ExceptionHandling;
using Apollo.Utilities.History;
using Apollo.Utilities.Logging;
using Autofac;
using NLog;
using NSarrac.Framework;

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

        private static void RegisterDiagnostics(ContainerBuilder builder)
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

            builder.Register<SystemDiagnostics>(
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

                    return new SystemDiagnostics(action, null);
                })
                .As<SystemDiagnostics>()
                .SingleInstance();
        }

        private static void RegisterTimeline(ContainerBuilder builder)
        {
            // Apparently we can do this by registering the most generic class
            // first and the least generic (i.e. the most limited) class last
            // But then we also need a way to provide the correct parameters
            // and that is a bit more tricky with a RegisterGeneric method call.
            builder.RegisterSource(new DictionaryTimelineRegistrationSource());
            builder.RegisterSource(new ListTimelineRegistrationSource());
            builder.RegisterSource(new ValueTimelineRegistrationSource());

            builder.Register(
                c => 
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return new Timeline(t => { return ctx.Resolve(t) as IStoreTimelineValues; }); 
                })
                .As<ITimeline>()
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

                builder.Register(c => new ProgressReportingHub())
                    .As<ICollectProgressReports>()
                    .SingleInstance();

                builder.Register(c => new StepBasedProgressTracker())
                    .OnActivated(
                        a =>
                        {
                            var hub = a.Context.Resolve<ICollectProgressReports>();
                            hub.AddReporter(a.Instance);
                        })
                    .As<ITrackSteppingProgress>()
                    .As<ITrackProgress>();

                RSAParameters rsaParameters = SrcOnlyExceptionHandlingUtillities.ReportingPublicKey();
                builder.RegisterModule(new FeedbackReportingModule(() => rsaParameters));

                // Register the loggers
                RegisterDiagnostics(builder);
                RegisterAppDomainBuilder(builder);
                RegisterTimeline(builder);
            }
        }
    }
}
