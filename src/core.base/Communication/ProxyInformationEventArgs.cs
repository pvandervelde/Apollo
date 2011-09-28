//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Stores <see cref="EventArgs"/> describing the registration of a new proxy on a
    /// remote endpoint.
    /// </summary>
    internal sealed class ProxyInformationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyInformationEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint on which the proxy was registered.</param>
        /// <param name="proxyType">The command that was registered.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxyType"/> is <see langword="null" />.
        /// </exception>
        public ProxyInformationEventArgs(EndpointId endpoint, ISerializedType proxyType)
        {
            {
                Enforce.Argument(() => endpoint);
                Enforce.Argument(() => proxyType);
            }

            Endpoint = endpoint;
            Proxy = proxyType;
        }

        /// <summary>
        /// Gets the ID number of the remote endpoint.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the proxy that was registered on the remote endpoint.
        /// </summary>
        public ISerializedType Proxy
        {
            get;
            private set;
        }
    }
}
