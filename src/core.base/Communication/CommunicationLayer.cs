//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods needed to communicate with one or more remote applications.
    /// </summary>
    internal sealed class CommunicationLayer : ICommunicationLayer
    {
        /// <summary>
        /// The collection of endpoints that have been discovered.
        /// </summary>
        private readonly Dictionary<EndpointId, ChannelConnectionInformation> m_PotentialEndpoints =
            new Dictionary<EndpointId, ChannelConnectionInformation>();

        /// <summary>
        /// The collection of endpoints to which a connection exists.
        /// </summary>
        private readonly Dictionary<EndpointId, ICommunicationChannel> m_OpenConnections =
            new Dictionary<EndpointId, ICommunicationChannel>();

        //// <summary>
        //// The endpoint discovery channel.
        //// </summary>
        // private readonly IDiscoverOtherServices m_DiscoveryChannel;

        //// <summary>
        //// The object used to build communication channels.
        //// </summary>
        // private readonly ChannelBuilder m_ChannelBuilder;

        /// <summary>
        /// An event raised when the availability of an endpoint changes.
        /// </summary>
        public event EventHandler<EndpointAvailabilityEventArgs> OnAvailabilityChange;

        private void RaiseOnAvailabilityChange()
        {
            var local = OnAvailabilityChange;
            if (local != null)
            {
                local(this, new EndpointAvailabilityEventArgs());
            }
        }

        /// <summary>
        /// An event raised when an endpoint goes offline permanently.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnTerminate;

        private void RaiseOnTerminate(EndpointId endpoint)
        {
            var local = OnTerminate;
            if (local != null)
            {
                local(this, new EndpointEventArgs(endpoint));
            }
        }

        /// <summary>
        /// Returns a value indicating if the given endpoint is available on the network.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if the endpoint can be reached over the network. Otherwise; <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsEndpointAvailable(EndpointId endpoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connects to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public void ConnectToEndpoint(EndpointId endpoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends the given message to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        public void SendMessageTo(EndpointId endpoint, ICommunicationMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
