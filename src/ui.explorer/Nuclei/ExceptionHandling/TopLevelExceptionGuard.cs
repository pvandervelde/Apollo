//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Explorer.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Defines a top level exception handler which stops all exceptions from propagating out of the application, thus
    /// providing a chance for logging and semi-graceful termination of the application.
    /// </summary>
    internal static class TopLevelExceptionGuard
    {
        /// <summary>
        /// Runs an action inside a high level try .. catch construct that will not let any errors escape
        /// but will log errors to a file and the event log.
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
            {
                Debug.Assert(actionToExecute != null, "The application method should not be null.");
            }

            using (var processor = new ExceptionHandler(eventLogSource, errorLogFileName))
            {
                GuardResult result = GuardResult.None;
                ExceptionFilter.ExecuteWithFilter(
                    () =>
                    {
                        actionToExecute();
                        result = GuardResult.Success;
                    },
                    e =>
                    {
                        processor.OnException(e, false);
                        result = GuardResult.Failure;
                    });

                return result;
            }
        }
    }
}
