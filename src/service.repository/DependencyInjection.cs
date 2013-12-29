//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Apollo.Core.Base;
using Apollo.Core.Base.Plugins;
using Apollo.Service.Repository.Plugins;
using Apollo.Utilities;
using Autofac;
using NLog;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;
using Nuclei.Diagnostics.Profiling.Reporting;

namespace Apollo.Service.Repository
{
    internal static class DependencyInjection
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
                    Path.Combine(
                        c.Resolve<FileConstants>().LogPath(),
                        string.Format(
                            CultureInfo.InvariantCulture,
                            DefaultInfoFileName,
                            Process.GetCurrentProcess().Id)),
                    new DebugLogTemplate(
                        c.Resolve<IConfiguration>(),
                        () => DateTimeOffset.Now)))
                .As<ILogger>()
                .SingleInstance();
        }

        private static void RegisterProfiler(ContainerBuilder builder)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[LoadProfilerAppSetting];

                bool result;
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

        private static void RegisterNotifications(ContainerBuilder builder)
        {
        }

        private static void RegisterCommands(ContainerBuilder builder)
        {
        }

        private static void RegisterPlugins(ContainerBuilder builder)
        {
            builder.Register((c, p) => new AppDomainOwningPluginScanner(
                    c.Resolve<Func<string, AppDomainPaths, AppDomain>>(),
                    p.TypedAs<IPluginRepository>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IAssemblyScanner>();

            builder.Register<Func<IPluginRepository, IAssemblyScanner>>(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return r => ctx.Resolve<IAssemblyScanner>(new TypedParameter(typeof(IPluginRepository), r));
                    });

            builder.Register(c => new PluginDetector(
                    c.Resolve<IPluginRepository>(),
                    c.Resolve<Func<IPluginRepository, IAssemblyScanner>>(),
                    c.Resolve<IFileSystem>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<PluginDetector>();

            builder.Register(c => new GroupImportEngine(
                    c.Resolve<ISatisfyPluginRequests>(),
                    c.Resolve<IConnectParts>()))
                .As<IConnectGroups>();

            builder.Register(c => new PartImportEngine(
                    c.Resolve<ISatisfyPluginRequests>()))
                .As<IConnectParts>();

            builder.Register(c => new PluginRepository())
                .As<IPluginRepository>()
                .As<ISatisfyPluginRequests>()
                .SingleInstance();
        }

        /// <summary>
        /// Creates the DI container for the application.
        /// </summary>
        /// <returns>The DI container.</returns>
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            {
                builder.Register(c => new ApplicationConstants())
                   .As<ApplicationConstants>();

                builder.Register(c => new FileConstants(c.Resolve<ApplicationConstants>()))
                    .As<FileConstants>();

                builder.Register(c => new XmlConfiguration(
                        CommunicationConfigurationKeys.ToCollection()
                            .Append(DiagnosticsConfigurationKeys.ToCollection())
                            .ToList(),
                        RepositoryServiceConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>();

                builder.RegisterModule(
                    new CommunicationModule(
                        new List<CommunicationSubject>
                            {
                                CommunicationSubjects.Plugins,
                            },
                        new[]
                            {
                                ChannelType.NamedPipe,
                                ChannelType.TcpIP,
                            },
                        true));

                RegisterCommands(builder);
                RegisterNotifications(builder);

                RegisterLoggers(builder);
                RegisterProfiler(builder);
                RegisterDiagnostics(builder);

                RegisterPlugins(builder);
            }

            var result = builder.Build();
            return result;
        }
    }
}
