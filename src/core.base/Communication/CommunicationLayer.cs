//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods needed to communicate with one or more remote applications.
    /// </summary>
    internal sealed class CommunicationLayer : ICommunicationLayer
    {
        /// <summary>
        /// Creates a new <see cref="EndpointId"/> for the current process.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="EndpointId"/>.
        /// </returns>
        private static EndpointId CreateEndpointIdForCurrentProcess()
        {
            return new EndpointId(string.Format("{0}:{1}", Environment.MachineName, Process.GetCurrentProcess().Id));
        }

        /// <summary>
        /// The collection of endpoints that have been discovered.
        /// </summary>
        private readonly Dictionary<EndpointId, SortedList<int, ChannelConnectionInformation>> m_PotentialEndpoints =
            new Dictionary<EndpointId, SortedList<int, ChannelConnectionInformation>>();

        /// <summary>
        /// The collection of <see cref="IChannelType"/> objects which refer to a communication.
        /// </summary>
        private readonly Dictionary<Type, Tuple<ICommunicationChannel, IDirectIncomingMessages>> m_OpenConnections =
            new Dictionary<Type, Tuple<ICommunicationChannel, IDirectIncomingMessages>>();

        /// <summary>
        /// The ID number of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Id = CreateEndpointIdForCurrentProcess();

        /// <summary>
        /// The collection of endpoint discovery objects.
        /// </summary>
        private readonly IEnumerable<IDiscoverOtherServices> m_DiscoverySources;

        /// <summary>
        /// The function that returns a tuple of a <see cref="ICommunicationChannel"/> and
        /// a <see cref="IDirectIncomingMessages"/> which belong together. The return values
        /// are based on the type of the <see cref="IChannelType"/> for the channel.
        /// </summary>
        private readonly Func<Type, EndpointId, Tuple<ICommunicationChannel, IDirectIncomingMessages>> m_ChannelBuilder;

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
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoverySources"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelBuilder"/> is <see langword="null" />.
        /// </exception>
        public CommunicationLayer(IEnumerable<IDiscoverOtherServices> discoverySources, Func<Type, EndpointId, Tuple<ICommunicationChannel, IDirectIncomingMessages>> channelBuilder)
        {
            {
                Enforce.Argument(() => discoverySources);
                Enforce.Argument(() => channelBuilder);
            }

            m_ChannelBuilder = channelBuilder;
            m_DiscoverySources = discoverySources;
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
        public bool IsSignedOn
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
            foreach (var tuple in m_OpenConnections.Values)
            {
                var info = tuple.Item1.LocalConnectionPoint;
                if (info != null)
                { 
                    yield return info;
                }
            }
        }

        /// <summary>
        /// Connects to the network and broadcasts a sign on message.
        /// </summary>
        public void SignOn()
        {
            if (m_AlreadySignedOn)
            {
                return;
            }

            // First we load up our own channels so that we 
            // can send out our own information.
            m_OpenConnections.Add(typeof(NamedPipeChannelType), m_ChannelBuilder(typeof(NamedPipeChannelType), m_Id));
            m_OpenConnections.Add(typeof(TcpChannelType), m_ChannelBuilder(typeof(TcpChannelType), m_Id));
            foreach (var tuple in m_OpenConnections.Values)
            {
                tuple.Item1.OpenChannel();
            }

            // Now we initiate discovery of other services. Note that discovery only works for 
            // TCP based connections. It does not work with named pipes, however we have a 
            // discovery source that can manually be controlled. This source will be able
            // to provide the named pipe discoveries. Also note that in our case
            // we don't need it to work with named pipes because the current application is 
            // responsible for the creation of all apps that it communicates with through
            // a named pipe, i.e. we never need to discover anything on a named pipe.
            // Also note that the only thing we discover on a TCP connection is the
            // application that is in control of creating dataset applications on the
            // remote machine.
            foreach (var source in m_DiscoverySources)
            {
                source.OnEndpointBecomingAvailable += HandleEndpointSignOn;
                source.OnEndpointBecomingUnavailable += HandleEndpointSignedOff;
                source.StartDiscovery();
            }

            m_AlreadySignedOn = true;
        }

        private void HandleEndpointSignOn(object sender, ConnectionInformationEventArgs args)
        {
            var info = args.ConnectionInformation;
            if (m_Id.Equals(info.Id))
            {
                return;
            }

            // Add to the available list
            if (!m_PotentialEndpoints.ContainsKey(info.Id))
            {
                m_PotentialEndpoints.Add(info.Id, new SortedList<int, ChannelConnectionInformation>());
            }

            var list = m_PotentialEndpoints[info.Id];
            if (!list.Values.Exists(m => m.ChannelType.Equals(info.ChannelType) && string.Equals(m.Address.AbsoluteUri, info.Address.AbsoluteUri, StringComparison.OrdinalIgnoreCase)))
            {
                var attributes = info.ChannelType.GetCustomAttributes(typeof(ChannelRelativePerformanceAttribute), false);
                int order = (attributes.Length > 0) ? (attributes[0] as ChannelRelativePerformanceAttribute).RelativeOrder : int.MaxValue;
                list.Add(order, info);

                // Notify the world
                RaiseOnEndpointSignOn(info.Id, info.ChannelType, info.Address);
            }
        }

        // How do we handle endpoints disappearing and then reappearing. If the remote process
        // crashed then we'll have an enpoint on the same machine but with another process id.
        // The catch is that we can't just look for the machine name because there is a possibility
        // that there will be more than one process on the machine .... Gah
        //
        // Also note that it is quite easily possible to fake being another endpoint. All you have
        // to do is send a message saying that you're a different endpoint and then the evil is
        // done. Not quite sure how to make that not happen though ...
        private void HandleEndpointSignedOff(object sender, EndpointEventArgs args)
        {
            if (m_Id.Equals(args.Endpoint))
            {
                return;
            }

            // Remove from the available list
            if (m_PotentialEndpoints.ContainsKey(args.Endpoint))
            {
                // notify the outside world
                RaiseOnEndpointSignedOff(args.Endpoint);

                var list = m_PotentialEndpoints[args.Endpoint];
                m_PotentialEndpoints.Remove(args.Endpoint);

                // Notify the comm.channel that the endpoint is dead or about to be
                foreach (var pair in list)
                {
                    var connection = pair.Value;
                    if (m_OpenConnections.ContainsKey(connection.ChannelType))
                    {
                        var channel = m_OpenConnections[connection.ChannelType];
                        channel.Item1.DisconnectFrom(args.Endpoint);
                    }
                }
            }
        }

        /// <summary>
        /// Broadcasts a sign off message and disconnects from the network.
        /// </summary>
        public void SignOff()
        {
            if (!m_AlreadySignedOn)
            {
                return;
            }

            // Stop discovering other services. We just stopped caring.
            foreach (var source in m_DiscoverySources)
            {
                source.EndDiscovery();
                source.OnEndpointBecomingAvailable -= HandleEndpointSignOn;
                source.OnEndpointBecomingUnavailable -= HandleEndpointSignedOff;
            }

            // There may be a race condition here. We could be disconnecting while
            // others may be trying to connect, so we might have to put a 
            // lock in here to block things from happening.
            //
            // Disconnect from all channels
            foreach (var pair in m_OpenConnections)
            {
                var connection = pair.Value.Item1;
                connection.CloseChannel();
            }

            // Clear all connections
            m_OpenConnections.Clear();

            m_AlreadySignedOn = false;
        }

        /// <summary>
        /// An event raised when an endpoint joined the network.
        /// </summary>
        public event EventHandler<ConnectionInformationEventArgs> OnEndpointSignedOn;

        private void RaiseOnEndpointSignOn(EndpointId id, Type channelType, Uri address)
        {
            var local = OnEndpointSignedOn;
            if (local != null)
            {
                local(this, new ConnectionInformationEventArgs(new ChannelConnectionInformation(id, channelType, address)));
            }
        }

        /// <summary>
        /// An event raised when an endpoint has left the network.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointSignedOff;

        private void RaiseOnEndpointSignedOff(EndpointId endpoint)
        {
            var local = OnEndpointSignedOff;
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
            return (endpoint != null) && m_PotentialEndpoints.ContainsKey(endpoint);
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

            var pair = m_OpenConnections[connection.ChannelType];
            pair.Item1.ConnectTo(connection);
        }

        private ChannelConnectionInformation SelectMostAppropriateConnection(EndpointId endpoint)
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

            var pair = m_OpenConnections[connection.ChannelType];
            var channel = pair.Item1;
            if (!channel.HasConnectionTo(endpoint))
            {
                channel.ConnectTo(connection);
            }
            
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
        public Task<ICommunicationMessage> SendMessageAndWaitForRespone(EndpointId endpoint, ICommunicationMessage message)
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

            var pair = m_OpenConnections[connection.ChannelType];
            var result = pair.Item2.ForwardResponse(endpoint, message.Id);

            pair.Item1.Send(endpoint, message);
            return result;
        }

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public void DisconnectFromEndpoint(EndpointId endpoint)
        {
            // Remove from the available list
            if (m_PotentialEndpoints.ContainsKey(endpoint))
            {
                var list = m_PotentialEndpoints[endpoint];
                foreach (var pair in list)
                {
                    var connection = pair.Value;
                    if (m_OpenConnections.ContainsKey(connection.ChannelType))
                    {
                        var channel = m_OpenConnections[connection.ChannelType];
                        channel.Item1.DisconnectFrom(endpoint);
                    }
                }
            }
        }
    }
}
