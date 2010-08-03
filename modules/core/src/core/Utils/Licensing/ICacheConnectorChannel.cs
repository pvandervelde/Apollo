//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils.Licensing;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// Defines the interface for objects that handle passing <see cref="ILicenseValidationCacheProxy"/> objects
    /// between different <see cref="AppDomain"/>s.
    /// </summary>
    internal interface ICacheConnectorChannel
    {
        /// <summary>
        /// Connects the channel to the given endpoint.
        /// </summary>
        /// <param name="appDomainForEndPoint">The <see cref="AppDomain"/> for the endpoint that should be added.</param>
        /// <param name="endPoint">The endpoint to which the channel is connected.</param>
        void ConnectTo(AppDomain appDomainForEndPoint, ICacheConnectorChannelEndPoint endPoint);

        /// <summary>
        /// Disconnects the channel from the given endpoint.
        /// </summary>
        /// <param name="appDomainForEndPoint">The <see cref="AppDomain"/> for the endpoint that should be removed.</param>
        void DisconnectFrom(AppDomain appDomainForEndPoint);
    }
}
