//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Properties;
using Apollo.Utilities;
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
    [ExcludeFromCodeCoverage]
    internal sealed class CommunicationChannel : ICommunicationChannel, IDisposable
    {
        /// <summary>
        /// The maximum number of faults that are allowed to occur inside a given time span.
        /// </summary>
        private const int MaximumNumberOfSequentialFaults = 10;

        /// <summary>
        /// The number of minutes over which the number of faults is not allowed
        /// to exceed the <see cref="MaximumNumberOfSequentialFaults"/>.
        /// </summary>
        private const int FaultingTimeSpanInMinutes = 1;

        /// <summary>
        /// Maps the endpoint to the connection information.
        /// </summary>
        private readonly Dictionary<EndpointId, ChannelConnectionInformation> m_ChannelConnectionMap =
            new Dictionary<EndpointId, ChannelConnectionInformation>();

        /// <summary>
        /// The collection that contains the times at which faults occurred.
        /// </summary>
        /// <remarks>
        /// We always clear out the times that are futher away than the <see cref="FaultingTimeSpanInMinutes"/>
        /// so there should never be more than <see cref="MaximumNumberOfSequentialFaults"/> entries in the 
        /// storage. Except when we push the one fault time in that will push us over the limit. Hence we
        /// reserve space for that one more entry.
        /// </remarks>
        private readonly List<DateTimeOffset> m_LatestFaultingTimes =
            new List<DateTimeOffset>(MaximumNumberOfSequentialFaults + 1);

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
        /// The function that returns the current time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_CurrentTime;

        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

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
        /// The event handler used when the host closes.
        /// </summary>
        private EventHandler m_HostClosedHandler;

        /// <summary>
        /// The message handler that is used to receive messages from the
        /// receiving endpoint.
        /// </summary>
        private EventHandler<MessageEventArgs> m_MessageReceivingHandler;

        /// <summary>
        /// The connection information for the current channel.
        /// </summary>
        private ChannelConnectionInformation m_LocalConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationChannel"/> class.
        /// </summary>
        /// <param name="id">The ID number of the current endpoint.</param>
        /// <param name="channelType">The type of channel, e.g. TCP.</param>
        /// <param name="receiverBuilder">The function that builds receiving endpoints.</param>
        /// <param name="senderBuilder">The function that builds sending endpoints.</param>
        /// <param name="currentTime">The function that returns the current time.</param>
        /// <param name="logger">The function that is used to write messages to the log.</param>
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
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="currentTime"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public CommunicationChannel(
            EndpointId id, 
            IChannelType channelType, 
            Func<IMessagePipe> receiverBuilder,
            Func<Func<EndpointId, IChannelProxy>, ISendingEndpoint> senderBuilder,
            Func<DateTimeOffset> currentTime,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => id);
                Enforce.Argument(() => channelType);
                Enforce.Argument(() => receiverBuilder);
                Enforce.Argument(() => senderBuilder);
                Enforce.Argument(() => currentTime);
                Enforce.Argument(() => logger);
            }

            m_Id = id;
            m_Type = channelType;
            m_ReceiverBuilder = receiverBuilder;
            m_SenderBuilder = senderBuilder;
            m_CurrentTime = currentTime;
            m_Logger = logger;
        }

        /// <summary>
        /// Gets the connection information that describes the local endpoint.
        /// </summary>
        public ChannelConnectionInformation LocalConnectionPoint
        {
            get
            {
                return m_LocalConnection;
            }
        }

        /// <summary>
        /// Opens the channel and provides information on how to connect to the given channel.
        /// </summary>
        public void OpenChannel()
        {
            if (m_Receiver != null)
            {
                CloseChannel();
            }

            m_Receiver = m_ReceiverBuilder();
            m_MessageReceivingHandler = (s, e) => RaiseOnReceive(e.Message);
            m_Receiver.OnNewMessage += m_MessageReceivingHandler;

            var uri = m_Type.GenerateNewChannelUri();
            ReopenChannel(uri);
        }

        private void ReopenChannel(Uri uri)
        {
            // Clear the old host
            CleanupHost();

            // Create the new host
            m_HostFaultingHandler = (s, e) =>
                {
                    m_Logger(
                        LogSeverityProxy.Warning,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for address: {0} has faulted.",
                            uri));

                    StoreCurrentTimeAsFaultingTime();
                    ReopenChannel(uri);
                };

            m_HostClosedHandler = (s, e) =>
                {
                    m_Logger(
                        LogSeverityProxy.Warning,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for address: {0} has closed prematurely.",
                            uri));

                    ReopenChannel(uri);
                };

            m_Host = new ServiceHost(m_Receiver, uri);
            m_Host.Faulted += m_HostFaultingHandler;
            m_Host.Closed += m_HostClosedHandler;
            var endpoint = m_Type.AttachEndpoint(m_Host, typeof(IReceivingEndpoint), m_Id);
            m_LocalConnection = new ChannelConnectionInformation(m_Id, m_Type.GetType(), endpoint.Address.Uri);

            m_Host.Open();

            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Opened channel on address: {0}.",
                    uri));
        }

        private void StoreCurrentTimeAsFaultingTime()
        {
            var now = m_CurrentTime();
            m_LatestFaultingTimes.Add(now);
            
            var timespan = new TimeSpan(0, FaultingTimeSpanInMinutes, 0);
            while (m_LatestFaultingTimes[0] < now - timespan)
            {
                m_LatestFaultingTimes.RemoveAt(0);
            }

            if (m_LatestFaultingTimes.Count > MaximumNumberOfSequentialFaults)
            {
                throw new MaximumNumberOfChannelRestartsExceededException();
            }
        }

        private void CleanupHost()
        {
            if (m_Host != null)
            {
                m_Host.Faulted -= m_HostFaultingHandler;
                m_HostFaultingHandler = null;

                m_Host.Closed -= m_HostClosedHandler;
                m_HostClosedHandler = null;

                try
                {
                    m_Host.Close();
                }
                catch (TimeoutException)
                {
                    // The default interval of time that was allotted for the operation was exceeded
                    // before the operation was completed.
                    m_Host.Abort();
                }
                catch (InvalidOperationException)
                {
                    // EITHER:
                    // The communication object is in a System.ServiceModel.CommunicationState.Closing
                    // or System.ServiceModel.CommunicationState.Closed state and cannot be modified.
                    //
                    // OR
                    //
                    // The communication object is not in a System.ServiceModel.CommunicationState.Opened
                    // or System.ServiceModel.CommunicationState.Opening state and cannot be modified.
                    m_Host.Abort();
                }
                catch (CommunicationObjectFaultedException)
                {
                    // The communication object is in a System.ServiceModel.CommunicationState.Faulted
                    // state and cannot be modified.
                    m_Host.Abort();
                }

                var disposable = m_Host as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                m_Host = null;
            }
        }

        /// <summary>
        /// Closes the current channel.
        /// </summary>
        public void CloseChannel()
        {
            if (m_Sender != null)
            {
                // First notify the recipients that we're closing the channel.
                var knownEndpoints = new List<EndpointId>(m_Sender.KnownEndpoints());
                foreach (var key in knownEndpoints)
                {
                    var msg = new EndpointDisconnectMessage(m_Id);
                    try
                    {
                        m_Sender.Send(key, msg);
                    }
                    catch (FailedToSendMessageException)
                    {
                        // For some reason the message didn't arrive. Honestly we don't
                        // care, we're about to quit, not our problem anymore.
                    }
                }

                // Then close the channel. We'll do this in a different
                // loop to give the channels time to process the messages.
                foreach (var key in knownEndpoints)
                {
                    m_Sender.CloseChannelTo(key);
                }
            }

            CleanupHost();

            if (m_Receiver != null)
            {
                m_Receiver.OnNewMessage -= m_MessageReceivingHandler;
                m_Receiver = null;
            }

            m_LocalConnection = null;

            m_Logger(LogSeverityProxy.Trace, "Closed channel");
            RaiseOnClosed();
        }

        /// <summary>
        /// Returns a value indicating if a connection to the given endpoint has been made.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <returns>
        ///     <see langword="true"/> if a connection to the endpoint has been made; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasConnectionTo(EndpointId endpoint)
        {
            return m_ChannelConnectionMap.ContainsKey(endpoint);
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
            Send(
                connection.Id, 
                new EndpointConnectMessage(
                    m_Id, 
                    m_LocalConnection.Address.AbsoluteUri, 
                    m_Type.GetType()));
        }

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint from which the channel needs to disconnect.</param>
        public void DisconnectFrom(EndpointId endpoint)
        {
            if (m_ChannelConnectionMap.ContainsKey(endpoint))
            {
                Send(endpoint, new EndpointDisconnectMessage(m_Id));

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
        /// <param name="scheduler">The scheduler that is used to run the return task with.</param>
        /// <returns>
        /// The connection information necessary to connect to the newly created channel and the task 
        /// responsible for handling the data reception.
        /// </returns>
        public System.Tuple<StreamTransferInformation, Task<FileInfo>> PrepareForDataReception(
            string localFile,
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Enforce.Argument(() => localFile);
                Enforce.With<ArgumentException>(!string.IsNullOrWhiteSpace(localFile), Resources.Exceptions_Messages_FilePathCannotBeEmpty);
            }

            return m_Type.PrepareForDataReception(localFile, token, scheduler);
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
        /// <param name="scheduler">The scheduler that is used to run the return task with.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        public Task TransferData(
            string filePath,
            StreamTransferInformation transferInformation,
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Enforce.Argument(() => filePath);
                Enforce.With<ArgumentException>(!string.IsNullOrWhiteSpace(filePath), Resources.Exceptions_Messages_FilePathCannotBeEmpty);
                Enforce.Argument(() => transferInformation);
            }

            return m_Type.TransferData(filePath, transferInformation, token, scheduler);
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
            var endpoint = new EndpointAddress(connectionInfo.Address);

            Debug.Assert(m_Type.GetType().Equals(connectionInfo.ChannelType), "Trying to connect to a channel with a different binding type.");
            var binding = m_Type.GenerateBinding();

            var factory = new ChannelFactory<IReceivingWcfEndpointProxy>(binding, endpoint);
            return new SelfResurrectingSendingEndpoint(factory, m_Logger);
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
                local(this, new ChannelClosedEventArgs(m_Id));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CleanupHost();
        }
    }
}
