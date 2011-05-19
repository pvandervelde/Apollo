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
    [ExcludeFromCodeCoverage()]
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

        private static void RegisterCommunicationComponents(ContainerBuilder builder)
        {
            builder.Register(c => new RemoteCommandHub(
                    c.Resolve<ICommunicationLayer>(),
                    c.Resolve<CommandProxyBuilder>(),
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .SingleInstance();

            builder.Register(c => new CommandProxyBuilder(
                c.Resolve<ICommunicationLayer>().Id,
                (endpoint, msg) =>
                    {
                        return c.Resolve<ICommunicationLayer>().SendMessageAndWaitForResponse(endpoint, msg);
                    }));

            builder.Register(c => new LocalCommandCollection())
                .As<ICommandCollection>()
                .SingleInstance();

            builder.Register(c => new CommunicationLayer(
                    c.Resolve<IEnumerable<IDiscoverOtherServices>>(),
                    (Type t, EndpointId id) => 
                    { 
                        return Tuple.Create(
                            c.ResolveKeyed<ICommunicationChannel>(t, new TypedParameter(typeof(EndpointId), id)),
                            c.ResolveKeyed<IDirectIncomingMessages>(t)); 
                    },
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .As<ICommunicationLayer>()
                .SingleInstance();

            builder.Register(c => new TcpBasedDiscoverySource())
                .As<IDiscoverOtherServices>();

            // For now we're marking this as a single instance because
            // we want it to be linked to the CommunicationLayer at all times
            // and yet we want to be able to give it out to users without 
            // having to worry if we have given out the correct instance. Maybe
            // there is a cleaner solution to this problem though ...
            builder.Register(c => new ManualDiscoverySource())
                .As<IDiscoverOtherServices>()
                .As<IAcceptExternalEndpointInformation>()
                .SingleInstance();

            // MessageHandler
            // Note that there is no direct relation between the IChannelType and the MessageHandler
            // however every CommunicationChannel needs exactly one MessageHandler attached ... Hence
            // we pretend that there is a connection between IChannelType and the MessageHandler.
            {
                builder.Register(c => new MessageHandler())
                    .OnActivated(a =>
                    {
                        AttachMessageProcessingActions(a);
                    })
                    .Keyed<IProcessIncomingMessages>(typeof(NamedPipeChannelType))
                    .Keyed<IDirectIncomingMessages>(typeof(NamedPipeChannelType))
                    .SingleInstance();

                builder.Register(c => new MessageHandler())
                    .OnActivated(a =>
                        {
                            AttachMessageProcessingActions(a);
                        })
                    .Keyed<IProcessIncomingMessages>(typeof(TcpChannelType))
                    .Keyed<IDirectIncomingMessages>(typeof(TcpChannelType))
                    .SingleInstance();
            }

            // IMessageProcessAction(s)
            //
            // For now we'll just create two extra objects only to get their types
            // and then throw those objects away. If this turns out to be too expensive
            // or the list becomes too long then we can do something cunning with the 
            // use of Autofac Metadata.
            builder.Register(c => new EndpointConnectProcessAction(
                    c.Resolve<IAcceptExternalEndpointInformation>(),
                    from channelType in c.Resolve<IEnumerable<IChannelType>>() select channelType.GetType(),
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new CommandInvokedProcessAction(
                    c.Resolve<ICommunicationLayer>().Id,
                    (endpoint, msg) => c.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                    c.Resolve<ICommandCollection>(),
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new EndpointInformationRequestProcessAction(
                    c.Resolve<ICommunicationLayer>().Id,
                    (endpoint, msg) => c.Resolve<ICommunicationLayer>().SendMessageTo(endpoint, msg),
                    c.Resolve<ICommandCollection>(),
                    c.Resolve<Action<LogSeverityProxy, string>>()))
                .As<IMessageProcessAction>();
            
            // CommunicationChannel.
            // Register one channel for each communication type. At the moment
            // that is only named pipe and TCP.
            {
                builder.Register((c, p) => new CommunicationChannel(
                        p.TypedAs<EndpointId>(),
                        c.Resolve<NamedPipeChannelType>(),
                        () => c.Resolve<IMessagePipe>(),
                        endpointToProxy =>
                        {
                            return c.Resolve<ISendingEndpoint>(
                                new TypedParameter(
                                    typeof(Func<EndpointId, IChannelProxy>),
                                    endpointToProxy));
                        },
                        c.Resolve<Action<LogSeverityProxy, string>>()))
                    .OnActivated(a =>
                        {
                            ConnectToMessageHandler(a, typeof(NamedPipeChannelType));
                        })
                    .Keyed<ICommunicationChannel>(typeof(NamedPipeChannelType))
                    .SingleInstance();

                builder.Register((c, p) => new CommunicationChannel(
                        p.TypedAs<EndpointId>(),
                        c.Resolve<TcpChannelType>(),
                        () => c.Resolve<IMessagePipe>(),
                        endpointToProxy =>
                        {
                            return c.Resolve<ISendingEndpoint>(
                                new TypedParameter(
                                    typeof(Func<EndpointId, IChannelProxy>),
                                    endpointToProxy));
                        },
                        c.Resolve<Action<LogSeverityProxy, string>>()))
                    .OnActivated(a =>
                        {
                            ConnectToMessageHandler(a, typeof(TcpChannelType));
                        })
                    .Keyed<ICommunicationChannel>(typeof(TcpChannelType))
                    .SingleInstance();
            }

            builder.Register((c, p) => new SendingEndpoint(
                    p.TypedAs<Func<EndpointId, IChannelProxy>>()))
                .As<ISendingEndpoint>();

            builder.Register(c => new ReceivingEndpoint())
                .As<IMessagePipe>();

            builder.Register(c => new NamedPipeChannelType(
                    c.Resolve<IConfiguration>()))
                .As<IChannelType>()
                .As<NamedPipeChannelType>();

            builder.Register(c => new TcpChannelType(
                    c.Resolve<IConfiguration>()))
                .As<IChannelType>()
                .As<TcpChannelType>();
        }

        private static void RegisterLoaderComponents(ContainerBuilder builder)
        {
            // DatasetLoader
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterCommunicationComponents(builder);
            RegisterLoaderComponents(builder);
        }
    }
}
