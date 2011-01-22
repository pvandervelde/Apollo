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
using System.Windows.Forms;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        private const int s_ApplicationExitNormal = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        private const int s_ApplicationExitUnhandledException = 1;

        /// <summary>
        /// The event log name to which the application writes.
        /// </summary>
        private const string ApplicationEventLog = "Application";

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
        /// The table that maps an exception type to an event category.
        /// </summary>
        private static readonly Dictionary<Type, short> m_ExceptionTypeToEventCategoryMap =
            new Dictionary<Type, short> 
                { 
                    // Runtime errors
                    { typeof(OutOfMemoryException), 0 },
                    { typeof(StackOverflowException), 0 },
                    { typeof(AccessViolationException), 0 },
                    
                    // Standard errors
                    { typeof(AppDomainUnloadedException), 1 },
                    { typeof(ArgumentException), 1 },
                    { typeof(ArgumentNullException), 1 },
                    { typeof(ArgumentOutOfRangeException), 1 },
                    
                    // I/O
                    { typeof(IOException), 2 },
                    { typeof(DirectoryNotFoundException), 2 },
                    { typeof(DriveNotFoundException), 2 },
                    { typeof(EndOfStreamException), 2 },
                    { typeof(FileLoadException), 2 },
                    { typeof(FileNotFoundException), 2 },
                    { typeof(InternalBufferOverflowException), 2 },
                    { typeof(InvalidDataException), 2 },
                    { typeof(PathTooLongException), 2 },

                    // Exception, used in case nothing else fits
                    { typeof(Exception), short.MaxValue }
                };

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string s_DefaultErrorFileName = "dataset.error.log";

        /// <summary>
        /// The main entry point for the dataset application.
        /// </summary>
        /// <param name="args">
        /// The array containing the start-up arguments for the application.
        /// </param>
        /// <returns>A value indicating if the process exited normally (0) or abnormally (&gt; 0).</returns>
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                {
                    Debug.Assert(args != null, "The arguments array should not be null.");
                }

                var context = new ApplicationContext()
                    {
                        Tag = args
                    };

                Application.Run(context);
                return s_ApplicationExitNormal;
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
                    // Write the exception to the event log if we can.
                    // We won't create an event log if it isn't there. Too
                    // much chance of things going wrong.
                    if (EventLog.Exists(ApplicationEventLog))
                    {
                        var log = new EventLog(ApplicationEventLog);
                        log.WriteEntry(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Fatal exception occurred during application execution",
                                e),
                            EventLogEntryType.Error,
                            EventIdForException(e),
                            EventCategoryForException(e));
                    }
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

                // We'll also try to log the exception to an exception file in a semi-sane
                // location (users application directory)
                try
                {
                    WriteExceptionToFile(e);
                }
                catch (IOException)
                {
                    // We're really screwed now. Nothing we can do anymore ... just exit
                }

                return s_ApplicationExitUnhandledException;
            }
        }

        private static int EventIdForException(Exception exception)
        {
            // Go over the exception and see if we have defined it's type, if not
            // then go for the parent exception etc. all the way up to the base 
            // Exception type.
            throw new NotImplementedException();
        }

        private static short EventCategoryForException(Exception exception)
        {
            // Go over the exception and see if we have defined it's type, if not
            // then go for the parent exception etc. all the way up to the base 
            // Exception type.
            throw new NotImplementedException();
        }

        private static void WriteExceptionToFile(Exception exception)
        {
            // Get the local file path. There is a function for this (AssemblyExtensions.LocalFilePath)
            // but we don't want to use it because we want to prevent any of our own code from
            // loading. If an exception happens that is not handled then we might be having
            // loader issues. These are probably due to us trying to load some of our code or
            // one of it's dependencies. Given that this is causing a problem it seems wise to not
            // try to use our code to find an assembly file path ...
            //
            // Note that some (nearly) identical code lives in Apollo.Core.Utils.ApplicationConstant.
            // However we can't load any additional code so we have had to duplicate it here (sigh).
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

            // Write the exception text to the file.
            var filePath = Path.Combine(productDirectory, s_DefaultErrorFileName);
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
