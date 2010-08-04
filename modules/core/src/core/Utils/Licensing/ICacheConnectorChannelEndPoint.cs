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
    /// Defines the interface for objects that connect different <see cref="ILicenseValidationCache"/> object
    /// through their proxies.
    /// </summary>
    internal interface ICacheConnectorChannelEndpoint
    {
        /// <summary>
        /// Returns the <see cref="ILicenseValidationCacheProxy"/> for the <see cref="ILicenseValidationCache"/> that
        /// lives in the same <see cref="AppDomain"/> as the current endpoint.
        /// </summary>
        /// <returns>
        /// A proxy to the local cache.
        /// </returns>
        ILicenseValidationCacheProxy LocalProxy();

        /// <summary>
        /// Connects the local <see cref="ILicenseValidationCache"/> with the given proxy.
        /// </summary>
        /// <param name="cacheDomain">The <see cref="AppDomain"/> where the proxy came from.</param>
        /// <param name="proxy">The proxy.</param>
        void Connect(AppDomain cacheDomain, ILicenseValidationCacheProxy proxy);

        /// <summary>
        /// Disconnects the local <see cref="ILicenseValidationCache"/> from the proxy that is
        /// linked to the given <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="cacheDomain">The domain that is about to be disconnected.</param>
        void Disconnect(AppDomain cacheDomain);
    }
}
