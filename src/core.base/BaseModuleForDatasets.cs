//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Utilities;
using Autofac;
using Lokad;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registration for the dataset components for 
    /// applications that load and manipulate datasets.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class BaseModuleForDatasets : Module
    {
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

        private static void RegisterCommandCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalCommandCollection(
                    c.Resolve<ICommunicationLayer>()))
                .As<ICommandCollection>()
                .SingleInstance();
        }

        private static void RegisterNotificationCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalNotificationCollection(
                    c.Resolve<ICommunicationLayer>()))
                .As<INotificationSendersCollection>()
                .As<ISendNotifications>()
                .SingleInstance();
        }

        private static void RegisterMessageProcessingActions(ContainerBuilder builder)
        {
            builder.Register(
                   c =>
                   {
                       // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                       // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                       var ctx = c.Resolve<IComponentContext>();
                       return new CommandInformationRequestProcessAction(
                           EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                           (endpoint, msg) => ctx.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                           c.Resolve<ICommandCollection>(),
                           c.Resolve<SystemDiagnostics>());
                   })
               .As<IMessageProcessAction>();

            builder.Register(
                    c =>
                    {
                        // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                        // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                        var ctx = c.Resolve<IComponentContext>();
                        return new CommandInvokedProcessAction(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            (endpoint, msg) => ctx.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                            c.Resolve<ICommandCollection>(),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IMessageProcessAction>();

            builder.Register(
                    c =>
                    {
                        // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                        // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                        var ctx = c.Resolve<IComponentContext>();
                        return new NotificationInformationRequestProcessAction(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            (endpoint, msg) => ctx.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                            c.Resolve<INotificationSendersCollection>(),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IMessageProcessAction>();

            builder.Register(c => new RegisterForNotificationProcessAction(
                    c.Resolve<ISendNotifications>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new UnregisterFromNotificationProcessAction(
                    c.Resolve<ISendNotifications>()))
                .As<IMessageProcessAction>();
        }

        /// <summary>
        /// The action that is used to close the dataset application.
        /// </summary>
        private readonly Action m_CloseDatasetAction;

        /// <summary>
        /// The action that is used to load a dataset.
        /// </summary>
        private readonly Action<FileInfo> m_LoadDatasetAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModuleForDatasets"/> class.
        /// </summary>
        /// <param name="closeDataset">The action that is used to close the dataset application.</param>
        /// <param name="loadDataset">The action that is used to load a dataset application.</param>
        public BaseModuleForDatasets(
            Action closeDataset,
            Action<FileInfo> loadDataset)
        {
            {
                Enforce.Argument(() => closeDataset);
                Enforce.Argument(() => loadDataset);
            }

            m_CloseDatasetAction = closeDataset;
            m_LoadDatasetAction = loadDataset;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterCommands(builder);
            RegisterNotifications(builder);
            RegisterCommandCollection(builder);
            RegisterNotificationCollection(builder);
            RegisterMessageProcessingActions(builder);
        }

        private void RegisterCommands(ContainerBuilder builder)
        {
            builder.Register(c => new DatasetApplicationCommands(
                    c.Resolve<ICommunicationLayer>(),
                    m_CloseDatasetAction,
                    m_LoadDatasetAction))
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<ICommandCollection>();
                        collection.Register(typeof(IDatasetApplicationCommands), a.Instance);
                    })
                .As<IDatasetApplicationCommands>()
                .As<ICommandSet>()
                .SingleInstance();
        }
    }
}
