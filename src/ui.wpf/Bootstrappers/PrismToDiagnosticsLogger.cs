//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.Practices.Prism.Logging;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Apollo.UI.Wpf.Bootstrappers
{
    /// <summary>
    /// Defines an <see cref="ILoggerFacade"/> that redirects log messages to the <see cref="SystemDiagnostics"/> object for the
    /// application.
    /// </summary>
    public sealed class PrismToDiagnosticsLogger : ILoggerFacade
    {
        private static LevelToLog ConvertFromPrismCategoryToLevelToLog(Category category)
        {
            switch (category)
            {
                case Category.Debug:
                    return LevelToLog.Trace;
                case Category.Exception:
                    return LevelToLog.Error;
                case Category.Info:
                    return LevelToLog.Info;
                case Category.Warn:
                    return LevelToLog.Warn;
                default:
                    return LevelToLog.None;
            }
        }

        /// <summary>
        /// The object that provides all the diagnostics methods for the object.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrismToDiagnosticsLogger"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public PrismToDiagnosticsLogger(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Write a new log entry with the specified category and priority.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category of the entry.</param>
        /// <param name="priority">The priority of the entry.</param>
        public void Log(string message, Category category, Priority priority)
        {
            m_Diagnostics.Log(
                ConvertFromPrismCategoryToLevelToLog(category),
                UIConstants.LogPrefix,
                message);
        }
    }
}
