//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities.Licensing
{
    /// <summary>
    /// Stores constants used by the licensing classes.
    /// </summary>
    internal static class LicensingConstants
    {
        /// <summary>
        /// The interval, in milliseconds, between is-alive events.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Milli",
            Justification = "It seems FxCop can't spell.")]
        public const int LicenseWatchdogIntervalInMilliseconds = 3 * 10 * 1000;

        /// <summary>
        /// The maximum number of sequential failures that we allow to happen 
        /// while trying to verify the license.
        /// </summary>
        public const int MaximumNumberOfSequentialValidationFailures = 3;

        /// <summary>
        /// The maximum number of ticks that a time can be 'off' from the desired
        /// value.
        /// </summary>
        public const int ThresholdForTimeDeviationsInTicks = 100;
    }
}
