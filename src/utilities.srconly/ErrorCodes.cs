//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines a set of standard error codes that can be used with rapid application
    /// shut down cases.
    /// </summary>
    internal static class ErrorCodes
    {
        /// <summary>
        /// The error code that is used for errors that indicate that the
        /// validation routines failed to set a watch dog flag.
        /// </summary>
        public const string ValidationWatchdogSetFailure = @"VWD-001";

        /// <summary>
        /// The error code that is used for errors that indicate that the 
        /// validation routines failed several times in a row.
        /// </summary>
        public const string ValidationCannotValidateWithoutTimePeriods = @"VWP-001";

        /// <summary>
        /// The error code that is used for errors that indicate that the 
        /// validation routines failed several times in a row.
        /// </summary>
        public const string ValidationExceededMaximumSequentialFailures = @"VCF-001";
    }
}
