//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Defines the period over which a license verification check
    /// should be repeated.
    /// </summary>
    internal enum RepeatPeriod
    {
        /// <summary>
        /// The check should be repeated on an hourly basis.
        /// </summary>
        Hourly,

        /// <summary>
        /// The check should be repeated on an daily basis.
        /// </summary>
        Daily,

        /// <summary>
        /// The check should be repeated on an weekly basis.
        /// </summary>
        Weekly,

        /// <summary>
        /// The check should be repeated on an fortnightly basis.
        /// </summary>
        Fortnightly,

        /// <summary>
        /// The check should be repeated on an monthly basis.
        /// </summary>
        Monthly,

        /// <summary>
        /// The check should be repeated on an yearly basis.
        /// </summary>
        Yearly,
    }
}
