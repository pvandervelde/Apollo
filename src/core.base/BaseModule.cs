//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using Autofac;
using Autofac.Core;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registrations for the communication and loader components.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class BaseModule : Module
    {
        private static void AttachMessageProcessingActions(IActivatedEventArgs<MessageHandler> args)
        {
            var handler = (MessageHandler)args.Instance;
            var filterActions = args.Context.Resolve<IEnumerable<IMessageProcessAction>>();
            foreach (var action in filterActions)
            {
                handler.ActOnArrival(
                   new MessageKindFilter(action.MessageTypeToProcess),
                   action);
            }
        }

        private static void ConnectToMessageHandler(IActivatedEventArgs<ICommunicationChannel> args, Type key)
        {
            var handler = args.Context.ResolveKeyed<IProcessIncomingMessages>(key);
            args.Instance.OnReceive += (s, e) => handler.ProcessMessage(e.Message);
            args.Instance.OnClosed += (s, e) => handler.OnLocalChannelClosed();
        }

        private static void RegisterUtilities(ContainerBuilder builder)
        {
            builder.Register(c => new CommunicationConstants())
                .As<ICommunicationConstants>();
        }

        private static void RegisterCommandHub(ContainerBuilder builder)
        {
            builder.Register(c => new RemoteCommandHub(
                    c.Resolve<ICommunicationLayer>(),
                    c.ResolveKeyed<IReportNewProxies>(typeof(ICommandSet)),
                    c.Resolve<CommandProxyBuilder>(),
                    c.Resolve<Action<LogSeverityProxy, string>>()))
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
                        c.Resolve<Action<LogSeverityProxy, string>>());
                });

            builder.Register(c => new LocalCommandCollection(
                    c.Resolve<ICommunicationLayer>()))
                .As<ICommandCollection>()
                .SingleInstance();
        }

        private static void RegisterNotificationHub(ContainerBuilder builder)
        {
            builder.Register(c => new RemoteNotificationHub(
                    c.Resolve<ICommunicationLayer>(),
                    c.ResolveKeyed<IReportNewProxies>(typeof(INotificationSet)),
                    c.Resolve<NotificationProxyBuilder>(),
                    c.Resolve<Action<LogSeverityProxy, string>>()))
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
                            return ctx.Resolve<ICommunicationLayer>().SendMessageAndWaitForResponse(endpoint, msg);
                        },
                        c.Resolve<Action<LogSeverityProxy, string>>());
                });

            builder.Register(c => new LocalNotificationCollection(
                    c.Resolve<ICommunicationLayer>()))
                .As<ICommandCollection>()
                .SingleInstance();
        }

        private static void RegisterCommunicationLayer(ContainerBuilder builder)
        {
            builder.Register(
                c => 
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommunicationLayer(
                        c.Resolve<IEnumerable<IDiscoverOtherServices>>(),
                        (Type t, EndpointId id) =>
                        {
                            return Tuple.Create(
                                ctx.ResolveKeyed<ICommunicationChannel>(t, new TypedParameter(typeof(EndpointId), id)),
                                ctx.ResolveKeyed<IDirectIncomingMessages>(t));
                        },
                        c.Resolve<Action<LogSeverityProxy, string>>());
                })
                .As<ICommunicationLayer>()
                .SingleInstance();
        }

        private static void RegisterEndpointDiscoverySources(ContainerBuilder builder, bool allowChannelDiscovery)
        {
            if (allowChannelDiscovery)
            {
                builder.Register(c => new TcpBasedDiscoverySource())
                    .As<IDiscoverOtherServices>();
            }

            // For now we're marking this as a single instance because
            // we want it to be linked to the CommunicationLayer at all times
            // and yet we want to be able to give it out to users without 
            // having to worry if we have given out the correct instance. Maybe
            // there is a cleaner solution to this problem though ...
            builder.Register(c => new ManualDiscoverySource())
                .As<IDiscoverOtherServices>()
                .As<IAcceptExternalEndpointInformation>()
                .SingleInstance();

            // This function is used to resolve connection information from
            // a set of strings.
            builder.Register<Action<string, string, string>>(
                c =>
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return (id, channelType, address) =>
                    {
                        ctx.Resolve<IAcceptExternalEndpointInformation>().RecentlyConnectedEndpoint(
                            EndpointIdExtensions.Deserialize(id),
                            Type.GetType(channelType, null, null, true, false),
                            new Uri(address));
                    };
                });
        }

        private static void RegisterCommandDiscoverySources(ContainerBuilder builder)
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

        private static void RegisterMessageHandler(ContainerBuilder builder)
        {
            // Note that there is no direct relation between the IChannelType and the MessageHandler
            // however every CommunicationChannel needs exactly one MessageHandler attached ... Hence
            // we pretend that there is a connection between IChannelType and the MessageHandler.
            builder.Register(c => new MessageHandler(
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .OnActivated(a =>
                {
                    AttachMessageProcessingActions(a);
                })
                .Keyed<IProcessIncomingMessages>(typeof(NamedPipeChannelType))
                .Keyed<IDirectIncomingMessages>(typeof(NamedPipeChannelType))
                .SingleInstance();

            builder.Register(c => new MessageHandler(
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .OnActivated(a =>
                {
                    AttachMessageProcessingActions(a);
                })
                .Keyed<IProcessIncomingMessages>(typeof(TcpChannelType))
                .Keyed<IDirectIncomingMessages>(typeof(TcpChannelType))
                .SingleInstance();
        }

        private static void RegisterMessageProcessingActions(ContainerBuilder builder)
        {
            // For now we'll just create two extra objects only to get their types
            // and then throw those objects away. If this turns out to be too expensive
            // or the list becomes too long then we can do something cunning with the 
            // use of Autofac Metadata.
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
                        c.Resolve<Action<LogSeverityProxy, string>>());
                })
                .As<IMessageProcessAction>();

            builder.Register(c => new DataDownloadProcessAction(
                    c.Resolve<WaitingUploads>(),
                    c.Resolve<ICommunicationLayer>(),
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .As<IMessageProcessAction>();

            builder.Register(
                c => 
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new EndpointConnectProcessAction(
                        c.Resolve<IAcceptExternalEndpointInformation>(),
                        from channelType in ctx.Resolve<IEnumerable<IChannelType>>() select channelType.GetType(),
                        c.Resolve<Action<LogSeverityProxy, string>>());
                })
                .As<IMessageProcessAction>();

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
                        c.Resolve<Action<LogSeverityProxy, string>>());
                })
                .As<IMessageProcessAction>();

            builder.Register(c => new NewCommandRegisteredProcessAction(
                    c.ResolveKeyed<IAcceptExternalProxyInformation>(typeof(ICommandSet))))
                .As<IMessageProcessAction>();

            builder.Register(c => new RegisterForNotificationProcessAction(
                    c.Resolve<ISendNotifications>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new UnregisterFromNotificationProcessAction(
                    c.Resolve<ISendNotifications>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new NewNotificationRegisteredProcessAction(
                    c.ResolveKeyed<IAcceptExternalProxyInformation>(typeof(INotificationSet))))
                .As<IMessageProcessAction>();

            builder.Register(c => new NotificationRaisedProcessAction(
                    c.Resolve<INotifyOfRemoteEndpointEvents>(),
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .As<IMessageProcessAction>();
        }

        private static void RegisterCommunicationChannel(ContainerBuilder builder)
        {
            // CommunicationChannel.
            // Register one channel for each communication type. At the moment
            // that is only named pipe and TCP.
            builder.Register(
                (c, p) => 
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommunicationChannel(
                        p.TypedAs<EndpointId>(),
                        c.Resolve<NamedPipeChannelType>(),
                        () => ctx.Resolve<IMessagePipe>(),
                        endpointToProxy =>
                        {
                            return ctx.Resolve<ISendingEndpoint>(
                                new TypedParameter(
                                    typeof(Func<EndpointId, IChannelProxy>),
                                    endpointToProxy));
                        },
                        () => DateTimeOffset.Now,
                        c.Resolve<Action<LogSeverityProxy, string>>());
                })
                .OnActivated(a =>
                {
                    ConnectToMessageHandler(a, typeof(NamedPipeChannelType));
                })
                .Keyed<ICommunicationChannel>(typeof(NamedPipeChannelType))
                .SingleInstance();

            builder.Register(
                (c, p) => 
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommunicationChannel(
                        p.TypedAs<EndpointId>(),
                        c.Resolve<TcpChannelType>(),
                        () => ctx.Resolve<IMessagePipe>(),
                        endpointToProxy =>
                        {
                            return ctx.Resolve<ISendingEndpoint>(
                                new TypedParameter(
                                    typeof(Func<EndpointId, IChannelProxy>),
                                    endpointToProxy));
                        },
                        () => DateTimeOffset.Now,
                        c.Resolve<Action<LogSeverityProxy, string>>());
                })
                .OnActivated(a =>
                {
                    ConnectToMessageHandler(a, typeof(TcpChannelType));
                })
                .Keyed<ICommunicationChannel>(typeof(TcpChannelType))
                .SingleInstance();
        }

        private static void RegisterEndpoints(ContainerBuilder builder)
        {
            builder.Register((c, p) => new SendingEndpoint(
                    p.TypedAs<Func<EndpointId, IChannelProxy>>()))
                .As<ISendingEndpoint>();

            builder.Register(c => new ReceivingEndpoint(
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .As<IMessagePipe>();
        }

        private static void RegisterChannelTypes(ContainerBuilder builder, bool allowChannelDiscovery)
        {
            builder.Register(c => new NamedPipeChannelType(
                    c.Resolve<IConfiguration>()))
                .As<IChannelType>()
                .As<NamedPipeChannelType>();

            builder.Register(c => new TcpChannelType(
                    c.Resolve<IConfiguration>(),
                    allowChannelDiscovery))
                .As<IChannelType>()
                .As<TcpChannelType>();
        }

        private static void RegisterUploads(ContainerBuilder builder)
        {
            builder.Register(c => new WaitingUploads())
                .SingleInstance();
        }

        private static void RegisterStartables(ContainerBuilder builder)
        {
            builder.Register(c => new CommunicationLayerStarter(
                    c.Resolve<ICommunicationLayer>()))
                .As<IStartable>();
        }

        /// <summary>
        /// Indicates if the communication channels are allowed to provide discovery.
        /// </summary>
        private readonly bool m_AllowChannelDiscovery;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModule"/> class.
        /// </summary>
        /// <param name="allowChannelDiscovery">
        ///     A flag that indicates if the communication channels are allowed to provide
        ///     discovery.
        /// </param>
        public BaseModule(bool allowChannelDiscovery)
            : base()
        {
            m_AllowChannelDiscovery = allowChannelDiscovery;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterUtilities(builder);
            RegisterCommandHub(builder);
            RegisterNotificationHub(builder);
            RegisterCommunicationLayer(builder);
            RegisterEndpointDiscoverySources(builder, m_AllowChannelDiscovery);
            RegisterCommandDiscoverySources(builder);
            RegisterMessageHandler(builder);
            RegisterMessageProcessingActions(builder);
            RegisterCommunicationChannel(builder);
            RegisterEndpoints(builder);
            RegisterChannelTypes(builder, m_AllowChannelDiscovery);
            RegisterUploads(builder);
            RegisterStartables(builder);
        }
    }
}
