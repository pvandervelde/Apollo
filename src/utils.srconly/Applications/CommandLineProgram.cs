//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;

namespace Apollo.Utils.Applications
{
    /// <summary>
    /// Defines the default implementation for a commandline application.
    /// </summary>
    internal static class CommandLineProgram
    {
        /// <summary>
        /// The event log name to which the application writes.
        /// </summary>
        private const string ApplicationEventLog = "Application";

        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        public const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        public const int UnhandledExceptionApplicationExitcode = 1;

        /// <summary>
        /// The main entry point for the dataset application.
        /// </summary>
        /// <param name="applicationMethod">The function that runs the application logic.</param>
        /// <param name="eventLogSource">The name of the event log source.</param>
        /// <param name="errorLogFileName">The name of the file that contains the error log.</param>
        /// <returns>A value indicating if the process exited normally (0) or abnormally (&gt; 0).</returns>
        public static int EntryPoint(Func<int> applicationMethod, string eventLogSource, string errorLogFileName)
        {
            try
            {
                {
                    Debug.Assert(applicationMethod != null, "The application method should not be null.");
                }

                return applicationMethod();
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
                //
                // We'll try to log to the event log.
                try
                {
                    WriteExceptionToEventLog(eventLogSource, e);
                }
                catch (ArgumentException)
                {
                    // One of the arguments was incorrect. Oh well
                }
                catch (InvalidOperationException)
                {
                    // Could not open the registry key for the event log
                }
                catch (Win32Exception)
                {
                    // The operating system reported an error without 
                    // an error code.
                }

                try
                {
                    // We'll also try to log the exception to an exception file in a semi-sane
                    // location (users application directory)
                    WriteExceptionToFile(errorLogFileName, e);
                }
                catch (IOException)
                {
                    // We're really screwed now. Nothing we can do anymore ... just exit
                }

                return UnhandledExceptionApplicationExitcode;
            }
        }

        /// <summary>
        /// Writes the given exception to the event log.
        /// </summary>
        /// <param name="eventLogSource">The name of the event log source.</param>
        /// <param name="exception">The exception.</param>
        public static void WriteExceptionToEventLog(string eventLogSource, Exception exception)
        {
            WriteToEventLog(
                eventLogSource,
                EventLogEntryType.Error,
                ExceptionTypeToEventIdMap.EventIdForException(exception),
                EventType.Exception,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Fatal exception occurred during application execution",
                    exception));
        }

        /// <summary>
        /// Writes the given text to the Application event log and records the event ID and category ID in the
        /// event log entry.
        /// </summary>
        /// <param name="eventLogSource">The name of the event log source.</param>
        /// <param name="entryType">The kind of event log entry that should be generated.</param>
        /// <param name="eventId">The application specific identifier for the event.</param>
        /// <param name="category">The category type for the event.</param>
        /// <param name="text">The text which should be recorded in the event entry.</param>
        private static void WriteToEventLog(string eventLogSource, EventLogEntryType entryType, int eventId, EventType category, string text)
        {
            // If the source does not exist we're stuffed. Normal users can't create an event log source
            // and they can't even check whether the source does exist so we'll just pretend that the 
            // installer has done it's job by creating the event source for us.
            if (EventLog.Exists(ApplicationEventLog))
            {
                var log = new EventLog(ApplicationEventLog);
                log.Source = eventLogSource;
                log.WriteEntry(text, entryType, eventId, EventTypeToEventCategoryMap.EventCategory(category));
            }
        }

        private static void WriteExceptionToFile(string fileName, Exception exception)
        {
            // Get the local file path. There is a function for this (AssemblyExtensions.LocalFilePath)
            // but we don't want to use it because we want to prevent any of our own code from
            // loading. If an exception happens that is not handled then we might be having
            // loader issues. These are probably due to us trying to load some of our code or
            // one of it's dependencies. Given that this is causing a problem it seems wise to not
            // try to use our (external) code to find an assembly file path ...
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (localAppDataPath == null)
            {
                return;
            }

            var companyAttribute = GetAttributeFromAssembly<AssemblyCompanyAttribute>();
            if ((companyAttribute == null) || string.IsNullOrEmpty(companyAttribute.Company))
            {
                return;
            }

            var productAttribute = GetAttributeFromAssembly<AssemblyProductAttribute>();
            if ((productAttribute == null) || string.IsNullOrEmpty(productAttribute.Product))
            {
                return;
            }

            var companyDirectory = Path.Combine(localAppDataPath, companyAttribute.Company);
            var productDirectory = Path.Combine(companyDirectory, productAttribute.Product);
            if (!Directory.Exists(productDirectory))
            {
                Directory.CreateDirectory(productDirectory);
            }

            var filePath = Path.Combine(productDirectory, fileName);
            using (var writer = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate), Encoding.ASCII))
            {
                writer.WriteLine("Unhandled exception occurred. Exception information to follow.");
                writer.WriteLine(exception);
            }
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
