//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        private readonly Dictionary<EndpointId, ChannelConnectionInformation> m_PotentialEndpoints =
            new Dictionary<EndpointId, ChannelConnectionInformation>();

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
        /// The endpoint discovery channel.
        /// </summary>
        private readonly IDiscoverOtherServices m_DiscoveryChannel;

        /// <summary>
        /// The function that returns a tuple of a <see cref="ICommunicationChannel"/> and
        /// a <see cref="IDirectIncomingMessages"/> which belong together. The return values
        /// are based on the type of the <see cref="IChannelType"/> for the channel.
        /// </summary>
        private readonly Func<Type, Tuple<ICommunicationChannel, IDirectIncomingMessages>> m_ChannelBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationLayer"/> class.
        /// </summary>
        /// <param name="discoveryChannel">The object that handles the discovery of remote endpoints.</param>
        /// <param name="channelBuilder">
        ///     The function that returns a tuple of a <see cref="ICommunicationChannel"/> and a <see cref="IDirectIncomingMessages"/>
        ///     based on the type of the <see cref="IChannelType"/> that is related to the channel.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoveryChannel"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelBuilder"/> is <see langword="null" />.
        /// </exception>
        public CommunicationLayer(IDiscoverOtherServices discoveryChannel, Func<Type, Tuple<ICommunicationChannel, IDirectIncomingMessages>> channelBuilder)
        {
            {
                Enforce.Argument(() => discoveryChannel);
                Enforce.Argument(() => channelBuilder);
            }

            m_DiscoveryChannel = discoveryChannel;
            m_ChannelBuilder = channelBuilder;
        }

        private void HandleEndpointSignOn(EndpointId id, ChannelConnectionInformation connectionInfo)
        {
            // Add to the available list
            if (!m_PotentialEndpoints.ContainsKey(id))
            {
                m_PotentialEndpoints.Add(id, connectionInfo);
            }

            // Notify the world
            RaiseOnEndpointSignOn(id);
        }

        // How do we handle endpoints disappearing and then reappearing. If the remote process
        // crashed then we'll have an enpoint on the same machine but with another process id.
        // The catch is that we can't just look for the machine name because there is a possibility
        // that there will be more than one process on the machine .... Gah
        private void HandleEndpointSignedOff(EndpointId id)
        {
            // notify the outside world
            RaiseOnEndpointSignedOff(id);

            // Remove from the available list
            if (m_PotentialEndpoints.ContainsKey(id))
            {
                var connectionInfo = m_PotentialEndpoints[id];
                m_PotentialEndpoints.Remove(id);

                // Notify the comm.channel that the endpoint is dead or about to be
                if (m_OpenConnections.ContainsKey(connectionInfo.ChannelType))
                {
                    var channel = m_OpenConnections[connectionInfo.ChannelType];
                    channel.Item1.DisconnectFrom(id);
                }
            }
        }

        /// <summary>
        /// Connects to the network and broadcasts a sign on message.
        /// </summary>
        public void SignOn()
        {
            // First we load up our own channels so that we 
            // can send out our own information.
            m_OpenConnections.Add(typeof(NamedPipeChannelType), m_ChannelBuilder(typeof(NamedPipeChannelType)));
            m_OpenConnections.Add(typeof(TcpChannelType), m_ChannelBuilder(typeof(TcpChannelType)));

            // Now we initiate discovery of other services.
            m_DiscoveryChannel.StartDiscovery();
        }

        /// <summary>
        /// Broadcasts a sign off message and disconnects from the network.
        /// </summary>
        public void SignOff()
        {
            // Tell the world that we're done
            m_DiscoveryChannel.EndDiscovery();

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
        }

        /// <summary>
        /// An event raised when an endpoint joined the network.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointSignedOn;

        private void RaiseOnEndpointSignOn(EndpointId id)
        {
            var local = OnEndpointSignedOn;
            if (local != null)
            {
                local(this, new EndpointEventArgs(id));
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

            var connectionInfo = m_PotentialEndpoints[endpoint];
            if (!m_OpenConnections.ContainsKey(connectionInfo.ChannelType))
            {
                var tuple = m_ChannelBuilder(connectionInfo.ChannelType);
                m_OpenConnections.Add(connectionInfo.ChannelType, tuple);
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

            var connectionInfo = m_PotentialEndpoints[endpoint];
            if (!m_OpenConnections.ContainsKey(connectionInfo.ChannelType))
            {
                ConnectToEndpoint(endpoint);
            }

            var pair = m_OpenConnections[connectionInfo.ChannelType];
            pair.Item1.Send(endpoint, message);
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

            var connectionInfo = m_PotentialEndpoints[endpoint];
            if (!m_OpenConnections.ContainsKey(connectionInfo.ChannelType))
            {
                ConnectToEndpoint(endpoint);
            }

            var pair = m_OpenConnections[connectionInfo.ChannelType];
            var result = pair.Item2.ForwardResponse(endpoint, message.Id);

            pair.Item1.Send(endpoint, message);
            return result;
        }
    }
}
