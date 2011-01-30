//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines a <see cref="KernelService"/> that handles logging for the system.
    /// </summary>
    internal sealed partial class LogSink : KernelService, ILogSink, IHaveServiceDependencies
    {
        /// <summary>
        /// The collection of loggers.
        /// </summary>
        private readonly Dictionary<LogType, ILogger> m_Loggers = new Dictionary<LogType, ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogSink"/> class.
        /// </summary>
        /// <param name="configuration">The log configuration.</param>
        /// <param name="debugTemplate">The debug template.</param>
        /// <param name="commandTemplate">The command template.</param>
        /// <param name="fileConstants">The object containing constant values describing file and file paths.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="configuration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="debugTemplate"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="commandTemplate"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="fileConstants"/> is <see langword="null"/>.
        /// </exception>
        public LogSink(
            ILoggerConfiguration configuration, 
            DebugLogTemplate debugTemplate, 
            CommandLogTemplate commandTemplate,
            IFileConstants fileConstants)
            : base()
        {
            {
                Enforce.Argument(() => configuration);
                Enforce.Argument(() => debugTemplate);
                Enforce.Argument(() => commandTemplate);
                Enforce.Argument(() => fileConstants);
            }

            // Add the loggers to the collection
            {
                m_Loggers.Add(LogType.Debug, new Logger(configuration, debugTemplate, fileConstants));
                m_Loggers.Add(LogType.Command, new Logger(configuration, commandTemplate, fileConstants));
            }
        }

        #region Overrides

        /// <summary>
        /// Performs the tasks necessary to start the log service.
        /// </summary>
        protected override void StartService()
        {
            LogWithoutVerifying(LogType.Debug, new LogMessage(GetType().FullName, LevelToLog.Info, Resources_NonTranslatable.LogSink_LogMessage_LoggersStarted));
        }

        /// <summary>
        /// Performs the tasks necessary to stop the log service.
        /// </summary>
        protected override void StopService()
        {
            LogWithoutVerifying(LogType.Debug, new LogMessage(GetType().FullName, LevelToLog.Info, Resources_NonTranslatable.LogSink_LogMessage_LoggersStopped));

            // Inform all the loggers that the system is being stopped. This allows
            // the loggers to flush all the buffers.
            foreach (var pair in m_Loggers)
            {
                pair.Value.Stop();
            }
        }

        #endregion

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
                Enforce.With<ArgumentException>(m_Loggers.ContainsKey(logType), Resources_NonTranslatable.Exception_Messages_LogTypeHasNoLogger_WithLogType, logType);
            }

            return m_Loggers[logType].Level;
        }

        /// <summary>
        /// Sets the log level for the specified log type.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="newLevel">The new log level.</param>
        public void Level(LogType logType, LevelToLog newLevel)
        {
            {
                Enforce.With<ArgumentException>(m_Loggers.ContainsKey(logType), Resources_NonTranslatable.Exception_Messages_LogTypeHasNoLogger_WithLogType, logType);
            }

            m_Loggers[logType].ChangeLevel(newLevel);
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
            if (!IsFullyFunctional)
            {
                return false;
            }

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
        /// <exception cref="ArgumentException">
        /// Thrown when the service is not fully functional.
        /// </exception>
        public void Log(LogType logType, ILogMessage message)
        {
            {
                Enforce.With<ArgumentException>(m_Loggers.ContainsKey(logType), Resources_NonTranslatable.Exception_Messages_LogTypeHasNoLogger_WithLogType, logType);
                Enforce.With<ArgumentException>(IsFullyFunctional, Resources_NonTranslatable.Exception_Messages_ServicesIsNotFullyFunctional, StartupState);
            }

            LogWithoutVerifying(logType, message);
        }

        /// <summary>
        /// Logs the given message without doing any verification of the Logger state.
        /// </summary>
        /// <design>
        /// This method should never be called without either verifying the state or verifying that it
        /// is safe the perform the log action.
        /// </design>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        private void LogWithoutVerifying(LogType logType, ILogMessage message)
        {
            m_Loggers[logType].Log(message);
        }

        #endregion
    }
}
