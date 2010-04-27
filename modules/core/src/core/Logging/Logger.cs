//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.Utils;
using Lokad;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines a logging object that translates <see cref="ILogMessage"/> objects and
    /// writes them to a log.
    /// </summary>
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
            switch(levelToLog)
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
        /// Builds the Nlog configuration from the <see cref="ILoggerConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="template">The template.</param>
        /// <param name="constants">The constants that describe file and file path values.</param>
        /// <returns>A new NLog logger configuration.</returns>
        private static LoggingConfiguration BuildNLogConfiguration(ILoggerConfiguration configuration, ILogTemplate template, IFileConstants constants)
        {
            {
                Debug.Assert(configuration != null, @"Must have an ILoggerConfiguration object.");
                Debug.Assert(template != null, @"Must have an ILogTemplate object");
                Debug.Assert(constants != null, @"Must have an IFileConstants object");
            }

            var result = new LoggingConfiguration();
            {
                // Define where the log is written to
                var fileTarget = new FileTarget();
                {
                    // Only write the message. The message should contain all the important 
                    // information anyway.
                    fileTarget.Layout = "${message}";

                    // Get the file path for the log file.
                    fileTarget.FileName = Path.Combine(configuration.TargetDirectory, template.Name + constants.LogExtension);

                    // Automatically flush each message to the file
                    fileTarget.AutoFlush = true;

                    // Always close the file so that we don't lose messages
                    // this does make logging slower though.
                    fileTarget.KeepFileOpen = false;

                    // Always append to the file
                    fileTarget.ReplaceFileContentsOnEachWrite = false;
                }

                var logTarget = new BufferingTargetWrapper(fileTarget);
                {
                    // Define how many messages should be buffered.
                    logTarget.BufferSize = configuration.NumberOfMessagesToBuffer;

                    // Define how quickly the message buffer should be flushed
                    logTarget.FlushTimeout = configuration.FlushAfter;
                }

                result.AddTarget(template.Name, logTarget);

                // define only one logging rule. We log everything (*) to the this rule starting
                // at log level TRACE and everything goes to the only target
                result.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Trace, logTarget));
            }

            return result;
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
        /// <param name="configuration">The configuration.</param>
        /// <param name="template">The template.</param>
        /// <param name="fileConstants">The constants that describe file and file path values.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="configuration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="template"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileConstants"/> is <see langword="null" />.
        /// </exception>
        public Logger(ILoggerConfiguration configuration, ILogTemplate template, IFileConstants fileConstants)
        {
            {
                Enforce.Argument(() => configuration);
                Enforce.Argument(() => template);
                Enforce.Argument(() => fileConstants);
            }

            m_Template = template;
            m_Factory = new LogFactory(BuildNLogConfiguration(configuration, template, fileConstants));
            {
                // Default setting is to log errors and fatals only.
                m_Factory.GlobalThreshold = NLog.LogLevel.Error;
            }

            m_Logger = m_Factory.GetLogger(template.Name);
        }

        #region Implementation of ILogger

        /// <summary>
        /// Gets the current <see cref="LevelToLog"/>.
        /// </summary>
        /// <value>The current level.</value>
        public LevelToLog Level
        {
            get
            {
                return TranslateFromNlogLevel(m_Logger);
            }
        }

        /// <summary>
        /// Changes the current log level to the specified level.
        /// </summary>
        /// <param name="newLevel">The new level.</param>
        public void ChangeLevel(LevelToLog newLevel)
        {
            var nlogLevel = TranslateToNlogLevel(newLevel);
            m_Factory.GlobalThreshold = nlogLevel;
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
            if (message == null)
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
            m_Logger.Log(level, m_Template.Translate(message));
        }

        #endregion
    }
}
