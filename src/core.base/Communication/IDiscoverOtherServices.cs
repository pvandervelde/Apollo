//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the methods for objects that handle discovery of endpoints.
    /// </summary>
    internal interface IDiscoverOtherServices
    {
        /// <summary>
        /// An event raised when a remote endpoint becomes available.
        /// </summary>
        event EventHandler<ConnectionInformationEventArgs> OnEndpointBecomingAvailable;

        /// <summary>
        /// An event raised when a remote endpoint becomes unavailable.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnEndpointBecomingUnavailable;

        /// <summary>
        /// Starts the endpoint discovery process.
        /// </summary>
        void StartDiscovery();

        /// <summary>
        /// Ends the endpoint discovery process.
        /// </summary>
        void EndDiscovery();
    }
}
