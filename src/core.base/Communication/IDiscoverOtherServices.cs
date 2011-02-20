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
        /// Announces to the network that the current endpoint is available.
        /// </summary>
        void AnnounceLogOn();

        /// <summary>
        /// Announces to the network that the current endpoint is not available.
        /// </summary>
        void AnnounceLogOff();
    }
}
