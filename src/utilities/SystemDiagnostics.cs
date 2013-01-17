//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Utilities.Diagnostics.Profiling;

namespace Apollo.Utilities
{
    /// <summary>
    /// Provides methods that help with diagnosing possible issues with the framework.
    /// </summary>
    public sealed class SystemDiagnostics
    {
        /// <summary>
        /// The profiler that is used to time the different actions in the application.
        /// </summary>
        private readonly Profiler m_Profiler;

        /// <summary>
        /// The action that logs the given string to the underlying loggers.
        /// </summary>
        private readonly Action<LogSeverityProxy, string> m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemDiagnostics"/> class.
        /// </summary>
        /// <param name="logger">The action that logs the given string to the underlying loggers.</param>
        /// <param name="profiler">The object that provides interval measuring methods. May be <see langword="null" />.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public SystemDiagnostics(Action<LogSeverityProxy, string> logger, Profiler profiler)
        {
            {
                Lokad.Enforce.Argument(() => logger);
            }

            m_Logger = logger;
            m_Profiler = profiler;
        }

        /// <summary>
        /// Passes the given message to the system loggers.
        /// </summary>
        /// <param name="severity">The severity for the message.</param>
        /// <param name="message">The message.</param>
        public void Log(LogSeverityProxy severity, string message)
        {
            m_Logger(severity, message);
        }

        /// <summary>
        /// Gets the profiler that can be used to gather timing intervals for any specific action
        /// that is executed in the framework.
        /// </summary>
        public Profiler Profiler
        {
            get
            {
                return m_Profiler;
            }
        }
    }
}
