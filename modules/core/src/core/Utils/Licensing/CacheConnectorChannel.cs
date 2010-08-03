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
    /// Connects the different <see cref="ICacheConnectorChannelEndPoint"/> objects from 
    /// around the application.
    /// </summary>
    internal sealed class CacheConnectorChannel : ICacheConnectorChannel
    {
        /// <summary>
        /// The collection of endpoints that are known.
        /// </summary>
        private readonly Dictionary<AppDomain, ICacheConnectorChannelEndPoint> m_EndPoints =
            new Dictionary<AppDomain, ICacheConnectorChannelEndPoint>();

        /// <summary>
        /// Connects the channel to the given endpoint.
        /// </summary>
        /// <param name="appDomainForEndPoint">The <see cref="AppDomain"/> for the endpoint that should be added.</param>
        /// <param name="endPoint">The endpoint to which the channel is connected.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="appDomainForEndPoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endPoint"/> is <see langword="null" />.
        /// </exception>
        public void ConnectTo(AppDomain appDomainForEndPoint, ICacheConnectorChannelEndPoint endPoint)
        {
            {
                Enforce.Argument(() => appDomainForEndPoint);
                Enforce.Argument(() => endPoint);
            }

            if (m_EndPoints.ContainsKey(appDomainForEndPoint))
            {
                return;
            }

            foreach (var pair in m_EndPoints)
            {
                // Connect all the existing endpoints to the proxy of the new one
                var existingEndPoint = pair.Value;
                existingEndPoint.Connect(appDomainForEndPoint, endPoint.LocalProxy());

                // Connect the new endpoint to the proxies of the existing endpoints
                var existingAppDomain = pair.Key;
                endPoint.Connect(existingAppDomain, existingEndPoint.LocalProxy());
            }

            m_EndPoints.Add(appDomainForEndPoint, endPoint);
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

            if (!m_EndPoints.ContainsKey(appDomainForEndpoint))
            {
                return;
            }

            // Remove the endpoint from the collection
            var endPoint = m_EndPoints[appDomainForEndpoint];
            m_EndPoints.Remove(appDomainForEndpoint);

            // Disconnect all the existing endpoints from the proxy of the one 
            // that will be removed
            foreach (var pair in m_EndPoints)
            {
                var existingEndPoint = pair.Value;
                existingEndPoint.Disconnect(appDomainForEndpoint);

                endPoint.Disconnect(pair.Key);
            }
        }
    }
}
