//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Utils;
using Apollo.Utils.Configuration;
using Autofac;
using Autofac.Core;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registrations for the communication and loader components.
    /// </summary>
    [ExcludeFromCoverage("Modules are used for dependency injection purposes. Testing is done through integration testing.")]
    public sealed class BaseModule : Module
    {
        private static void AttachMessageProcessingActions(IActivatedEventArgs<MessageHandler> a)
        {
            var handler = (MessageHandler)a.Instance;
            var filterActions = a.Context.Resolve<IEnumerable<IMessageProcessAction>>();
            foreach (var action in filterActions)
            {
                handler.ActOnArrival(
                   new MessageKindFilter(action.MessageTypeToProcess),
                   action);
            }
        }

        private static void ConnectToCommunicationChannel(IActivatedEventArgs<MessageHandler> a, Type key)
        {
            var channel = a.Context.ResolveKeyed<ICommunicationChannel>(key);
            channel.OnReceive += (s, e) => a.Instance.ProcessMessage(e.Message);
            channel.OnClosed += (s, e) => a.Instance.OnLocalChannelClosed();
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

        private void RegisterCommunicationComponents(ContainerBuilder builder)
        {
            builder.Register(c => new MessageHub(
                    c.Resolve<ICommunicationLayer>(),
                    c.Resolve<CommandProxyBuilder>()))
                .SingleInstance();

            builder.Register(c => new CommandProxyBuilder());

            builder.Register(c => new CommunicationLayer(
                    c.Resolve<IDiscoverOtherServices>(),
                    (Type t) => 
                    { 
                        return Tuple.Create(c.ResolveKeyed<ICommunicationChannel>(t), c.ResolveKeyed<IDirectIncomingMessages>(t)); 
                    }))
                .As<ICommunicationLayer>();

            builder.Register(c => new object())
                .As<IDiscoverOtherServices>();

            // MessageHandler
            // Note that there is no direct relation between the IChannelType and the MessageHandler
            // however every CommunicationChannel needs exactly one MessageHandler attached ... Hence
            {
                builder.Register(c => new MessageHandler())
                    .OnActivated(a =>
                    {
                        AttachMessageProcessingActions(a);
                        ConnectToCommunicationChannel(a, typeof(NamedPipeChannelType));
                    })
                    .AsImplementedInterfaces()
                    .Keyed<ICommunicationChannel>(typeof(NamedPipeChannelType))
                    .SingleInstance();

                builder.Register(c => new MessageHandler())
                    .OnActivated(a =>
                        {
                            AttachMessageProcessingActions(a);
                            ConnectToCommunicationChannel(a, typeof(TcpChannelType));
                        })
                    .AsImplementedInterfaces()
                    .Keyed<ICommunicationChannel>(typeof(TcpChannelType))
                    .SingleInstance();
            }

            // IMessageProcessAction(s)
            //
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
                        }))
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
                        }))
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
                .As<NamedPipeChannelType>();

            builder.Register(c => new TcpChannelType(
                    c.Resolve<IConfiguration>()))
                .As<TcpChannelType>();
        }

        private void RegisterLoaderComponents(ContainerBuilder builder)
        {
            // DatasetLoader
            throw new NotImplementedException();
        }
    }
}
