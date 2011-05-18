//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Implements <see cref="ISendingEndpoint"/> to allow sending messages to a given endpoint.
    /// </summary>
    internal sealed class SendingEndpoint : ISendingEndpoint
    {
        /// <summary>
        /// The collection that maps between the endpoint ID and the channel that is used to
        /// send messages to the given endpoint.
        /// </summary>
        private readonly Dictionary<EndpointId, IChannelProxy> m_EndpointMap =
            new Dictionary<EndpointId, IChannelProxy>();

        /// <summary>
        /// The function that is used to retrieve the channel for a given endpoint.
        /// </summary>
        private readonly Func<EndpointId, IChannelProxy> m_ProxyBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendingEndpoint"/> class.
        /// </summary>
        /// <param name="proxyBuilder">
        /// The function used to create new channels for a given endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxyBuilder"/> is <see langword="null" />.
        /// </exception>
        public SendingEndpoint(Func<EndpointId, IChannelProxy> proxyBuilder)
        {
            {
                Enforce.Argument(() => proxyBuilder);
            }

            m_ProxyBuilder = proxyBuilder;
        }

        /// <summary>
        /// Returns the collection of known endpoints.
        /// </summary>
        /// <returns>
        /// The collection of known endpoints.
        /// </returns>
        public IEnumerable<EndpointId> KnownEndpoints()
        {
            return m_EndpointMap.Keys;
        }

        /// <summary>
        /// Sends the given message.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="message">The message to be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public void Send(EndpointId endpoint, ICommunicationMessage message)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.Argument(() => message);
            }

            if (!m_EndpointMap.ContainsKey(endpoint))
            {
                m_EndpointMap.Add(endpoint, m_ProxyBuilder(endpoint));
            }

            var channel = m_EndpointMap[endpoint];
            channel.Send(message);
        }

        /// <summary>
        /// Closes the channel that connects to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint to which the connection should be closed.</param>
        public void CloseChannelTo(EndpointId endpoint)
        {
            if (!m_EndpointMap.ContainsKey(endpoint))
            {
                return;
            }

            var channel = m_EndpointMap[endpoint];
            m_EndpointMap.Remove(endpoint);
            channel.Dispose();
        }
    }
}
