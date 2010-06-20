//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Defines the interface for objects that store license verification results.
    /// </summary>
    internal interface ILicenseVerificationCache
    {
        /// <summary>
        /// Gets the latest verification result.
        /// </summary>
        /// <value>The latest result.</value>
        LicenseCheckResult LatestResult 
        {
            get;
        }

        /// <summary>
        /// Gets the last verification time.
        /// </summary>
        /// <value>The last verification time.</value>
        DateTimeOffset LastVerificationTime
        {
            get;
        }

        /// <summary>
        /// Invalidates the cache and gets a new <see cref="LicenseCheckResult"/> with the specified expiration time.
        /// </summary>
        /// <param name="nextExpiration">
        /// The <see cref="TimePeriod"/> that must occur before the validated
        /// license check expires.
        /// </param>
        void Invalidate(TimePeriod nextExpiration);

        /// <summary>
        /// Creates a new <see cref="ILicenseVerificationCacheProxy"/> object with the
        /// current cache as owner.
        /// </summary>
        /// <returns>
        ///     A new <see cref="ILicenseVerificationCacheProxy"/> object.
        /// </returns>
        ILicenseVerificationCacheProxy CreateNewProxy();
    }
}
