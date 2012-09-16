//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utilities;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Defines the interface for objects that remote logging calls.
    /// </summary>
    internal interface ILogMessagesFromRemoteAppdomains
    {
        /// <summary>
        /// Logs the given message with the given severity.
        /// </summary>
        /// <param name="severity">The importance of the log message.</param>
        /// <param name="message">The message.</param>
        void Log(LogSeverityProxy severity, string message);
    }
}
