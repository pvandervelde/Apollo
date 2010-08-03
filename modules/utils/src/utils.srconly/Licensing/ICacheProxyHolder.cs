//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Defines the interface for objects that store one or more
    /// <see cref="ILicenseValidationCacheProxy"/> objects.
    /// </summary>
    internal interface ICacheProxyHolder
    {
        /// <summary>
        /// Stores the specified proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        void Store(ILicenseValidationCacheProxy proxy);

        /// <summary>
        /// Releases the specified proxy from storage.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        void Release(ILicenseValidationCacheProxy proxy);
    }
}
