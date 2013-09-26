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
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Apollo.Core.Base;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Dataset.Plugins;
using Apollo.Core.Dataset.Scheduling;
using Apollo.Core.Dataset.Scheduling.Processors;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Autofac;
using NLog;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;
using Nuclei.Diagnostics.Profiling.Reporting;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Initializes the dependency injection system.
    /// </summary>
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

            builder.Register(c => LoggerBuilder.ForEventLog(
                    Assembly.GetExecutingAssembly().GetName().Name,
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

        private static void RegisterScheduleStorage(ContainerBuilder builder)
        {
            builder.Register(c => c.Resolve<ITimeline>().AddToTimeline<ScheduleActionStorage>(ScheduleActionStorage.CreateInstance))
                .As<IStoreScheduleActions>()
                .SingleInstance();

            builder.Register(c => c.Resolve<ITimeline>().AddToTimeline<ScheduleConditionStorage>(ScheduleConditionStorage.CreateInstance))
                .As<IStoreScheduleConditions>()
                .SingleInstance();

            builder.Register(c => c.Resolve<ITimeline>().AddToTimeline<ScheduleStorage>(ScheduleStorage.CreateInstance))
                .As<IStoreSchedules>()
                .SingleInstance();

            builder.Register(c => new HistoryMarkerStorage())
                .As<IStoreHistoryMarkers>();
        }

        private static void RegisterTimeline(ContainerBuilder builder)
        {
            builder.Register(c => new BidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge>())
                .As<IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge>>();

            builder.Register(c => new BidirectionalGraphHistory<PartCompositionId, PartImportExportEdge<PartCompositionId>>())
                .As<IBidirectionalGraphHistory<PartCompositionId, PartImportExportEdge<PartCompositionId>>>();

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

        private static void RegisterScheduleExecutors(ContainerBuilder builder)
        {
            builder.Register(c => new ActionVertexProcessor(
                    c.Resolve<IStoreScheduleActions>()))
                .As<IProcesExecutableScheduleVertices>();

            builder.Register(c => new StartVertexProcessor())
                .As<IProcesExecutableScheduleVertices>();

            builder.Register(c => new EndVertexProcessor())
                .As<IProcesExecutableScheduleVertices>();

            builder.Register(c => new InsertVertexProcessor())
                .As<IProcesExecutableScheduleVertices>();

            builder.Register(c => new SubScheduleVertexProcessor(
                    c.Resolve<IDistributeScheduleExecutions>()))
                .As<IProcesExecutableScheduleVertices>();

            builder.Register(
                    c => 
                    {
                        var storage = c.Resolve<IStoreHistoryMarkers>();
                        return new MarkHistoryVertexProcessor(
                            c.Resolve<ITimeline>(),
                            m => 
                            {
                                storage.Add(m);
                            });
                    })
                .As<IProcesExecutableScheduleVertices>();

            builder.Register((c, p) => new ScheduleExecutor(
                    c.Resolve<IEnumerable<IProcesExecutableScheduleVertices>>(),
                    c.Resolve<IStoreScheduleConditions>(),
                    p.TypedAs<ISchedule>(),
                    p.TypedAs<ScheduleId>(),
                    p.TypedAs<ScheduleExecutionInfo>()))
                .As<IExecuteSchedules>();

            builder.Register(
                    c => 
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new ScheduleDistributor(
                            c.Resolve<IStoreSchedules>(),
                            (s, id, i) =>
                            {
                                return ctx.Resolve<IExecuteSchedules>(
                                    new TypedParameter(typeof(ISchedule), s),
                                    new TypedParameter(typeof(ScheduleId), id),
                                    new TypedParameter(typeof(ScheduleExecutionInfo), i));
                            });
                    })
                .As<IDistributeScheduleExecutions>()
                .SingleInstance();
        }

        private static void RegisterPartStorage(ContainerBuilder builder)
        {
            builder.Register(c => c.Resolve<ITimeline>().AddToTimeline<CompositionLayer>(CompositionLayer.CreateInstance))
                .As<IStoreGroupsAndConnections>()
                .SingleInstance();

            builder.Register(c => c.Resolve<ITimeline>().AddToTimeline<InstanceLayer>(InstanceLayer.CreateInstance))
                .As<IStoreInstances>()
                .SingleInstance();
        }

        private static void RegisterDatasetLock(ContainerBuilder builder)
        {
            builder.Register(c => new DatasetLock())
                .OnActivated(
                    a =>
                    {
                        DatasetLock obj = a.Instance;
                        var notifications = a.Context.Resolve<IDatasetApplicationNotificationInvoker>();
                        obj.OnLockForReading += (s, e) => notifications.RaiseOnSwitchToEditingMode();
                        obj.OnUnlockFromReading += (s, e) => notifications.RaiseOnSwitchToExecutingMode();
                    })
                .As<ITrackDatasetLocks>()
                .SingleInstance();
        }

        private static void RegisterNotifications(ContainerBuilder builder)
        {
            builder.Register(c => new DatasetApplicationNotifications())
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<INotificationSendersCollection>();
                        collection.Store(typeof(IDatasetApplicationNotifications), a.Instance);
                    })
                .As<IDatasetApplicationNotificationInvoker>()
                .As<IDatasetApplicationNotifications>()
                .As<INotificationSet>()
                .SingleInstance();
        }

        private static void RegisterCommands(
            ContainerBuilder builder,
            Action closeAction,
            Action<FileInfo> loadAction)
        {
            builder.Register(c => new DatasetApplicationCommands(
                    c.Resolve<DownloadDataFromRemoteEndpoints>(),
                    c.Resolve<ITrackDatasetLocks>(),
                    closeAction,
                    loadAction,
                    c.Resolve<SystemDiagnostics>()))
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(IDatasetApplicationCommands), a.Instance);
                    })
                .As<IDatasetApplicationCommands>()
                .As<ICommandSet>()
                .SingleInstance();

            builder.Register(c => new CompositionCommands(
                    c.Resolve<ITrackDatasetLocks>(),
                    c.Resolve<IStoreGroupsAndConnections>()))
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(ICompositionCommands), a.Instance);
                    })
                .As<ICompositionCommands>()
                .As<ICommandSet>()
                .SingleInstance();
        }

        private static void RegisterAssemblyResolver(ContainerBuilder builder)
        {
            builder.Register((c, p) => new PluginLoadingAssemblyResolver(
                c.Resolve<ISendCommandsToRemoteEndpoints>(),
                c.Resolve<DownloadDataFromRemoteEndpoints>(),
                c.Resolve<IFileSystem>(),
                p.TypedAs<EndpointId>()));
        }

        /// <summary>
        /// Creates the DI container.
        /// </summary>
        /// <param name="context">The application context that controls the life time of the application.</param>
        /// <returns>The DI container.</returns>
        public static IContainer Load(ApplicationContext context)
        {
            IContainer result = null;
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
                        DatasetApplicationConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>();

                builder.Register(c => new FileSystem())
                    .As<IFileSystem>();

                // Don't allow discovery on the dataset application because:
                // - The dataset application wouldn't know what to do with it anyway
                // - We don't want anybody talking to the application except for the
                //   application that started it.
                builder.RegisterModule(
                    new CommunicationModule(
                        new List<CommunicationSubject>
                            {
                                CommunicationSubjects.Dataset,
                            },
                        new[]
                            {
                                ChannelType.NamedPipe,
                            }, 
                        false));
                
                RegisterCommands(
                    builder,
                    () => CloseApplication(result),
                    file => LoadDatasetFile(result, file));
                RegisterNotifications(builder);

                builder.Register(c => context)
                    .As<ApplicationContext>()
                    .ExternallyOwned();

                RegisterLoggers(builder);
                RegisterProfiler(builder);
                RegisterDiagnostics(builder);
                RegisterScheduleStorage(builder);
                RegisterTimeline(builder);
                RegisterScheduleExecutors(builder);
                RegisterPartStorage(builder);
                RegisterDatasetLock(builder);
                RegisterAssemblyResolver(builder);
            }

            result = builder.Build();
            return result;
        }

        private static void CloseApplication(IContainer container)
        {
            var context = container.Resolve<ApplicationContext>();
            
            container.Dispose();
            context.ExitThread();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fileToLoad",
            Justification = "Leaving this for now. At least until we can actually load stuff from file.")]
        private static void LoadDatasetFile(IContainer container, FileInfo fileToLoad)
        {
            // For now we fake this out by pretending it takes time to load.
            var progressAction = container.Resolve<IDatasetApplicationNotificationInvoker>();
            progressAction.RaiseOnProgress(0, "Starting dataset load ...");

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(33, "Loading ...");

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(66, "Loading ...");

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(100, "Load complete");
        }
    }
}
