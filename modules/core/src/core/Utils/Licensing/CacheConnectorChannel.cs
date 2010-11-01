//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Lokad;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// Connects the different <see cref="ICacheConnectorChannelEndpoint"/> objects from 
    /// around the application.
    /// </summary>
    internal sealed class CacheConnectorChannel : MarshalByRefObject, ICacheConnectorChannel
    {
        /// <summary>
        /// The collection of endpoints that are known.
        /// </summary>
        private readonly Dictionary<AppDomain, ICacheConnectorChannelEndpoint> m_Endpoints =
            new Dictionary<AppDomain, ICacheConnectorChannelEndpoint>();

        /// <summary>
        /// Connects the channel to the given endpoint.
        /// </summary>
        /// <param name="appDomainForEndpoint">The <see cref="AppDomain"/> for the endpoint that should be added.</param>
        /// <param name="endpoint">The endpoint to which the channel is connected.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="appDomainForEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        public void ConnectTo(AppDomain appDomainForEndpoint, ICacheConnectorChannelEndpoint endpoint)
        {
            {
                Enforce.Argument(() => appDomainForEndpoint);
                Enforce.Argument(() => endpoint);
            }

            if (m_Endpoints.ContainsKey(appDomainForEndpoint))
            {
                return;
            }

            foreach (var pair in m_Endpoints)
            {
                // Connect all the existing endpoints to the proxy of the new one
                var existingEndpoint = pair.Value;
                existingEndpoint.Connect(appDomainForEndpoint, endpoint.LocalProxy());

                // Connect the new endpoint to the proxies of the existing endpoints
                var existingAppDomain = pair.Key;
                endpoint.Connect(existingAppDomain, existingEndpoint.LocalProxy());
            }

            m_Endpoints.Add(appDomainForEndpoint, endpoint);
        }

        /// <summary>
        /// Disconnects the channel from the given endpoint.
        /// </summary>
        /// <param name="appDomainForEndpoint">The <see cref="AppDomain"/> for the endpoint that should be removed.</param>
        public void DisconnectFrom(AppDomain appDomainForEndpoint)
        {
            {
                Enforce.Argument(() => appDomainForEndpoint);
            }

            if (!m_Endpoints.ContainsKey(appDomainForEndpoint))
            {
                return;
            }

            // Remove the endpoint from the collection
            var endpoint = m_Endpoints[appDomainForEndpoint];
            m_Endpoints.Remove(appDomainForEndpoint);

            // Disconnect all the existing endpoints from the proxy of the one 
            // that will be removed
            foreach (var pair in m_Endpoints)
            {
                var existingEndpoint = pair.Value;
                existingEndpoint.Disconnect(appDomainForEndpoint);

                endpoint.Disconnect(pair.Key);
            }
        }
    }
}
