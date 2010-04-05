//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines the interface for objects that log information.
    /// </summary>
    internal interface ILogger
    {
        /// <summary>
        /// Gets the current <see cref="LogLevel"/>.
        /// </summary>
        /// <value>The current level.</value>
        LogLevel Level 
        {
            get; 
        }

        /// <summary>
        /// Changes the current log level to the specified level.
        /// </summary>
        /// <param name="newLevel">The new level.</param>
        void ChangeLevel(LogLevel newLevel);

        /// <summary>
        /// Indicates if a message will be written to the log file based on the
        /// current log level and the level of the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// <see langword="true" /> if the message will be logged; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ShouldLog(ILogMessage message);

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Log(ILogMessage message);
    }
}
