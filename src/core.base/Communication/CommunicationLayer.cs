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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Properties;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods needed to communicate with one or more remote applications.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class CommunicationLayer : ICommunicationLayer, IDisposable
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of endpoints that have been discovered.
        /// </summary>
        private readonly Dictionary<EndpointId, SortedList<int, ChannelConnectionInformation>> m_PotentialEndpoints =
            new Dictionary<EndpointId, SortedList<int, ChannelConnectionInformation>>();

        /// <summary>
        /// The collection of <see cref="IChannelType"/> objects which refer to a communication.
        /// </summary>
        private readonly Dictionary<Type, System.Tuple<ICommunicationChannel, IDirectIncomingMessages>> m_OpenConnections =
            new Dictionary<Type, System.Tuple<ICommunicationChannel, IDirectIncomingMessages>>();

        /// <summary>
        /// The ID number of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();

        /// <summary>
        /// The collection of endpoint discovery objects.
        /// </summary>
        private readonly IEnumerable<IDiscoverOtherServices> m_DiscoverySources;

        /// <summary>
        /// The function that returns a tuple of a <see cref="ICommunicationChannel"/> and
        /// a <see cref="IDirectIncomingMessages"/> which belong together. The return values
        /// are based on the type of the <see cref="IChannelType"/> for the channel.
        /// </summary>
        private readonly Func<Type, EndpointId, System.Tuple<ICommunicationChannel, IDirectIncomingMessages>> m_ChannelBuilder;

        /// <summary>
        /// The function used to write messages to the log.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Indicates if the layer is signed on or not.
        /// </summary>
        private volatile bool m_AlreadySignedOn;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationLayer"/> class.
        /// </summary>
        /// <param name="discoverySources">The object that handles the discovery of remote endpoints.</param>
        /// <param name="channelBuilder">
        ///     The function that returns a tuple of a <see cref="ICommunicationChannel"/> and a <see cref="IDirectIncomingMessages"/>
        ///     based on the type of the <see cref="IChannelType"/> that is related to the channel.
        /// </param>
        /// <param name="logger">The function that is used to write messages to the log.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoverySources"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public CommunicationLayer(
            IEnumerable<IDiscoverOtherServices> discoverySources,
            Func<Type, EndpointId, System.Tuple<ICommunicationChannel, IDirectIncomingMessages>> channelBuilder,
            Action<LogSeverityProxy, string> logger)
        {
            {
                Enforce.Argument(() => discoverySources);
                Enforce.Argument(() => channelBuilder);
                Enforce.Argument(() => logger);
            }

            m_ChannelBuilder = channelBuilder;
            m_DiscoverySources = discoverySources;
            m_Logger = logger;
        }

        /// <summary>
        /// Gets the endpoint ID of the local endpoint.
        /// </summary>
        public EndpointId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the communication layer has signed on with
        /// the network.
        /// </summary>
        public bool IsSignedIn
        {
            get
            {
                return m_AlreadySignedOn;
            }
        }

        /// <summary>
        /// Returns a collection containing information about the local connection points.
        /// </summary>
        /// <returns>
        /// The collection that describes the local connection points.
        /// </returns>
        public IEnumerable<ChannelConnectionInformation> LocalConnectionPoints()
        {
            var list = new List<ChannelConnectionInformation>();
            lock (m_Lock)
            {
                list.AddRange(from tuple in m_OpenConnections.Values select tuple.Item1.LocalConnectionPoint);
            }

            return list;
        }

        /// <summary>
        /// Returns a collection containing the endpoint IDs of the known remote endpoints.
        /// </summary>
        /// <returns>
        ///     The collection that contains the endpoint IDs of the remote endpoints.
        /// </returns>
        public IEnumerable<EndpointId> KnownEndpoints()
        {
            var list = new List<EndpointId>();
            lock (m_Lock)
            {
                list.AddRange(m_PotentialEndpoints.Keys);
            }

            return list;
        }

        /// <summary>
        /// Connects to the network and broadcasts a sign on message.
        /// </summary>
        public void SignIn()
        {
            if (m_AlreadySignedOn)
            {
                return;
            }

            // First we load up our own channels so that we 
            // can send out our own information.
            lock (m_Lock)
            {
                m_OpenConnections.Add(typeof(NamedPipeChannelType), m_ChannelBuilder(typeof(NamedPipeChannelType), m_Id));
                m_OpenConnections.Add(typeof(TcpChannelType), m_ChannelBuilder(typeof(TcpChannelType), m_Id));
                foreach (var tuple in m_OpenConnections.Values)
                {
                    tuple.Item1.OpenChannel();
                }
            }

            // Now we initiate discovery of other services. Note that discovery only works for 
            // TCP based connections. It does not work with named pipes, however we have a 
            // discovery source that can manually be controlled. This source will be able
            // to provide the named pipe discoveries. 
            //
            // NOTE: in our case we don't need it to work with named pipes because the current 
            // application is responsible for the creation of all apps that it communicates with 
            // through a named pipe, i.e. we never need to discover anything on a named pipe.
            //
            // NOTE: the only thing we discover on a TCP connection is the application that is in 
            // control of creating dataset applications on the remote machine.
            foreach (var source in m_DiscoverySources)
            {
                source.OnEndpointBecomingAvailable += HandleEndpointSignIn;
                source.OnEndpointBecomingUnavailable += HandleEndpointSignedOut;
                source.StartDiscovery();
            }

            m_Logger(LogSeverityProxy.Trace, "Sign on process finished.");
            m_AlreadySignedOn = true;
        }

        private void HandleEndpointSignIn(object sender, ConnectionInformationEventArgs args)
        {
            var info = args.ConnectionInformation;
            if (m_Id.Equals(info.Id))
            {
                return;
            }

            // If we don't know the endpoint at all then add a collection for it.
            lock (m_Lock)
            {
                if (!m_PotentialEndpoints.ContainsKey(info.Id))
                {
                    m_PotentialEndpoints.Add(info.Id, new SortedList<int, ChannelConnectionInformation>());
                }

                var list = m_PotentialEndpoints[info.Id];

                var matchingItems = from pair in list
                                    where pair.Value.ChannelType.Equals(info.ChannelType)
                                    select pair;

                Debug.Assert(matchingItems.Count() <= 1, "We expect at maximum one channel to match the endpoint search here.");
                if (matchingItems.Exists())
                {
                    foreach (var item in matchingItems)
                    {
                        if (item.Value.Address.Equals(info.Address))
                        {
                            // We already have the connection with the exact 
                            // address. So we don't do anything.
                            return;
                        }

                        list.Remove(item.Key);
                    }
                }

                var attributes = info.ChannelType.GetCustomAttributes(typeof(ChannelRelativePerformanceAttribute), false);
                int order = (attributes.Length > 0) ? (attributes[0] as ChannelRelativePerformanceAttribute).RelativeOrder : int.MaxValue;
                list.Add(order, info);
            }

            // Notify the world
            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "New endpoint ({0}) signed in via the {1} channel. Endpoint URL: {2}.",
                    info.Id,
                    info.ChannelType,
                    info.Address));

            ConnectToEndpoint(info);
            RaiseOnEndpointSignIn(info.Id, info.ChannelType, info.Address);
        }

        // How do we handle endpoints disappearing and then reappearing. If the remote process
        // crashed then we'll have an enpoint on the same machine but with another process id.
        // The catch is that we can't just look for the machine name because there is a possibility
        // that there will be more than one process on the machine .... Gah
        //
        // Also note that it is quite easily possible to fake being another endpoint. All you have
        // to do is send a message saying that you're a different endpoint and then the evil is
        // done. Not quite sure how to make that not happen though ...
        private void HandleEndpointSignedOut(object sender, EndpointEventArgs args)
        {
            if (m_Id.Equals(args.Endpoint))
            {
                return;
            }

            var exists = false;
            lock (m_Lock)
            {
                exists = m_PotentialEndpoints.ContainsKey(args.Endpoint);
            }

            // Remove from the available list
            if (exists)
            {
                // notify the outside world
                RaiseOnEndpointSignedOut(args.Endpoint);

                var list = new List<ChannelConnectionInformation>();
                lock (m_Lock)
                {
                    list.AddRange(from info in m_PotentialEndpoints[args.Endpoint] select info.Value);
                    m_PotentialEndpoints.Remove(args.Endpoint);
                }

                // Notify the comm.channel that the endpoint is dead or about to be
                foreach (var connection in list)
                {
                    var channel = ChannelForChannelType(connection.ChannelType);
                    if (channel != null)
                    {
                        channel.DisconnectFrom(args.Endpoint);
                    }
                }
            }
        }

        private ICommunicationChannel ChannelForChannelType(Type connection)
        {
            return ChannelInformationForType(connection).Item1;
        }

        private System.Tuple<ICommunicationChannel, IDirectIncomingMessages> ChannelInformationForType(Type connection)
        {
            System.Tuple<ICommunicationChannel, IDirectIncomingMessages> channel = null;
            lock (m_Lock)
            {
                if (m_OpenConnections.ContainsKey(connection))
                {
                    channel = m_OpenConnections[connection];
                }

                return channel;
            }
        }

        /// <summary>
        /// Broadcasts a sign off message and disconnects from the network.
        /// </summary>
        public void SignOut()
        {
            if (!m_AlreadySignedOn)
            {
                return;
            }

            // Stop discovering other services. We just stopped caring.
            foreach (var source in m_DiscoverySources)
            {
                source.EndDiscovery();
                source.OnEndpointBecomingAvailable -= HandleEndpointSignIn;
                source.OnEndpointBecomingUnavailable -= HandleEndpointSignedOut;
            }

            // There may be a race condition here. We could be disconnecting while
            // others may be trying to connect, so we might have to put a 
            // lock in here to block things from happening.
            //
            // Disconnect from all channels
            lock (m_Lock)
            {
                foreach (var pair in m_OpenConnections)
                {
                    var connection = pair.Value.Item1;
                    connection.CloseChannel();
                }

                // Clear all connections
                m_OpenConnections.Clear();
            }

            m_Logger(LogSeverityProxy.Trace, "Sign off process finished.");
            m_AlreadySignedOn = false;
        }

        /// <summary>
        /// An event raised when an endpoint joined the network.
        /// </summary>
        public event EventHandler<ConnectionInformationEventArgs> OnEndpointSignedIn;

        private void RaiseOnEndpointSignIn(EndpointId id, Type channelType, Uri address)
        {
            var local = OnEndpointSignedIn;
            if (local != null)
            {
                local(this, new ConnectionInformationEventArgs(new ChannelConnectionInformation(id, channelType, address)));
            }
        }

        /// <summary>
        /// An event raised when an endpoint has left the network.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointSignedOut;

        private void RaiseOnEndpointSignedOut(EndpointId endpoint)
        {
            var local = OnEndpointSignedOut;
            if (local != null)
            {
                local(this, new EndpointEventArgs(endpoint));
            }
        }

        /// <summary>
        /// Returns a value indicating if the given endpoint has provided the information required to
        /// contact it if it isn't offline.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if the endpoint has provided the information necessary to contact 
        ///     it over the network. Otherwise; <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsEndpointContactable(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                return (endpoint != null) && m_PotentialEndpoints.ContainsKey(endpoint);
            }
        }

        /// <summary>
        /// Connects to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="EndpointNotContactableException">
        ///     Thrown if <paramref name="endpoint"/> has not provided any contact information.
        /// </exception>
        public void ConnectToEndpoint(EndpointId endpoint)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.With<EndpointNotContactableException>(
                    m_PotentialEndpoints.ContainsKey(endpoint),
                    Resources.Exceptions_Messages_EndpointNotContactable_WithEndpoint,
                    endpoint);
            }

            var connection = SelectMostAppropriateConnection(endpoint);
            Debug.Assert(connection != null, "There are no known ways to connect to the given endpoint.");

            ConnectToEndpoint(connection);
        }

        private void ConnectToEndpoint(ChannelConnectionInformation connection)
        {
            {
                Debug.Assert(connection != null, "The connection information should not be null.");
            }

            var channel = ChannelForChannelType(connection.ChannelType);
            channel.ConnectTo(connection);

            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Connected to endpoint ({0}) signed in via the {1} channel",
                    connection.Id,
                    connection.ChannelType));
        }

        private ChannelConnectionInformation SelectMostAppropriateConnection(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                var list = m_PotentialEndpoints[endpoint];
                if (list.Count == 0)
                {
                    return null;
                }
                else
                {
                    // If there is an item in the list then we can just return the first one because that 
                    // will be the fastest connection.
                    return list.Values[0];
                }
            }
        }

        /// <summary>
        /// Sends the given message to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="EndpointNotContactableException">
        ///     Thrown if <paramref name="endpoint"/> has not provided any contact information.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public void SendMessageTo(EndpointId endpoint, ICommunicationMessage message)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.With<EndpointNotContactableException>(
                    m_PotentialEndpoints.ContainsKey(endpoint),
                    Resources.Exceptions_Messages_EndpointNotContactable_WithEndpoint,
                    endpoint);

                Enforce.Argument(() => message);
            }

            var connection = SelectMostAppropriateConnection(endpoint);
            Debug.Assert(connection != null, "There are no known ways to connect to the given endpoint.");

            var channel = ChannelForChannelType(connection.ChannelType);
            if (!channel.HasConnectionTo(endpoint))
            {
                channel.ConnectTo(connection);
            }

            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Sending msg of type {0} to endpoint ({1}) via the {2} channel without waiting for the response.",
                    message.GetType(),
                    endpoint,
                    connection.ChannelType));

            channel.Send(endpoint, message);
        }

        /// <summary>
        /// Sends the given message to the specified endpoint and returns a task that
        /// will eventually contain the return message.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <returns>A task object that will eventually contain the response message.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="EndpointNotContactableException">
        ///     Thrown if <paramref name="endpoint"/> has not provided any contact information.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public Task<ICommunicationMessage> SendMessageAndWaitForResponse(EndpointId endpoint, ICommunicationMessage message)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.With<EndpointNotContactableException>(
                    m_PotentialEndpoints.ContainsKey(endpoint),
                    Resources.Exceptions_Messages_EndpointNotContactable_WithEndpoint,
                    endpoint);

                Enforce.Argument(() => message);
            }

            var connection = SelectMostAppropriateConnection(endpoint);
            Debug.Assert(connection != null, "There are no known ways to connect to the given endpoint.");

            var pair = ChannelInformationForType(connection.ChannelType);
            var result = pair.Item2.ForwardResponse(endpoint, message.Id);

            m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Sending msg of type {0} to endpoint ({1}) via the {2} channel while waiting for the response.",
                    message.GetType(),
                    endpoint,
                    connection.ChannelType));

            pair.Item1.Send(endpoint, message);
            return result;
        }

        /// <summary>
        /// Uploads a given file to a specific endpoint.
        /// </summary>
        /// <param name="filePath">The full path to the file that should be transferred.</param>
        /// <param name="transferInfo">The object that provides the upload information.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler that is used to run the return task.</param>
        /// <returns>
        ///     A task that will return once the upload is complete.
        /// </returns>
        public Task UploadData(
            string filePath,
            StreamTransferInformation transferInfo,
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Enforce.Argument(() => filePath);
                Enforce.Argument(() => transferInfo);
            }

            var channel = ChannelForChannelType(transferInfo.ChannelType);
            return channel.TransferData(filePath, transferInfo, token, scheduler);
        }

        /// <summary>
        /// Downloads a given file from a specific endpoint.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="localFile"/> does not exist a new file will be created with the given path. If
        /// it does exist then the data will be appended to it.
        /// </remarks>
        /// <param name="endpointToDownloadFrom">The endpoint ID of the endpoint from which the data should be transferred.</param>
        /// <param name="uploadToken">The token that indicates which file should be uploaded.</param>
        /// <param name="localFile">The full file path to which the network stream should be written.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler that is used to run the return task.</param>
        /// <returns>
        /// The task which will return the pointer to the file once the download is complete.
        /// </returns>
        public Task<Stream> DownloadData(
            EndpointId endpointToDownloadFrom,
            UploadToken uploadToken,
            string localFile,
            CancellationToken token,
            TaskScheduler scheduler)
        {
            {
                Enforce.Argument(() => endpointToDownloadFrom);
                Enforce.With<EndpointNotContactableException>(
                    m_PotentialEndpoints.ContainsKey(endpointToDownloadFrom),
                    Resources.Exceptions_Messages_EndpointNotContactable_WithEndpoint,
                    endpointToDownloadFrom);

                Enforce.Argument(() => uploadToken);
            }

            var connection = SelectMostAppropriateConnection(endpointToDownloadFrom);
            Debug.Assert(connection != null, "There are no known ways to connect to the given endpoint.");

            var channel = ChannelForChannelType(connection.ChannelType);
            var info = channel.PrepareForDataReception(localFile, token, scheduler);

            var msg = new DataDownloadRequestMessage(Id, uploadToken, info.Item1);
            channel.Send(endpointToDownloadFrom, msg);
            return info.Item2.ContinueWith<Stream>(
                t =>
                {
                    return new FileStream(
                        t.Result.FullName,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.None);
                },
                token,
                TaskContinuationOptions.None,
                scheduler);
        }

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public void DisconnectFromEndpoint(EndpointId endpoint)
        {
            // Remove from the available list
            bool exists = false;
            lock (m_Lock)
            {
                exists = m_PotentialEndpoints.ContainsKey(endpoint);
            }

            if (exists)
            {
                var list = new List<ChannelConnectionInformation>();
                lock (m_Lock)
                {
                    list.AddRange(from pair in m_PotentialEndpoints[endpoint] select pair.Value);
                }

                foreach (var connection in list)
                {
                    if (m_OpenConnections.ContainsKey(connection.ChannelType))
                    {
                        var channel = m_OpenConnections[connection.ChannelType];
                        channel.Item1.DisconnectFrom(endpoint);
                    }
                }

                m_Logger(
                LogSeverityProxy.Trace,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Disconnecting from endpoint ({0}).",
                    endpoint));

                // Technically the endpoint hasn't signed out, however we have told it
                // we are going to and so it comes down to the same thing.
                RaiseOnEndpointSignedOut(endpoint);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SignOut();
        }
    }
}
