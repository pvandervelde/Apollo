﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Apollo.Utilities.Logging
{
    /// <summary>
    /// Defines a factory that builds <see cref="ILogger"/> objects.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class LoggerBuilder
    {
        static LoggerBuilder()
        {
            TurnOnLogDebugging();
        }

        /// <summary>
        /// Turns on the NLog internal logging for debug purposes.
        /// </summary>
        private static void TurnOnLogDebugging()
        {
            // Set the internal logging for NLog
            // InternalLogger.LogFile = @"d:\temp\nloginternal.log";
            // InternalLogger.LogLevel = LogLevel.Trace;
        }

        private static Target BuildFileTarget(string filePath)
        {
            var fileTarget = new FileTarget()
                {
                    // Only write the message. The message should contain all the important 
                    // information anyway.
                    Layout = "${message}",

                    // Get the file path for the log file.
                    FileName = filePath,

                    // Create the directories if needed
                    CreateDirs = true,

                    // Automatically flush each message to the file
                    AutoFlush = true,

                    // Always close the file so that we don't lose messages
                    // this does make logging slower though.
                    KeepFileOpen = false,

                    // Always append to the file
                    ReplaceFileContentsOnEachWrite = false,

                    // Do not concurrently write to the logger (at least for now)
                    ConcurrentWrites = false,
                };

            return fileTarget;
        }

        private static Target BuildEventLogTarget(string eventLogSource)
        {
            var eventLogTarget = new EventLogTarget()
            {
                // Only write the message. The message should contain all the important 
                // information anyway.
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",

                // The source which has been registered to write to the event log.
                Source = eventLogSource,

                // Always write to the application log for now.
                Log = "Application",

                // Define how we move the event id to the logger.
                EventId = string.Format(CultureInfo.InvariantCulture, "${{event-context:item={0}}}", AdditionalLogMessageProperties.EventId),

                // Define how we move the event category to the logger.
                Category = string.Format(CultureInfo.InvariantCulture, "${{event-context:item={0}}}", AdditionalLogMessageProperties.EventCategory),
            };

            return eventLogTarget;
        }

        private static Target BufferTarget(Target targetToBuffer)
        {
            // var logTarget = new BufferingTargetWrapper(targetToBuffer);
            // {
            //     // Define how many messages should be buffered.
            //     logTarget.BufferSize = 10;
            //
            //     // Define how quickly the message buffer should be flushed
            //     logTarget.FlushTimeout = 500;
            // }
            //
            // return logTarget;
            return targetToBuffer;
        }

        private static LogFactory BuildLogFactory(string name, NLog.LogLevel minimumLevel, Target target)
        {
            var config = new LoggingConfiguration();
            {
                config.AddTarget(name, target);

                // define only one logging rule. We log everything (*) to the this rule starting
                // at log level TRACE and everything goes to the only target
                config.LoggingRules.Add(new LoggingRule("*", minimumLevel, target));
            }
            
            var result = new LogFactory(config);
            {
                result.GlobalThreshold = NLog.LogLevel.Trace;
                result.ThrowExceptions = true;
            }

            result.EnableLogging();
            return result;
        }

        /// <summary>
        /// Builds an <see cref="ILogger"/> object that logs information to a given file.
        /// </summary>
        /// <param name="filePath">The file path to which the information gets logged.</param>
        /// <param name="template">The log template that handles the translation from the <see cref="ILogMessage"/> to the log text.</param>
        /// <returns>
        /// The newly created logger that logs information to a given file.
        /// </returns>
        public static ILogger ForFile(string filePath, ILogTemplate template)
        {
            var factory = BuildLogFactory(template.Name, NLog.LogLevel.Trace, BufferTarget(BuildFileTarget(filePath)));
            return new Logger(factory, template);
        }

        /// <summary>
        /// Builds an <see cref="ILogger"/> object that logs information to the eventlog.
        /// </summary>
        /// <param name="eventLogSource">The eventlog source name under which the current application is registered.</param>
        /// <param name="template">The log template that handles the translation from the <see cref="ILogMessage"/> to the log text.</param>
        /// <returns>
        /// The newly created logger that logs infomration to the event log.
        /// </returns>
        public static ILogger ForEventLog(string eventLogSource, ILogTemplate template)
        {
            var factory = BuildLogFactory(template.Name, NLog.LogLevel.Warn, BuildEventLogTarget(eventLogSource));
            return new Logger(factory, template);
        }
    }
}