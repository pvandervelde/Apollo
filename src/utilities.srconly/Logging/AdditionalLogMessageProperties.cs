//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities.Logging
{
    /// <summary>
    /// Defines constants for possible additional log message properties.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class AdditionalLogMessageProperties
    {
        /// <summary>
        /// Defines the property key for information about an EventLog event ID.
        /// </summary>
        public const string EventId = "EventId";

        /// <summary>
        /// Defines the poperty key for information about an EventLog event category.
        /// </summary>
        public const string EventCategory = "EventCategory";
    }
}
