//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Lokad;
using NLog;

namespace Apollo.Utilities.Logging
{
    /// <summary>
    /// Defines a logging object that translates <see cref="ILogMessage"/> objects and
    /// writes them to a log.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class Logger : ILogger
    {
        /// <summary>
        /// Translates from nlog level to the apollo log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// The <see cref="LevelToLog"/>.
        /// </returns>
        private static LevelToLog TranslateFromNlogLevel(NLog.Logger logger)
        {
            if (logger.IsTraceEnabled)
            {
                return LevelToLog.Trace;
            }

            if (logger.IsDebugEnabled)
            {
                return LevelToLog.Debug;
            }

            if (logger.IsInfoEnabled)
            {
                return LevelToLog.Info;
            }

            if (logger.IsWarnEnabled)
            {
                return LevelToLog.Warn;
            }

            if (logger.IsErrorEnabled)
            {
                return LevelToLog.Error;
            }

            return logger.IsFatalEnabled ? LevelToLog.Fatal : LevelToLog.None;
        }

        /// <summary>
        /// Translates from apollo log level to nlog level.
        /// </summary>
        /// <param name="levelToLog">The log level.</param>
        /// <returns>
        /// The <see cref="NLog.LogLevel"/>.
        /// </returns>
        private static NLog.LogLevel TranslateToNlogLevel(LevelToLog levelToLog)
        {
            switch (levelToLog)
            {
                case LevelToLog.Trace:
                    return NLog.LogLevel.Trace;
                case LevelToLog.Debug:
                    return NLog.LogLevel.Debug;
                case LevelToLog.Info:
                    return NLog.LogLevel.Info;
                case LevelToLog.Warn:
                    return NLog.LogLevel.Warn;
                case LevelToLog.Error:
                    return NLog.LogLevel.Error;
                case LevelToLog.Fatal:
                    return NLog.LogLevel.Fatal;
                case LevelToLog.None:
                    return NLog.LogLevel.Off;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The log factory that is used to create the logger.
        /// </summary>
        private readonly LogFactory m_Factory;

        /// <summary>
        /// The logger that performs the actual logging of the log messages.
        /// </summary>
        private readonly NLog.Logger m_Logger;

        /// <summary>
        /// The log template that is used to translate log messages.
        /// </summary>
        private readonly ILogTemplate m_Template;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="factory">The log factory.</param>
        /// <param name="template">The template.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="template"/> is <see langword="null"/>.
        /// </exception>
        internal Logger(LogFactory factory, ILogTemplate template)
        {
            {
                Enforce.Argument(() => factory);
                Enforce.Argument(() => template);
            }

            m_Template = template;
            m_Factory = factory;
            {
                m_Factory.GlobalThreshold = TranslateToNlogLevel(template.DefaultLogLevel());
            }

            m_Logger = m_Factory.GetLogger(template.Name);
        }

        /// <summary>
        /// Gets or sets the current <see cref="LevelToLog"/>.
        /// </summary>
        public LevelToLog Level
        {
            get
            {
                return TranslateFromNlogLevel(m_Logger);
            }

            set
            {
                var nlogLevel = TranslateToNlogLevel(value);
                m_Factory.GlobalThreshold = nlogLevel;
            }
        }

        /// <summary>
        /// Indicates if a message will be written to the log file based on the
        /// current log level and the level of the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// <see langword="true" /> if the message will be logged; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ShouldLog(ILogMessage message)
        {
            if (Level == LevelToLog.None)
            {
                return false;
            }

            if (message == null)
            {
                return false;
            }

            if (message.Level == LevelToLog.None)
            {
                return false;
            }

            return message.Level >= Level;
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(ILogMessage message)
        {
            if (!ShouldLog(message))
            {
                return;
            }

            var level = TranslateToNlogLevel(message.Level);
            var info = new LogEventInfo(level, m_Template.Name, m_Template.Translate(message));
            if (message.HasAdditionalInformation)
            {
                foreach (var pair in message.Properties)
                {
                    info.Context[pair.Key] = pair.Value;
                }
            }

            m_Logger.Log(info);
            m_Logger.Factory.Flush();
        }

        /// <summary>
        /// Stops the logger and ensures that all log messages have been 
        /// saved to the log.
        /// </summary>
        public void Close()
        {
            m_Logger.Factory.Flush();
        }

        /// <summary>
        ///  Performs application-defined tasks associated with freeing, releasing, or
        ///  resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}
