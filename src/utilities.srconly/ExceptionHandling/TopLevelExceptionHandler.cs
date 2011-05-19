//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using Apollo.Utilities.Logging;

namespace Apollo.Utilities.ExceptionHandling
{
    /// <summary>
    /// Defines a top level exception handler which stops all exceptions from progagating out of the application, thus
    /// providing a chance for logging and semi-graceful termination of the application.
    /// </summary>
    internal static class TopLevelExceptionHandler
    {
        /// <summary>
        /// The event log name to which the application writes.
        /// </summary>
        private const string ApplicationEventLog = "Application";

        /// <summary>
        /// Runs an action inside a high level try .. catch construct that will not let any errors escape
        /// but will log errors to a file and the eventlog.
        /// </summary>
        /// <param name="actionToExecute">The action that should be executed.</param>
        /// <param name="eventLogSource">The name of the event log source.</param>
        /// <param name="errorLogFileName">The name of the file that contains the error log.</param>
        /// <returns>
        /// A value indicating whether the action executed successfully or not.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching an Exception object here because this is the top-level exception handler.")]
        public static GuardResult RunGuarded(Action actionToExecute, string eventLogSource, string errorLogFileName)
        {
            // Pre allocate these so that we actually have them.
            var loggers = new ILogger[] 
                { 
                    LoggerBuilder.ForFile(Path.Combine(CreateProductSpecificApplicationDataDirectory(), errorLogFileName), new DebugLogTemplate(() => DateTimeOffset.Now)),
                    LoggerBuilder.ForEventLog(eventLogSource, new DebugLogTemplate(() => DateTimeOffset.Now)),
                };

            try
            {
                {
                    Debug.Assert(actionToExecute != null, "The application method should not be null.");
                }

                actionToExecute();
                return GuardResult.Success;
            }
            catch (Exception e)
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
                var text = string.Format(
                    CultureInfo.InvariantCulture,
                    "Fatal exception occurred during application execution",
                    e);

                var msg = new LogMessage(
                    LevelToLog.Fatal,
                    text,
                    new Dictionary<string, object>() 
                        { 
                            { AdditionalLogMessageProperties.EventCategory, EventTypeToEventCategoryMap.EventCategory(EventType.Exception) },
                            { AdditionalLogMessageProperties.EventId, ExceptionTypeToEventIdMap.EventIdForException(e) },
                        });

                foreach (var logger in loggers)
                {
                    try
                    {
                        logger.Log(msg);
                    }
                    catch (Exception)
                    {
                        // Stuffed. Just give up.
                    }
                }

                return GuardResult.Failure;
            }
        }

        private static string CreateProductSpecificApplicationDataDirectory()
        {
            // Get the local file path. There is a function for this (ApplicationConstants)
            // but we don't want to use it because we want to prevent any of our own code from
            // loading. If an exception happens that is not handled then we might be having
            // loader issues. These are probably due to us trying to load some of our code or
            // one of it's dependencies. Given that this is causing a problem it seems wise to not
            // try to use our (external) code to find an assembly file path ...
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (localAppDataPath == null)
            {
                throw new DirectoryNotFoundException();
            }

            var companyAttribute = GetAttributeFromAssembly<AssemblyCompanyAttribute>();
            Debug.Assert((companyAttribute != null) && !string.IsNullOrEmpty(companyAttribute.Company), "There should be a company name.");

            var productAttribute = GetAttributeFromAssembly<AssemblyProductAttribute>();
            Debug.Assert((productAttribute != null) && !string.IsNullOrEmpty(productAttribute.Product), "There should be a product name.");

            var companyDirectory = Path.Combine(localAppDataPath, companyAttribute.Company);
            var productDirectory = Path.Combine(companyDirectory, productAttribute.Product);
            if (!Directory.Exists(productDirectory))
            {
                Directory.CreateDirectory(productDirectory);
            }

            return productDirectory;
        }

        /// <summary>
        /// Gets the attribute from the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute that should be gotten from the assembly.</typeparam>
        /// <returns>
        /// The requested attribute.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "The type parameter indicates which attribute we're looking for.")]
        private static T GetAttributeFromAssembly<T>() where T : Attribute
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            Debug.Assert(attributes.Length == 1, "There should only be one attribute.");

            var requestedAttribute = attributes[0] as T;
            Debug.Assert(requestedAttribute != null, "Found an incorrect attribute type.");

            return requestedAttribute;
        }
    }
}
