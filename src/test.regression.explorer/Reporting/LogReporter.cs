//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Test.Regression.Explorer.Reporting
{
    /// <summary>
    /// Defines an <see cref="IReporter"/> class that writes to the log file.
    /// </summary>
    internal sealed class LogReporter : IReporter
    {
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReporter"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public LogReporter(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Initializes the report.
        /// </summary>
        public void Initialize()
        {
            // Do nothing ...
        }

        /// <summary>
        /// Adds a new information report entry.
        /// </summary>
        /// <param name="message">The information message for the report entry.</param>
        public void AddInformationalMessage(string message)
        {
            m_Diagnostics.Log(
                LevelToLog.Info, 
                message);
        }

        /// <summary>
        /// Adds a new error message to the report.
        /// </summary>
        /// <param name="message">The error message for the report entry.</param>
        public void AddErrorMessage(string message)
        {
            m_Diagnostics.Log(
                LevelToLog.Error, 
                message);
        }

        /// <summary>
        /// Completes the report.
        /// </summary>
        public void Complete()
        {
            // Do nothing ...
        }
    }
}
