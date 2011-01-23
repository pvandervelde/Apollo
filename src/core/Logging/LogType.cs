//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines the types of logs that can be written.
    /// </summary>
    internal enum LogType
    {
        /// <summary>
        /// No log type is defined.
        /// </summary>
        None,

        /// <summary>
        /// The log is used to store debug information.
        /// </summary>
        Debug,

        /// <summary>
        /// The log is used to store command history information.
        /// </summary>
        Command,
    }
}
