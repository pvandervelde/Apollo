//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Castle.Core.Logging;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using ILogger = Castle.Core.Logging.ILogger;

namespace Test.Regression.Explorer
{
    /// <summary>
    /// Redirects 'White' log statements to the application log.
    /// </summary>
    internal sealed class WhiteLogRedirector : LevelFilteredLogger
    {
        private static LevelToLog TranslateLogLevel(LoggerLevel loggerLevel)
        {
            switch (loggerLevel)
            {
                case LoggerLevel.Off:
                    return LevelToLog.None;
                case LoggerLevel.Fatal:
                    return LevelToLog.Fatal;
                case LoggerLevel.Error:
                    return LevelToLog.Error;
                case LoggerLevel.Warn:
                    return LevelToLog.Warn;
                case LoggerLevel.Info:
                    return LevelToLog.Info;
                case LoggerLevel.Debug:
                    return LevelToLog.Debug;
                default:
                    return LevelToLog.Trace;
            }
        }

        /// <summary>
        /// The object that provides the log methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhiteLogRedirector"/> class.
        /// </summary>
        /// <param name="name">The name for the logger.</param>
        /// <param name="diagnostics">The object that provides the logging for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public WhiteLogRedirector(string name, SystemDiagnostics diagnostics)
            : base(name)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WhiteLogRedirector"/> class.
        /// </summary>
        /// <param name="name">The name for the logger.</param>
        /// <param name="level">The default log level.</param>
        /// <param name="diagnostics">The object that provides the logging for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public WhiteLogRedirector(string name, LoggerLevel level, SystemDiagnostics diagnostics)
            : base(name, level)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Create a new child logger.
        /// </summary>
        /// <param name="loggerName">The name of this logger.</param>
        /// <returns>
        /// The New ILogger instance.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">If the name has an empty element name.</exception>
        public override ILogger CreateChildLogger(string loggerName)
        {
            return new WhiteLogRedirector(loggerName, m_Diagnostics);
        }

        /// <summary>
        /// Outputs the log content.
        /// </summary>
        /// <param name="loggerLevel">The logger level.</param>
        /// <param name="loggerName">The name of the logger.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception. Maybe <see langword="null" />.</param>
        protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
        {
            m_Diagnostics.Log(
                TranslateLogLevel(loggerLevel),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "White - {0}",
                    message));

            if (exception != null)
            {
                m_Diagnostics.Log(
                    TranslateLogLevel(loggerLevel),
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "White - {0}",
                        exception));
            }
        }
    }
}
