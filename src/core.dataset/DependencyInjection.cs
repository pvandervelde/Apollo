//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Windows.Forms;
using Apollo.Core.Base;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Dataset.Nuclei;
using Apollo.Core.Dataset.Plugins;
using Apollo.Core.Dataset.Scheduling;
using Apollo.Core.Dataset.Scheduling.Processors;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities.History;
using Autofac;
using Nuclei.Communication;
using Nuclei.Diagnostics;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Initializes the dependency injection system.
    /// </summary>
    internal static class DependencyInjection
    {
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
                builder.RegisterModule(new NucleiModule());

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
                        false));
                
                RegisterCommands(
                    builder,
                    () => CloseApplication(result),
                    file => LoadDatasetFile(result, file));
                RegisterNotifications(builder);

                builder.Register(c => context)
                    .As<ApplicationContext>()
                    .ExternallyOwned();

                RegisterScheduleStorage(builder);
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
            progressAction.RaiseOnProgress(0, new StreamLoadProgressMark());

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(33, new StreamLoadProgressMark());

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(66, new StreamLoadProgressMark());

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(100, new StreamLoadProgressMark());
        }
    }
}
