//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;
using Apollo.Utilities.Licensing;

namespace Apollo.Core.Utilities.Licensing
{
    /// <summary>
    /// Stores the latest license validation results.
    /// </summary>
    internal class LicenseValidationResultStorage : IValidationResultStorage
    {
        /// <summary>
        /// The object used to take out locks on.
        /// </summary>
        private readonly LockObject m_Lock = new LockObject();

        /// <summary>
        /// The current license check result.
        /// </summary>
        private LicenseCheckResult m_Result;

        /// <summary>
        /// Used to store the latest license validation results.
        /// </summary>
        /// <param name="checksum">The checksum that was generated from the result.</param>
        /// <param name="generationTime">The time at which the result was generated.</param>
        /// <param name="expirationTime">The time after which the current result is considered expired.</param>
        public void StoreLicenseValidationResult(Checksum checksum, DateTimeOffset generationTime, DateTimeOffset expirationTime)
        {
            var result = new LicenseCheckResult(generationTime, expirationTime, checksum);
            lock (m_Lock)
            {
                m_Result = result;
            }
        }

        /// <summary>
        /// Gets the latest license validation result.
        /// </summary>
        public LicenseCheckResult LatestResult
        {
            get
            {
                lock (m_Lock)
                {
                    return m_Result; 
                }
            }
        }
    }
}
