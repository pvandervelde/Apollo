//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using Apollo.Utilities.Configuration;
using Apollo.Utilities.History;
using Apollo.Utilities.Logging;
using Autofac;
using NLog;
using NManto;
using NManto.Reporting;

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
        private const string DefaultInfoFileName = "dataset.info.{0}.log";

        /// <summary>
        /// The default name for the profiler log.
        /// </summary>
        private const string DefaultProfilerFileName = "dataset.profile";

        /// <summary>
        /// The default key for the value that indicates if the profiler should be loaded or not.
        /// </summary>
        private const string LoadProfilerAppSetting = "LoadProfiler";

        private static void RegisterDiagnostics(ContainerBuilder builder)
        {
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
                    Path.Combine(c.Resolve<IFileConstants>().LogPath(), DefaultInfoFileName),
                    new DebugLogTemplate(() => DateTimeOffset.Now)))
                .As<ILogger>()
                .SingleInstance();

            builder.Register(c => LoggerBuilder.ForEventLog(
                    Assembly.GetExecutingAssembly().GetName().Name,
                    new DebugLogTemplate(() => DateTimeOffset.Now)))
                .As<ILogger>()
                .SingleInstance();
        }

        private static void RegisterProfiler(ContainerBuilder builder)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[LoadProfilerAppSetting];

                bool result = true;
                if (bool.TryParse(value, out result) && result)
                {
                    // Only register the storage and the profiler because we won't be writing out
                    // intermediate results here anyway. No point in registering report converters
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
                                        FileMode.OpenOrCreate,
                                        FileAccess.Write,
                                        FileShare.Read);
                                var reporter = new TextReporter(factory);
                                reporter.Transform(storage.FromStartTillEnd());
                            })
                        .As<IStoreIntervals>();

                    builder.Register(c => new Profiler(
                            c.Resolve<IStoreIntervals>()));
                }
            }
            catch (ConfigurationErrorsException)
            {
                // could not retrieve the AppSetting from the config file
                // meh ...
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

                builder.Register(c => new XmlConfiguration())
                    .As<IConfiguration>();

                RegisterLoggers(builder);
                RegisterProfiler(builder);
                RegisterDiagnostics(builder);
                RegisterTimeline(builder);
            }
        }
    }
}
