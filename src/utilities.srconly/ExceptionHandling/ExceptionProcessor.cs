//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Apollo.Utilities.Logging;

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
    public sealed class ExceptionProcessor : IExceptionHandler
    {
        /// <summary>
        /// The event log name to which the application writes.
        /// </summary>
        private const string ApplicationEventLog = "Application";

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "apollo.error.log";

        private static string CreateProductSpecificApplicationDataDirectory()
        {
            // Get the local file path. There is a function for this (ApplicationConstants)
            // but we don't want to use it because we want to prevent any of our own code from
            // loading. If an exception happens that is not handled then we might be having
            // loader issues. These are probably due to us trying to load some of our code or
            // one of it's dependencies. Given that this is causing a problem it seems wise to not
            // try to use our (external) code to find an assembly file path ...
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (localAppDataPath == null)
            {
                throw new DirectoryNotFoundException();
            }

            var companyAttribute = GetAttributeFromAssembly<AssemblyCompanyAttribute>();
            Debug.Assert((companyAttribute != null) && !string.IsNullOrEmpty(companyAttribute.Company), "There should be a company name.");

            var productAttribute = GetAttributeFromAssembly<AssemblyProductAttribute>();
            Debug.Assert((productAttribute != null) && !string.IsNullOrEmpty(productAttribute.Product), "There should be a product name.");

            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var companyDirectory = Path.Combine(localAppDataPath, companyAttribute.Company);
            var productDirectory = Path.Combine(companyDirectory, productAttribute.Product);
            var versionDirectory = Path.Combine(productDirectory, new Version(assemblyVersion.Major, assemblyVersion.Minor).ToString(2));
            if (!Directory.Exists(versionDirectory))
            {
                Directory.CreateDirectory(versionDirectory);
            }

            return versionDirectory;
        }

        /// <summary>
        /// Gets the attribute from the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute that should be gotten from the assembly.</typeparam>
        /// <returns>
        /// The requested attribute.
        /// </returns>
        private static T GetAttributeFromAssembly<T>() where T : Attribute
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            Debug.Assert(attributes.Length == 1, "There should only be one attribute.");

            var requestedAttribute = attributes[0] as T;
            Debug.Assert(requestedAttribute != null, "Found an incorrect attribute type.");

            return requestedAttribute;
        }

        /// <summary>
        /// The collection of loggers that must be notified if an exception happens.
        /// </summary>
        private readonly ILogger[] m_Loggers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionProcessor"/> class.
        /// </summary>
        /// <param name="eventLogSource">The name of the event log source.</param>
        /// <param name="errorLogFileName">The name of the file that contains the error log.</param>
        public ExceptionProcessor(string eventLogSource, string errorLogFileName)
        {
            var eventLog = !string.IsNullOrWhiteSpace(eventLogSource)
                ? eventLogSource
                : ApplicationEventLog;

            var logFile = !string.IsNullOrWhiteSpace(errorLogFileName)
                ? errorLogFileName
                : DefaultErrorFileName;
            
            // Pre allocate these so that we actually have them.
            m_Loggers = new ILogger[] 
                { 
                    LoggerBuilder.ForFile(
                        Path.Combine(CreateProductSpecificApplicationDataDirectory(), logFile), 
                        new DebugLogTemplate(() => DateTimeOffset.Now)),
                    LoggerBuilder.ForEventLog(
                        eventLog, 
                        new DebugLogTemplate(() => DateTimeOffset.Now)),
                };
        }

        /// <summary>
        /// Used when an unhandled exception occurs in an <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="isApplicationTerminating">Indicates if the application is about to shut down or not.</param>
        public void OnException(Exception exception, bool isApplicationTerminating)
        {
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
            string text = string.Empty;
            if (exception != null)
            {
                text = string.Format(
                    CultureInfo.InvariantCulture,
                    "Fatal exception occurred during application execution. Exception message was: {0}",
                    exception);
            }
            else 
            {
                text = string.Format(
                    CultureInfo.InvariantCulture,
                    "Fatal exception occurred during application execution. No exception or stack trace provided.");
            }

            var msg = new LogMessage(
                LevelToLog.Fatal,
                text,
                new Dictionary<string, object>() 
                        { 
                            { AdditionalLogMessageProperties.EventCategory, EventTypeToEventCategoryMap.EventCategory(EventType.Exception) },
                            { AdditionalLogMessageProperties.EventId, ExceptionTypeToEventIdMap.EventIdForException(exception) },
                        });

            foreach (var logger in m_Loggers)
            {
                try
                {
                    logger.Log(msg);
                    logger.Close();
                }
                catch (Exception)
                {
                    // Stuffed. Just give up.
                }
            }
        }
    }
}
