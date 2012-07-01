//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Dataset.Scheduling;
using Apollo.Core.Dataset.Scheduling.Processors;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Autofac;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Initializes the dependency injection system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class DependencyInjection
    {
        private static void RegisterScheduleCommands(ContainerBuilder builder)
        {
            builder.Register(c => new ScheduleExecutionCommands(
                    c.Resolve<IDistributeScheduleExecutions>(),
                    c.Resolve<ITrackDatasetLocks>()))
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(IScheduleExecutionCommands), a.Instance);
                    })
                .As<IScheduleExecutionCommands>()
                .As<ICommandSet>()
                .SingleInstance();

            builder.Register(c => new ScheduleCreationCommands(
                    c.Resolve<IStoreSchedules>(),
                    c.Resolve<ITrackDatasetLocks>()))
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(IScheduleCreationCommands), a.Instance);
                    })
                .As<IScheduleCreationCommands>()
                .As<ICommandSet>()
                .SingleInstance();

            builder.Register(c => new ScheduleInformationCommands(
                    c.Resolve<IStoreScheduleActions>(),
                    c.Resolve<IStoreScheduleConditions>(),
                    c.Resolve<IStoreSchedules>()))
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(IScheduleInformationCommands), a.Instance);
                    })
                .As<IScheduleInformationCommands>()
                .As<ICommandSet>()
                .SingleInstance();
        }

        private static void RegisterScheduleNotifications(ContainerBuilder builder)
        {
            builder.Register(c => new ScheduleExecutionNotifications())
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<INotificationSendersCollection>();
                        collection.Store(typeof(IScheduleExecutionNotifications), a.Instance);
                    })
                .As<IScheduleExecutionNotifications>()
                .As<IScheduleExecutionNotificationInvoker>()
                .As<INotificationSet>()
                .SingleInstance();
        }

        private static void RegisterScheduleStorage(ContainerBuilder builder)
        {
            builder.Register(c => new ScheduleActionStorage())
                .As<IStoreScheduleActions>();

            builder.Register(c => new ScheduleConditionStorage())
                .As<IStoreScheduleConditions>();

            builder.Register(c => new ScheduleStorage())
                .As<IStoreSchedules>();
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

            builder.Register(c => new NoOpVertexProcessor())
                .As<IProcesExecutableScheduleVertices>();

            builder.Register(c => new SubScheduleVertexProcessor(
                    c.Resolve<IDistributeScheduleExecutions>()))
                .As<IProcesExecutableScheduleVertices>();

            builder.Register(
                    c => 
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new MarkHistoryVertexProcessor(
                            c.Resolve<ITimeline>(),
                            m => 
                            {
                                foobar();
                            });
                    })
                .As<IProcesExecutableScheduleVertices>();

            builder.Register((c, p) => new ScheduleExecutor(
                    c.Resolve<IEnumerable<IProcesExecutableScheduleVertices>>(),
                    c.Resolve<IStoreScheduleConditions>(),
                    p.TypedAs<ExecutableSchedule>(),
                    p.TypedAs<ScheduleExecutionInfo>()))
                .As<IExecuteSchedules>();

            builder.Register(
                    c => 
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new ScheduleDistributor(
                            c.Resolve<IStoreSchedules>(),
                            (s, i) =>
                            {
                                return ctx.Resolve<IExecuteSchedules>(
                                    new TypedParameter(typeof(ExecutableSchedule), s),
                                    new TypedParameter(typeof(ScheduleExecutionInfo), i));
                            });
                    })
                .As<IDistributeScheduleExecutions>()
                .SingleInstance();
        }

        private static void RegisterDatasetLock(ContainerBuilder builder)
        {
            builder.Register(c => new DatasetLock())
                .As<ITrackDatasetLocks>()
                .SingleInstance();
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
                builder.RegisterModule(new UtilitiesModule());
                builder.RegisterModule(new SchedulingModule());

                // Don't allow discovery on the dataset application because:
                // - The dataset application wouldn't know what to do with it anyway
                // - We don't want anybody talking to the application except for the
                //   application that started it.
                builder.RegisterModule(new BaseModule(false));
                builder.RegisterModule(new BaseModuleForDatasets(
                    () => CloseApplication(result),
                    file => LoadDatasetFile(result, file)));

                builder.Register(c => context)
                    .As<ApplicationContext>()
                    .ExternallyOwned();

                RegisterScheduleCommands(builder);
                RegisterScheduleNotifications(builder);
                RegisterScheduleStorage(builder);
                RegisterScheduleExecutors(builder);
                RegisterDatasetLock(builder);

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
