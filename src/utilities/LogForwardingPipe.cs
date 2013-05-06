//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Apollo.Utilities
{
    /// <summary>
    /// Provides methods to forward log messages across an <c>AppDomain</c> boundary.
    /// </summary>
    public sealed class LogForwardingPipe : MarshalByRefObject, ILogMessagesFromRemoteAppDomains
    {
        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogForwardingPipe"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public LogForwardingPipe(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Logs the given message with the given severity.
        /// </summary>
        /// <param name="severity">The importance of the log message.</param>
        /// <param name="message">The message.</param>
        public void Log(LevelToLog severity, string message)
        {
            m_Diagnostics.Log(
                severity,
                UtilitiesConstants.LogPrefix,
                message);
        }
    }
}
