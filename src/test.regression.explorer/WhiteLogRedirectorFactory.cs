//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Castle.Core.Logging;
using Nuclei.Diagnostics;

namespace Test.Regression.Explorer
{
    /// <summary>
    /// Defines an <see cref="ILoggerFactory"/> that redirects the White logs to the 
    /// standard logger.
    /// </summary>
    internal sealed class WhiteLogRedirectorFactory : ILoggerFactory
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The default log level.
        /// </summary>
        private readonly LoggerLevel m_DefaultLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhiteLogRedirectorFactory"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="defaultLevel">The default log level.</param>
        public WhiteLogRedirectorFactory(SystemDiagnostics diagnostics, LoggerLevel defaultLevel)
        {
            m_Diagnostics = diagnostics;
            m_DefaultLevel = defaultLevel;
        }

        /// <summary>
        /// Creates a new logger, getting the logger name from the specified type.
        /// </summary>
        /// <param name="type">The type for which the logger is created.</param>
        /// <returns>The newly created logger.</returns>
        public ILogger Create(Type type)
        {
            return new WhiteLogRedirector(type.FullName, m_DefaultLevel, m_Diagnostics);
        }

        /// <summary>
        /// Creates a new logger.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <returns>The newly created logger.</returns>
        public ILogger Create(string name)
        {
            return new WhiteLogRedirector(name, m_DefaultLevel, m_Diagnostics);
        }

        /// <summary>
        /// Creates a new logger, getting the logger name from the specified type.
        /// </summary>
        /// <param name="type">The type for which the logger is created.</param>
        /// <param name="level">The default log level.</param>
        /// <returns>The newly created logger.</returns>
        public ILogger Create(Type type, LoggerLevel level)
        {
            return new WhiteLogRedirector(type.FullName, level, m_Diagnostics);
        }

        /// <summary>
        /// Creates a new logger.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="level">The default log level.</param>
        /// <returns>The newly created logger.</returns>
        public ILogger Create(string name, LoggerLevel level)
        {
            return new WhiteLogRedirector(name, level, m_Diagnostics);
        }
    }
}
