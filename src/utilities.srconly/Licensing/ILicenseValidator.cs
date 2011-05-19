//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities.Licensing
{
    /// <summary>
    /// Defines the interface for objects that validate the license key or file for
    /// validity.
    /// </summary>
    internal interface ILicenseValidator
    {
        /// <summary>
        /// Verifies the license and stores a value based on the 
        /// license validity and the checksum.
        /// </summary>
        void Verify();

        /// <summary>
        /// Verifies the license and stores a value based on the
        /// license validity and the checksum.
        /// </summary>
        /// <param name="nextExpiration">
        /// The <see cref="TimePeriod"/> that must occur before the validated
        /// license check expires.
        /// </param>
        void Verify(TimePeriod nextExpiration);
    }
}
