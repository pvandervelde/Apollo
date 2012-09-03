//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
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
                        c.Resolve<SystemDiagnostics>());
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
                        // We need to make sure that the communication layer is ready to start
                        // sending / receiving messages, otherwise the we won't be able to
                        // tell it that there are new endpoints.
                        //
                        // NOTE: this is kinda yucky because really we're only interested in
                        // the IAcceptExternalEndpointInformation object and its willingness
                        // to process information. However the only way that object is going to
                        // be willing is if the communication layer is signed in so ...
                        var layer = ctx.Resolve<ICommunicationLayer>();
                        if (!layer.IsSignedIn)
                        {
                            var resetEvent = new AutoResetEvent(false);
                            var availability =
                                Observable.FromEventPattern<EventArgs>(
                                    h => layer.OnSignedIn += h,
                                    h => layer.OnSignedIn -= h)
                                .Take(1)
                                .Subscribe(
                                    args =>
                                    {
                                        resetEvent.Set();
                                    });

                            using (availability)
                            {
                                if (!layer.IsSignedIn)
                                {
                                    resetEvent.WaitOne();
                                }
                            }
                        }

                        var remoteEndpoint = EndpointIdExtensions.Deserialize(id);
                        ctx.Resolve<IAcceptExternalEndpointInformation>().RecentlyConnectedEndpoint(
                            remoteEndpoint,
                            Type.GetType(channelType, null, null, true, false),
                            new Uri(address));
                        layer.ConnectToEndpoint(remoteEndpoint);
                    };
                });
        }

        private static void RegisterMessageHandler(ContainerBuilder builder)
        {
            // Note that there is no direct relation between the IChannelType and the MessageHandler
            // however every CommunicationChannel needs exactly one MessageHandler attached ... Hence
            // we pretend that there is a connection between IChannelType and the MessageHandler.
            builder.Register(c => new MessageHandler(
                    c.Resolve<SystemDiagnostics>()))
                .OnActivated(a =>
                {
                    AttachMessageProcessingActions(a);
                })
                .Keyed<IProcessIncomingMessages>(typeof(NamedPipeChannelType))
                .Keyed<IDirectIncomingMessages>(typeof(NamedPipeChannelType))
                .SingleInstance();

            builder.Register(c => new MessageHandler(
                    c.Resolve<SystemDiagnostics>()))
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
            builder.Register(c => new DataDownloadProcessAction(
                    c.Resolve<WaitingUploads>(),
                    c.Resolve<ICommunicationLayer>(),
                    c.Resolve<SystemDiagnostics>()))
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
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IMessageProcessAction>();

            builder.Register(
                c =>
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new UnknownMessageTypeProcessAction(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        (endpoint, msg) => ctx.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                        c.Resolve<SystemDiagnostics>());
                })
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
                        c.Resolve<SystemDiagnostics>());
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
                        c.Resolve<SystemDiagnostics>());
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
                    c.Resolve<SystemDiagnostics>()))
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
            RegisterCommunicationLayer(builder);
            RegisterEndpointDiscoverySources(builder, m_AllowChannelDiscovery);
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
