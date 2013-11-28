//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Test.Regression.Explorer
{
    /// <summary>
    /// Provides logging capabilities for the user interface tests.
    /// </summary>
    internal sealed class Log
    {
        /// <summary>
        /// The object that provides the logging capabilities.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public Log(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            m_Diagnostics.Log(
                LevelToLog.Info, 
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} INFO - {1}",
                    DateTimeOffset.Now,
                    message));
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            m_Diagnostics.Log(
                LevelToLog.Error, 
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} ERROR - {1}",
                    DateTimeOffset.Now,
                    message));
        }
    }
}
