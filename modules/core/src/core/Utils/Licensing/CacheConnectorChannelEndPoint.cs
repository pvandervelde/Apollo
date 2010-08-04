//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Utils.Licensing;
using Lokad;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// Used by the <see cref="Bootstrapper"/> to connect the different implementations of
    /// the <see cref="ICacheProxyHolder"/>.
    /// </summary>
    internal sealed class CacheConnectorChannelEndpoint : MarshalByRefObject, ICacheConnectorChannelEndpoint
    {
        /// <summary>
        /// The collection that maps proxies to their <see cref="AppDomain"/>s.
        /// </summary>
        private readonly Dictionary<AppDomain, ILicenseValidationCacheProxy> m_KnownProxies
            = new Dictionary<AppDomain, ILicenseValidationCacheProxy>();

        /// <summary>
        /// The function that is used to get the proxy that belongs to the local cache.
        /// </summary>
        private readonly Func<ILicenseValidationCacheProxy> m_ProxyFactory;

        /// <summary>
        /// The <see cref="ICacheProxyHolder"/> that lives in the same <see cref="AppDomain"/>
        /// as the current instance of this class.
        /// </summary>
        private readonly ICacheProxyHolder m_LocalCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheConnectorChannelEndpoint"/> class.
        /// </summary>
        /// <param name="proxyFactory">The function that is used to get the proxy that belongs to the local cache.</param>
        /// <param name="localCache">
        ///     The <see cref="ICacheProxyHolder"/> that lives in the same <see cref="AppDomain"/> as the current instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localCache"/> is <see langword="null" />.
        /// </exception>
        public CacheConnectorChannelEndpoint(Func<ILicenseValidationCacheProxy> proxyFactory, ICacheProxyHolder localCache)
        {
            {
                Enforce.Argument(() => proxyFactory);
                Enforce.Argument(() => localCache);
            }

            m_ProxyFactory = proxyFactory;
            m_LocalCache = localCache;
        }

        /// <summary>
        /// Returns the <see cref="ILicenseValidationCacheProxy"/> for the <see cref="ILicenseValidationCache"/> that
        /// lives in the same <see cref="AppDomain"/> as the current endpoint.
        /// </summary>
        /// <returns>
        /// A proxy to the local cache.
        /// </returns>
        public ILicenseValidationCacheProxy LocalProxy()
        {
            return m_ProxyFactory();
        }

        /// <summary>
        /// Connects the local <see cref="ICacheProxyHolder"/> with the given proxy.
        /// </summary>
        /// <param name="cacheDomain">The <see cref="AppDomain"/> where the proxy came from.</param>
        /// <param name="proxy">The proxy.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="cacheDomain"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxy"/> is <see langword="null" />.
        /// </exception>
        public void Connect(AppDomain cacheDomain, ILicenseValidationCacheProxy proxy)
        {
            {
                Enforce.Argument(() => cacheDomain);
                Enforce.Argument(() => proxy);
            }

            if (m_KnownProxies.ContainsKey(cacheDomain))
            {
                return;
            }

            m_KnownProxies.Add(cacheDomain, proxy);
            m_LocalCache.Store(proxy);
        }

        /// <summary>
        /// Disconnects the local <see cref="ICacheProxyHolder"/> from the proxy that is
        /// linked to the given <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="cacheDomain">The domain that is about to be disconnected.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="cacheDomain"/> is <see langword="null" />.
        /// </exception>
        public void Disconnect(AppDomain cacheDomain)
        {
            {
                Enforce.Argument(() => cacheDomain);
            }

            if (m_KnownProxies.ContainsKey(cacheDomain))
            {
                m_LocalCache.Release(m_KnownProxies[cacheDomain]);
                m_KnownProxies.Remove(cacheDomain);
            }
        }
    }
}
