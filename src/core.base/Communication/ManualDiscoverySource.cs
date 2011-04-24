//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Handles the discovery of endpoints by accepting endpoint information from
    /// external sources.
    /// </summary>
    internal sealed class ManualDiscoverySource : IDiscoverOtherServices, IAcceptExternalEndpointInformation
    {
        /// <summary>
        /// Indicates if discovery information should be passed on or not.
        /// </summary>
        private bool m_IsDiscoveryAllowed;

        /// <summary>
        /// An event raised when a remote endpoint becomes available.
        /// </summary>
        public event EventHandler<ConnectionInformationEventArgs> OnEndpointBecomingAvailable;

        private void RaiseOnEndpointBecomingAvailable(ChannelConnectionInformation info)
        {
            var local = OnEndpointBecomingAvailable;
            if (local != null)
            {
                local(this, new ConnectionInformationEventArgs(info));
            }
        }

        /// <summary>
        /// An event raised when a remote endpoint becomes unavailable.
        /// </summary>
        public event EventHandler<EndpointEventArgs> OnEndpointBecomingUnavailable;

        private void RaiseOnEndpointBecomingUnavailable(EndpointId id)
        {
            var local = OnEndpointBecomingUnavailable;
            if (local != null)
            {
                local(this, new EndpointEventArgs(id));
            }
        }

        /// <summary>
        /// Starts the endpoint discovery process.
        /// </summary>
        public void StartDiscovery()
        {
            m_IsDiscoveryAllowed = true;
        }

        /// <summary>
        /// Ends the endpoint discovery process.
        /// </summary>
        public void EndDiscovery()
        {
            m_IsDiscoveryAllowed = false;
        }

        /// <summary>
        /// Stores or forwards information about an endpoint that has recently
        /// connected to the network.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        /// <param name="channelType">
        ///     The type of the <see cref="IChannelType"/> which indicates which kind of channel this connection
        ///     information describes.
        /// </param>
        /// <param name="address">The full URI for the channel.</param>
        public void RecentlyConnectedEndpoint(EndpointId id, Type channelType, Uri address)
        {
            if (!m_IsDiscoveryAllowed)
            {
                return;
            }

            RaiseOnEndpointBecomingAvailable(new ChannelConnectionInformation(id, channelType, address));
        }

        /// <summary>
        /// Stores or forwards information about an endpoint that has recently
        /// disconnected from the network.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        public void RecentlyDisconnectedEndpoint(EndpointId endpoint)
        {
            if (!m_IsDiscoveryAllowed)
            {
                return;
            }

            RaiseOnEndpointBecomingUnavailable(endpoint);
        }
    }
}
