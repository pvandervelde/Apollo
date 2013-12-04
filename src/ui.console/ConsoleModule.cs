//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Apollo.UI.Console.Nuclei.AppDomains;
using Apollo.UI.Console.Nuclei.ExceptionHandling;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Autofac;
using NLog;
using NSarrac.Framework;
using Nuclei;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;
using Nuclei.Diagnostics.Profiling.Reporting;

namespace Apollo.UI.Console
{
    /// <summary>
    /// Handles the component registrations for the console application.
    /// </summary>
    internal sealed class ConsoleModule : Autofac.Module
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultInfoFileName = "console.info.log";

        /// <summary>
        /// The default name for the profiler log.
        /// </summary>
        private const string DefaultProfilerFileName = "console.profile";

        /// <summary>
        /// The name for the plugins directory.
        /// </summary>
        private const string PluginsDirectoryName = "plugins";

        private static AppDomainResolutionPaths AppDomainResolutionPathsFor(FileConstants fileConstants, AppDomainPaths paths)
        {
            var filePaths = new List<string>();
            var directoryPaths = new List<string>();
            if ((paths & AppDomainPaths.Core) == AppDomainPaths.Core)
            {
                directoryPaths.Add(Assembly.GetExecutingAssembly().LocalDirectoryPath());
            }

            // @Todo: Note that the Apollo.Core.Host.Plugins.PluginService needs these paths ...
            if ((paths & AppDomainPaths.Plugins) == AppDomainPaths.Plugins)
            {
                // Plugins can be found in:
                // - The plugins directory in the main app directory (i.e. <INSTALL_DIRECTORY>\plugins)
                // - In the machine location for plugins (i.e. <COMMON_APPLICATION_DATA>\<COMPANY>\plugins)
                // - In the user location for plugins (i.e. <LOCAL_APPLICATION_DATA>\<COMPANY>\plugins)
                directoryPaths.Add(Path.Combine(Assembly.GetExecutingAssembly().LocalDirectoryPath(), PluginsDirectoryName));
                directoryPaths.Add(Path.Combine(fileConstants.CompanyCommonPath(), PluginsDirectoryName));
                directoryPaths.Add(Path.Combine(fileConstants.CompanyUserPath(), PluginsDirectoryName));
            }

            return AppDomainResolutionPaths.WithFilesAndDirectories(
                Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
                filePaths,
                directoryPaths);
        }

        private static void RegisterAppDomainBuilder(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    Func<string, AppDomainPaths, AppDomain> result =
                        (name, paths) => AppDomainBuilder.Assemble(
                            name,
                            AppDomainResolutionPathsFor(ctx.Resolve<FileConstants>(), paths));

                    return result;
                })
                .As<Func<string, AppDomainPaths, AppDomain>>()
                .SingleInstance();
        }

        private static void RegisterDiagnostics(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var loggers = c.Resolve<IEnumerable<ILogger>>();
                    Action<LevelToLog, string> action = (p, s) =>
                    {
                        var msg = new LogMessage(p, s);
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

                    Profiler profiler = null;
                    if (c.IsRegistered<Profiler>())
                    {
                        profiler = c.Resolve<Profiler>();
                    }

                    return new SystemDiagnostics(action, profiler);
                })
                .As<SystemDiagnostics>()
                .SingleInstance();
        }

        private static void RegisterLoggers(ContainerBuilder builder)
        {
            builder.Register(c => LoggerBuilder.ForFile(
                    Path.Combine(c.Resolve<FileConstants>().LogPath(), DefaultInfoFileName),
                    new DebugLogTemplate(
                        c.Resolve<IConfiguration>(),
                        () => DateTimeOffset.Now)))
                .As<ILogger>()
                .SingleInstance();
        }

        private static void RegisterProfiler(ContainerBuilder builder)
        {
            if (ConfigurationHelpers.ShouldBeProfiling())
            {
                builder.Register((c, p) => new TextReporter(p.TypedAs<Func<Stream>>()))
                        .As<TextReporter>()
                        .As<ITransformReports>();

                builder.Register(c => new TimingStorage())
                    .OnRelease(
                        storage =>
                        {
                            // Write all the profiling results out to disk. Do this the ugly way 
                            // because we don't know if any of the other items in the container have
                            // been removed yet.
                            Func<Stream> factory =
                                () => new FileStream(
                                    Path.Combine(new FileConstants(new ApplicationConstants()).LogPath(), DefaultProfilerFileName),
                                    FileMode.Append,
                                    FileAccess.Write,
                                    FileShare.Read);
                            var reporter = new TextReporter(factory);
                            reporter.Transform(storage.FromStartTillEnd());
                        })
                    .As<IStoreIntervals>()
                    .As<IGenerateTimingReports>()
                    .SingleInstance();

                builder.Register(c => new Profiler(
                        c.Resolve<IStoreIntervals>()))
                    .SingleInstance();
            }
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
        /// that the module is being registered by (i.e. it can have its own defaults).
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register the global application objects
            {
                builder.Register((c, p) => new ExceptionHandler(
                        p.TypedAs<ExceptionProcessor[]>()))
                    .As<ExceptionHandler>();

                builder.Register(c => new ApplicationConstants())
                    .As<ApplicationConstants>();

                builder.Register(c => new FileConstants(c.Resolve<ApplicationConstants>()))
                    .As<FileConstants>();

                builder.Register(
                    c => new XmlConfiguration(
                        CommunicationConfigurationKeys.ToCollection()
                            .Append(DiagnosticsConfigurationKeys.ToCollection())
                            .ToList(),
                        ConsoleConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>();

                builder.Register(c => new NotificationReportingHub())
                    .As<ICollectNotifications>()
                    .SingleInstance();

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

                RSAParameters rsaParameters = SrcOnlyExceptionHandlingUtilities.ReportingPublicKey();
                builder.RegisterModule(new FeedbackReportingModule(() => rsaParameters));

                RegisterLoggers(builder);
                RegisterProfiler(builder);
                RegisterDiagnostics(builder);
                RegisterAppDomainBuilder(builder);
                RegisterTimeline(builder);
            }
        }
    }
}
