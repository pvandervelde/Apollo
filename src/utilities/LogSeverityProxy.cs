//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines an enumeration of different log levels.
    /// </summary>
    /// <remarks>
    /// This enumeration forms a proxy or facade for the log level
    /// defined in the Apollo.Utilities.SrcOnly assembly. We
    /// can't directly link to the logging components in that assembly
    /// because we can't pull any of the logging elements into the
    /// Apollo.Utilities assembly. By leaving the logging elements in
    /// Apollo.Utilities.SrcOnly we ensure that the exception handler can always
    /// access the logger without having to load any assemblies (and thus
    /// maybe causing assembly resolve exceptions.
    /// </remarks>
    public enum LogSeverityProxy
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
        Warning = 3,

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
