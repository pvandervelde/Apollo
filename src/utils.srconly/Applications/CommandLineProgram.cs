//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
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
        /// The table that maps an exception type to an event ID.
        /// </summary>
        private static readonly Dictionary<Type, int> m_ExceptionTypeToEventIdMap =
            new Dictionary<Type, int> 
                { 
                    // Runtime errors
                    { typeof(OutOfMemoryException), 0 },
                    { typeof(StackOverflowException), 1 },
                    { typeof(AccessViolationException), 2 },
                    
                    // Standard errors
                    { typeof(AppDomainUnloadedException), 50 },
                    { typeof(ArgumentException), 51 },
                    { typeof(ArgumentNullException), 52 },
                    { typeof(ArgumentOutOfRangeException), 53 },
                    
                    // I/O
                    { typeof(IOException), 100 },
                    { typeof(DirectoryNotFoundException), 101 },
                    { typeof(DriveNotFoundException), 102 },
                    { typeof(EndOfStreamException), 103 },
                    { typeof(FileLoadException), 104 },
                    { typeof(FileNotFoundException), 105 },
                    { typeof(InternalBufferOverflowException), 106 },
                    { typeof(InvalidDataException), 107 },
                    { typeof(PathTooLongException), 108 },

                    // Exception, used in case nothing else fits
                    { typeof(Exception), int.MaxValue }
                };

        /// <summary>
        /// The table that maps an event type to an event category.
        /// </summary>
        private static readonly Dictionary<EventType, short> m_EventTypeToEventCategoryMap =
            new Dictionary<EventType, short> 
                { 
                    { EventType.Exception, 0 }
                };

        /// <summary>
        /// The main entry point for the dataset application.
        /// </summary>
        /// <param name="applicationMethod">The function that runs the application logic.</param>
        /// <param name="errorLogFileName">The name of the file that contains the error log.</param>
        /// <returns>A value indicating if the process exited normally (0) or abnormally (&gt; 0).</returns>
        public static int EntryPoint(Func<int> applicationMethod, string errorLogFileName)
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
                    WriteExceptionToEventLog(e);
                }
                catch (ArgumentException)
                {
                    // Failed to write to the event log. Nothing we can do about it
                    // Simply bail.
                }
                catch (InvalidOperationException)
                {
                    // Failed to write to the event log. Nothing we can do about it
                    // Simply bail.
                }
                catch (Win32Exception)
                {
                    // Failed to write to the event log. Nothing we can do about it
                    // Simply bail.
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
        /// <param name="exception">The exception.</param>
        public static void WriteExceptionToEventLog(Exception exception)
        {
            WriteToEventLog(
                EventLogEntryType.Error, 
                EventIdForException(exception),
                EventType.Exception,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Fatal exception occurred during application execution",
                    exception));
        }

        private static int EventIdForException(Exception exception)
        {
            var exceptionType = exception.GetType();
            while (!m_ExceptionTypeToEventIdMap.ContainsKey(exceptionType))
            {
                exceptionType = exceptionType.BaseType;
            }

            // If we get here then:
            // a) we found our exception type
            // b) we fell all the way through and found Exception as the type
            return m_ExceptionTypeToEventIdMap[exceptionType];
        }

        /// <summary>
        /// Writes the given text to the Application event log and records the event ID and category ID in the
        /// event log entry.
        /// </summary>
        /// <param name="entryType">The kind of event log entry that should be generated.</param>
        /// <param name="eventId">The application specific identifier for the event.</param>
        /// <param name="category">The category type for the event.</param>
        /// <param name="text">The text which should be recorded in the event entry.</param>
        public static void WriteToEventLog(EventLogEntryType entryType, int eventId, EventType category, string text)
        {
            if (EventLog.Exists(ApplicationEventLog))
            {
                var log = new EventLog(ApplicationEventLog);
                log.WriteEntry(text, entryType, eventId, EventCategory(category));
            }
        }

        /// <summary>
        /// Returns the event category for the given <see cref="EventType"/> value.
        /// </summary>
        /// <param name="type">The type of the event.</param>
        /// <returns>The requested category ID.</returns>
        private static short EventCategory(EventType type)
        {
            return m_EventTypeToEventCategoryMap[type];
        }

        private static void WriteExceptionToFile(string fileName, Exception exception)
        {
            // Get the local file path. There is a function for this (AssemblyExtensions.LocalFilePath)
            // but we don't want to use it because we want to prevent any of our own code from
            // loading. If an exception happens that is not handled then we might be having
            // loader issues. These are probably due to us trying to load some of our code or
            // one of it's dependencies. Given that this is causing a problem it seems wise to not
            // try to use our code to find an assembly file path ...
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
