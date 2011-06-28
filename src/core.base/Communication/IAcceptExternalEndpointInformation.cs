//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that store or forward information about
    /// the connection state of external endpoints.
    /// </summary>
    public interface IAcceptExternalEndpointInformation
    {
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
        void RecentlyConnectedEndpoint(EndpointId id, Type channelType, Uri address);

        /// <summary>
        /// Stores or forwards information about an endpoint that has recently
        /// disconnected from the network.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        void RecentlyDisconnectedEndpoint(EndpointId endpoint);
    }
}
