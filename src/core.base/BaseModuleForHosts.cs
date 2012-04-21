//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using Autofac;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registration for the loader components for 
    /// applications that initiate the loading of datasets in local or
    /// remote applications.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class BaseModuleForHosts : Module
    {
        private static void RegisterDistributors(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new RemoteDatasetDistributor(
                        c.Resolve<ISendCommandsToRemoteEndpoints>(),
                        c.Resolve<INotifyOfRemoteEndpointEvents>(),
                        c.Resolve<IConfiguration>(),
                        c.Resolve<WaitingUploads>(),
                        (dataset, endpoint, network) =>
                        {
                            return new DatasetOnlineInformation(
                                dataset,
                                endpoint,
                                network,
                                ctx.Resolve<ISendCommandsToRemoteEndpoints>(),
                                ctx.Resolve<SystemDiagnostics>());
                        },
                        () =>
                        {
                            return (from connection in ctx.Resolve<ICommunicationLayer>().LocalConnectionPoints()
                                    where connection.ChannelType.Equals(typeof(TcpChannelType))
                                    select connection).First();
                        },
                        c.Resolve<SystemDiagnostics>());
                })
                .As<IGenerateDistributionProposals>()
                .As<ILoadDatasets>()
                .SingleInstance();

            builder.Register(
                c =>
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new LocalDatasetDistributor(
                       c.Resolve<ICalculateDistributionParameters>(),
                       c.Resolve<IApplicationLoader>(),
                       c.Resolve<ISendCommandsToRemoteEndpoints>(),
                       c.Resolve<INotifyOfRemoteEndpointEvents>(),
                       c.Resolve<WaitingUploads>(),
                       (dataset, endpoint, network) =>
                       {
                           return new DatasetOnlineInformation(
                               dataset,
                               endpoint,
                               network,
                               ctx.Resolve<ISendCommandsToRemoteEndpoints>(),
                               ctx.Resolve<SystemDiagnostics>());
                       },
                       () =>
                       {
                           return (from connection in ctx.Resolve<ICommunicationLayer>().LocalConnectionPoints()
                                   where connection.ChannelType.Equals(typeof(NamedPipeChannelType))
                                   select connection).First();
                       },
                       c.Resolve<SystemDiagnostics>());
                })
                .As<IGenerateDistributionProposals>()
                .As<ILoadDatasets>()
                .SingleInstance();
        }

        private static void RegisterCommandHub(ContainerBuilder builder)
        {
            builder.Register(c => new RemoteCommandHub(
                    c.Resolve<ICommunicationLayer>(),
                    c.ResolveKeyed<IReportNewProxies>(typeof(ICommandSet)),
                    c.Resolve<CommandProxyBuilder>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<ISendCommandsToRemoteEndpoints>()
                .SingleInstance();

            builder.Register(
                c =>
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommandProxyBuilder(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        (endpoint, msg) =>
                        {
                            return ctx.Resolve<ICommunicationLayer>().SendMessageAndWaitForResponse(endpoint, msg);
                        },
                        c.Resolve<SystemDiagnostics>());
                });
        }

        private static void RegisterNotificationHub(ContainerBuilder builder)
        {
            builder.Register(c => new RemoteNotificationHub(
                    c.Resolve<ICommunicationLayer>(),
                    c.ResolveKeyed<IReportNewProxies>(typeof(INotificationSet)),
                    c.Resolve<NotificationProxyBuilder>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<INotifyOfRemoteEndpointEvents>()
                .SingleInstance();

            builder.Register(
                c => 
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new NotificationProxyBuilder(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        (endpoint, msg) =>
                        {
                            ctx.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg);
                        },
                        c.Resolve<SystemDiagnostics>());
                });
        }

        private static void RegisterProxyDiscoverySources(ContainerBuilder builder)
        {
            builder.Register(c => new ManualProxyRegistrationReporter())
                .Keyed<IAcceptExternalProxyInformation>(typeof(ICommandSet))
                .Keyed<IReportNewProxies>(typeof(ICommandSet))
                .SingleInstance();

            builder.Register(c => new ManualProxyRegistrationReporter())
                .Keyed<IAcceptExternalProxyInformation>(typeof(INotificationSet))
                .Keyed<IReportNewProxies>(typeof(INotificationSet))
                .SingleInstance();
        }

        private static void RegisterMessageProcessingActions(ContainerBuilder builder)
        {
            builder.Register(c => new NewCommandRegisteredProcessAction(
                    c.ResolveKeyed<IAcceptExternalProxyInformation>(typeof(ICommandSet))))
                .As<IMessageProcessAction>();

            builder.Register(c => new NewNotificationRegisteredProcessAction(
                    c.ResolveKeyed<IAcceptExternalProxyInformation>(typeof(INotificationSet))))
                .As<IMessageProcessAction>();

            builder.Register(c => new NotificationRaisedProcessAction(
                    c.Resolve<INotifyOfRemoteEndpointEvents>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessageProcessAction>();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterDistributors(builder);
            RegisterCommandHub(builder);
            RegisterNotificationHub(builder);
            RegisterProxyDiscoverySources(builder);
            RegisterMessageProcessingActions(builder);
        }
    }
}
