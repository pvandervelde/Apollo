//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Properties;
using Lokad;

namespace Apollo.Core.Logging
{
    /// <content>
    /// Defines the <see cref="ILogSink"/> members.
    /// </content>
    internal sealed partial class LogSink
    {
        /// <summary>
        /// The collection of loggers.
        /// </summary>
        private readonly Dictionary<LogType, ILogger> m_Loggers = new Dictionary<LogType, ILogger>();

        #region Implementation of ILogSink

        /// <summary>
        /// Returns the log level for the specified log type.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <returns>
        /// The log level for the specified log type.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when there is no registered logger for the given <paramref name="logType"/>.
        /// </exception>
        public LevelToLog Level(LogType logType)
        {
            {
                Enforce.With<ArgumentException>(m_Loggers.ContainsKey(logType), Resources_NonTranslatable.Exceptions_Messages_LogTypeHasNoLogger_WithLogType, logType);
            }

            return m_Loggers[logType].Level;
        }

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
        public bool ShouldLogMessage(LogType logType, ILogMessage message)
        {
            return m_Loggers.ContainsKey(logType) && m_Loggers[logType].ShouldLog(message);
        }

        /// <summary>
        /// Logs the specified message if the log level of the message is higher or equal to
        /// the current log level.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when there is no registered logger for the given <paramref name="logType"/>.
        /// </exception>
        public void Log(LogType logType, ILogMessage message)
        {
            {
                Enforce.With<ArgumentException>(m_Loggers.ContainsKey(logType), Resources_NonTranslatable.Exceptions_Messages_LogTypeHasNoLogger_WithLogType, logType);
            }

            m_Loggers[logType].Log(message);
        }

        #endregion
    }
}
