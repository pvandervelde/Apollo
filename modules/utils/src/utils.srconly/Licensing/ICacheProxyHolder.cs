//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Defines the interface for objects that store one or more
    /// <see cref="ILicenseVerificationCacheProxy"/> objects.
    /// </summary>
    internal interface ICacheProxyHolder
    {
        /// <summary>
        /// Stores the specified proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        void Store(ILicenseVerificationCacheProxy proxy);
    }
}
