//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Defines the interface for objects that form a proxy between two
    /// <see cref="ILicenseValidationCache"/> objects.
    /// </summary>
    internal interface ILicenseValidationCacheProxy
    {
        /// <summary>
        /// Gets the latest verification result.
        /// </summary>
        /// <value>The latest result.</value>
        LicenseCheckResult LatestResult
        {
            get;
        }
    }
}
