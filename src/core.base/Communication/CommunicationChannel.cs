//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods required for handling communication with other Apollo applications 
    /// across the network.
    /// </summary>
    /// <remarks>
    /// The design of this class assumes that there is only one of these active for a given
    /// channel address (e.g. net.tcp://my_machine:7000/apollo) at any given time.
    /// This is because the current class has a receiving endpoint of which there can only 
    /// be one. If there are multiple communication channels sharing the receiving endpoint then
    /// we don't know which channel should get the messages.
    /// </remarks>
    internal sealed class CommunicationChannel : ICommunicationChannel
    {
        /// <summary>
        /// Maps the endpoint to the connection information.
        /// </summary>
        private readonly Dictionary<EndpointId, ChannelConnectionInformation> m_ChannelConnectionMap =
            new Dictionary<EndpointId, ChannelConnectionInformation>();

        /// <summary>
        /// The ID number of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Id;

        /// <summary>
        /// Indicates the type of channel that we're dealing with and provides
        /// utility methods for the channel.
        /// </summary>
        private readonly IChannelType m_Type;

        /// <summary>
        /// The function that generates receiving endpoints.
        /// </summary>
        private readonly Func<IMessagePipe> m_ReceiverBuilder;

        /// <summary>
        /// The function that generates sending endpoints.
        /// </summary>
        private readonly Func<Func<EndpointId, IChannelProxy>, ISendingEndpoint> m_SenderBuilder;

        /// <summary>
        /// The object used to receive messages over the network.
        /// </summary>
        private IMessagePipe m_Receiver;

        /// <summary>
        /// The object used to send messages over the network.
        /// </summary>
        private ISendingEndpoint m_Sender;

        /// <summary>
        /// The host that maintains the network connection.
        /// </summary>
        private ServiceHost m_Host;

        /// <summary>
        /// The event handler used when the host faults.
        /// </summary>
        private EventHandler m_HostFaultingHandler;

        /// <summary>
        /// The message handler that is used to receive messages from the
        /// receiving endpoint.
        /// </summary>
        private EventHandler<MessageEventArgs> m_MessageReceivingHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationChannel"/> class.
        /// </summary>
        /// <param name="id">The ID number of the current endpoint.</param>
        /// <param name="channelType">The type of channel, e.g. TCP.</param>
        /// <param name="receiverBuilder">The function that builds receiving endpoints.</param>
        /// <param name="senderBuilder">The function that builds sending endpoints.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="receiverBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="senderBuilder"/> is <see langword="null" />.
        /// </exception>
        public CommunicationChannel(
            EndpointId id, 
            IChannelType channelType, 
            Func<IMessagePipe> receiverBuilder,
            Func<Func<EndpointId, IChannelProxy>, ISendingEndpoint> senderBuilder)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => channelType);
                Enforce.Argument(() => receiverBuilder);
                Enforce.Argument(() => senderBuilder);
            }

            m_Id = id;
            m_Type = channelType;
            m_ReceiverBuilder = receiverBuilder;
            m_SenderBuilder = senderBuilder;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the channel.
        /// </summary>
        public EndpointId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Opens the channel and provides information on how to connect to the given channel.
        /// </summary>
        /// <returns>
        /// The information describing how to connect to the current channel.
        /// </returns>
        public ChannelConnectionInformation OpenChannel()
        {
            if (m_Receiver != null)
            {
                Close();
            }

            m_Receiver = m_ReceiverBuilder();
            m_MessageReceivingHandler = (s, e) => RaiseOnReceive(e.Message);
            m_Receiver.OnNewMessage += m_MessageReceivingHandler;

            var uri = m_Type.GenerateNewChannelUri();
            var address = m_Type.GenerateNewAddress();
            ReopenChannel(uri, address);

            return new ChannelConnectionInformation(Id, m_Type.GetType(), uri, address);
        }

        private void ReopenChannel(Uri uri, string address)
        {
            // Clear the old host
            m_Host.Faulted -= m_HostFaultingHandler;
            m_Host = null;

            // Create the new host
            m_HostFaultingHandler = (s, e) =>
                {
                    ReopenChannel(uri, address);
                };
            m_Host = new ServiceHost(m_Receiver, uri);
            m_Host.Faulted += m_HostFaultingHandler;

            var binding = m_Type.GenerateBinding();
            m_Host.AddServiceEndpoint(typeof(IReceivingEndpoint), binding, address);

            m_Host.Open();
        }

        /// <summary>
        /// Closes the current channel.
        /// </summary>
        public void Close()
        {
            if (m_Sender != null)
            {
                // First notify the recipients that we're closing the channel.
                var knownEndpoints = new List<EndpointId>(m_Sender.KnownEndpoints());
                foreach (var key in knownEndpoints)
                {
                    var msg = new EndpointDisconnectMessage(Id);
                    m_Sender.Send(key, msg);
                }

                // Then close the channel. We'll do this in a different
                // loop to give the channels time to process the messages.
                foreach (var key in knownEndpoints)
                {
                    m_Sender.CloseChannelTo(key);
                }
            }

            if (m_Host != null)
            {
                m_Host.Close();
                m_Host.Faulted -= m_HostFaultingHandler;
                m_HostFaultingHandler = null;
                m_Host = null;
            }

            if (m_Receiver != null)
            {
                m_Receiver.OnNewMessage -= m_MessageReceivingHandler;
                m_Receiver = null;
            }

            RaiseOnClosed();
        }

        /// <summary>
        /// Connects to a channel by using the information provided in the connection information
        /// object.
        /// </summary>
        /// <param name="connection">The information necessary to connect to the remote channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="connection"/> is <see langword="null" />.
        /// </exception>
        public void ConnectTo(ChannelConnectionInformation connection)
        {
            {
                Enforce.Argument(() => connection);
            }

            if (!m_ChannelConnectionMap.ContainsKey(connection.Id))
            {
                m_ChannelConnectionMap.Add(connection.Id, null);
            }

            m_ChannelConnectionMap[connection.Id] = connection;
        }

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint from which the channel needs to disconnect.</param>
        public void DisconnectFrom(EndpointId endpoint)
        {
            if (m_ChannelConnectionMap.ContainsKey(endpoint))
            {
                if (m_Sender != null)
                {
                    m_Sender.CloseChannelTo(endpoint);
                }

                m_ChannelConnectionMap.Remove(endpoint);
            }
        }

        /// <summary>
        /// Creates the required channel(s) to receive a data stream across the network and returns
        /// the connection information and the task responsible for handling the data reception.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="localFile"/> does not exist a new file will be created with the given path. If
        /// it does exist then the data will be appended to it.
        /// </remarks>
        /// <param name="localFile">The full file path to which the network stream should be written.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// The connection information necessary to connect to the newly created channel and the task 
        /// responsible for handling the data reception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localFile"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="localFile"/> is an empty string.
        /// </exception>
        public Tuple<StreamTransferInformation, Task<FileInfo>> PrepareForDataReception(string localFile, CancellationToken token)
        {
            {
                Enforce.Argument(() => localFile);
                Enforce.With<ArgumentException>(!string.IsNullOrWhiteSpace(localFile), Resources.Exceptions_Messages_FilePathCannotBeEmpty);
            }

            return m_Type.PrepareForDataReception(localFile, token);
        }

        /// <summary>
        /// Transfers the data to the receiving endpoint.
        /// </summary>
        /// <param name="filePath">The file path to the file that should be transferred.</param>
        /// <param name="transferInformation">
        /// The information which describes the data to be transferred and the remote connection over
        /// which the data is transferred.
        /// </param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="transferInformation"/> is <see langword="null" />.
        /// </exception>
        public Task TransferData(string filePath, StreamTransferInformation transferInformation, CancellationToken token)
        {
            {
                Enforce.Argument(() => filePath);
                Enforce.With<ArgumentException>(!string.IsNullOrWhiteSpace(filePath), Resources.Exceptions_Messages_FilePathCannotBeEmpty);
                Enforce.Argument(() => transferInformation);
            }

            return m_Type.TransferData(filePath, transferInformation, token);
        }

        /// <summary>
        /// Sends the given message to the receiving endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="message">The message that should be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public void Send(EndpointId endpoint, ICommunicationMessage message)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.Argument(() => message);
            }

            if (m_Sender == null)
            {
                m_Sender = m_SenderBuilder(BuildChannelProxy);
            }

            m_Sender.Send(endpoint, message);
        }

        private IChannelProxy BuildChannelProxy(EndpointId id)
        {
            Debug.Assert(m_ChannelConnectionMap.ContainsKey(id), "Trying to send a message to an unknown endpoint.");
            var connectionInfo = m_ChannelConnectionMap[id];
            var uri = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", connectionInfo.ChannelBaseUri, connectionInfo.EndpointSubAddress);
            var endpoint = new EndpointAddress(uri);

            Debug.Assert(m_Type.GetType().Equals(connectionInfo.ChannelType), "Trying to connect to a channel with a different binding type.");
            var binding = m_Type.GenerateBinding();

            var factory = new ChannelFactory<IReceivingWcfEndpointProxy>(binding, endpoint);
            return new SelfResurrectingSendingEndpoint(factory);
        }

        /// <summary>
        /// An event raised when a new message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnReceive;

        private void RaiseOnReceive(ICommunicationMessage message)
        {
            var local = OnReceive;
            if (local != null)
            {
                local(this, new MessageEventArgs(message));
            }
        }

        /// <summary>
        /// An event raised when the other side of the connection is closed.
        /// </summary>
        public event EventHandler<ChannelClosedEventArgs> OnClosed;

        private void RaiseOnClosed()
        {
            var local = OnClosed;
            if (local != null)
            {
                local(this, new ChannelClosedEventArgs(Id));
            }
        }
    }
}