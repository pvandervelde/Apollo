//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines the level of a log message. Messages with a level lower than the 
    /// current level of the logger will be ignored.
    /// </summary>
    /// <design>
    /// Note that the enum values are not powers of two because it is not
    /// possible to combine different levels as a bit mask.
    /// </design>
    internal enum LevelToLog
    {
        /// <summary>
        /// The message describes trace information.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// The message describes debug information.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// The message describes some information.
        /// </summary>
        Info = 2,

        /// <summary>
        /// The message describes a situation that can potentially 
        /// lead to errors.
        /// </summary>
        Warn = 3,

        /// <summary>
        /// The message describes an (non-fatal) error condition.
        /// </summary>
        Error = 4,

        /// <summary>
        /// The message describes a fatal error condition.
        /// </summary>
        Fatal = 5,

        /// <summary>
        /// The message has no level.
        /// </summary>
        None = 6,
    }
}
