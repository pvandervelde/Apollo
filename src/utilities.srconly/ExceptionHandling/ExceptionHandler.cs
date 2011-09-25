//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using Apollo.Utilities.Logging;
using Autofac;
using NSarrac.Framework;

namespace Apollo.Utilities.ExceptionHandling
{
    /// <summary>
    /// A fake exception handler. This will later be removed and replaced
    /// by a proper one.
    /// </summary>
    /// <design>
    /// This class must be public because we use it in the AppDomainBuilder.
    /// </design>
    [Serializable]
    public sealed class ExceptionHandler : IExceptionHandler, IDisposable
    {
        /// <summary>
        /// The event log name to which the application writes.
        /// </summary>
        private const string ApplicationEventLog = "Application";

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "apollo.error.log";

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We don't really care if we get a processor or not but we really don't want a crash here.")]
        private static IExceptionProcessor CreateReportBuildingProcessor()
        {
            try
            {
                var builder = new ContainerBuilder();
                {
                    RSAParameters rsaParameters = SrcOnlyExceptionHandlingUtillities.ReportingPublicKey();
                    builder.RegisterModule(new FeedbackReportingModule(() => rsaParameters));
                }

                var container = builder.Build();
                return new ReportingExceptionProcessor(() => container.Resolve<IBuildReports>());
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We don't really care if we get a processor or not but we really don't want a crash here.")]
        private static IExceptionProcessor CreateFileLoggingProcessor(string logFile)
        {
            try
            {
                var logDir = ReportingUtilities.ProductSpecificApplicationDataDirectory();
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                return new LogBasedExceptionProcessor(
                    LoggerBuilder.ForFile(
                        Path.Combine(logDir, logFile),
                        new DebugLogTemplate(() => DateTimeOffset.Now)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We don't really care if we get a processor or not but we really don't want a crash here.")]
        private static IExceptionProcessor CreateEventLoggingProcessor(string eventLog)
        {
            try
            {
                return new LogBasedExceptionProcessor(
                    LoggerBuilder.ForEventLog(
                        eventLog,
                        new DebugLogTemplate(() => DateTimeOffset.Now)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// The collection of loggers that must be notified if an exception happens.
        /// </summary>
        private readonly IExceptionProcessor[] m_Loggers;

        /// <summary>
        /// Indicates if the object has been disposed or not.
        /// </summary>
        private volatile bool m_WasDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        /// <param name="eventLogSource">The name of the event log source.</param>
        /// <param name="errorLogFileName">The name of the file that contains the error log.</param>
        public ExceptionHandler(string eventLogSource, string errorLogFileName)
        {
            var eventLog = !string.IsNullOrWhiteSpace(eventLogSource)
                ? eventLogSource
                : ApplicationEventLog;

            var logFile = !string.IsNullOrWhiteSpace(errorLogFileName)
                ? errorLogFileName
                : DefaultErrorFileName;

            // Pre allocate these so that we actually have them.
            // Note that the loading of a processor may fail for many reasons
            // so we guard against that and then we just ignore the result.
            // This may lead to a case where we have no loggers ...
            var loggers = new List<IExceptionProcessor>();
            var toFile = CreateFileLoggingProcessor(logFile);
            if (toFile != null)
            {
                loggers.Add(toFile);
            }

            var toEventLog = CreateEventLoggingProcessor(eventLog);
            if (toEventLog != null)
            {
                loggers.Add(toEventLog);
            }

            var toReport = CreateReportBuildingProcessor();
            if (toReport != null)
            {
                loggers.Add(toReport);
            }

            m_Loggers = loggers.ToArray();
        }

        /// <summary>
        /// Used when an unhandled exception occurs in an <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="isApplicationTerminating">Indicates if the application is about to shut down or not.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We're doing exception handling here, we don't really want anything to escape.")]
        public void OnException(Exception exception, bool isApplicationTerminating)
        {
            if (m_WasDisposed)
            {
                return;
            }

            // Something has gone really wrong here. We need to be very careful
            // when we try to deal with this exception because:
            // - We might be here due to assembly loading issues, so we can't load
            //   any code which is not in the current class or in one of the system
            //   assemblies (that is we assume any code in the GAC is available ...
            //   which obviously may be incorrect).
            // - We might be here because the CLR failed hard (e.g. OutOfMemoryException
            //   and friends). In this case we're toast. We'll try our normal approach
            //   but that will probably fail ...
            //
            // We don't want to throw an exception if we're handling unhandled exceptions ...
            foreach (var logger in m_Loggers)
            {
                try
                {
                    logger.Process(exception);
                }
                catch (Exception)
                {
                    // Stuffed. Just give up.
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_WasDisposed)
            {
                return;
            }

            foreach (var logger in m_Loggers)
            {
                try
                {
                    logger.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
            }

            m_WasDisposed = true;
        }
    }
}
