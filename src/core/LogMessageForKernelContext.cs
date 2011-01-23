//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Logging;
using Apollo.Utils.Commands;
using Lokad;
using Lokad.Rules;

namespace Apollo.Core
{
    /// <summary>
    /// Defines and <see cref="ICommandContext"/> for the <see cref="LogMessageForKernelCommand"/>.
    /// </summary>
    internal sealed class LogMessageForKernelContext : ICommandContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageForKernelContext"/> class.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="message"/> is an empty string.
        /// </exception>
        public LogMessageForKernelContext(LevelToLog level, string message)
        {
            {
                Enforce.Argument(() => message);
                Enforce.Argument(() => message, StringIs.NotEmpty);
            }

            Level = level;
            Message = message;
        }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public LevelToLog Level
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get;
            private set;
        }
    }
}
