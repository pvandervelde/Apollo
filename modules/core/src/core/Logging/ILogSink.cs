//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines the interface for <see cref="KernelService"/> objects that handle logging.
    /// </summary>
    internal interface ILogSink
    {
        /// <summary>
        /// Returns the log level for the specified log type.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <returns>
        /// The log level for the specified log type.
        /// </returns>
        LogLevel Level(LogType logType);

        /// <summary>
        /// Indicates if the log message will be logged, depending on the 
        /// current log level.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///     <see langword="true" /> if the message will be placed in the log; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ShouldLogMessage(LogType logType, ILogMessage message);

        /// <summary>
        /// Logs the specified message if the log level of the message is higher or equal to
        /// the current log level.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        void Log(LogType logType, ILogMessage message);
    }
}
