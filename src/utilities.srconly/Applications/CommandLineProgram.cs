//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.ExceptionHandling;

namespace Apollo.Utilities.Applications
{
    /// <summary>
    /// Defines the default implementation for a commandline application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class CommandLineProgram
    {
        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        public const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        public const int UnhandledExceptionApplicationExitCode = 1;

        /// <summary>
        /// The main entry point for the dataset application.
        /// </summary>
        /// <param name="applicationMethod">The function that runs the application logic.</param>
        /// <param name="eventLogSource">The name of the event log source.</param>
        /// <param name="errorLogFileName">The name of the file that contains the error log.</param>
        /// <returns>A value indicating if the process exited normally (0) or abnormally (&gt; 0).</returns>
        public static int EntryPoint(Func<int> applicationMethod, string eventLogSource, string errorLogFileName)
        {
            int functionReturnResult = -1;
            Action applicationAction = () => functionReturnResult = applicationMethod();
            
            var result = TopLevelExceptionHandler.RunGuarded(applicationAction, eventLogSource, errorLogFileName);
            return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
        }
    }
}
