//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Properties;
using Lokad;
using Lokad.Rules;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Stores information about a logger configuration.
    /// </summary>
    internal sealed class LoggerConfiguration : ILoggerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerConfiguration"/> class.
        /// </summary>
        /// <param name="targetDirectory">The directory where the log files get written to.</param>
        /// <param name="numberOfMessagesToBuffer">The number of messages to buffer.</param>
        /// <param name="flushAfter">The number of miliseconds after which the log buffers get flushed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="targetDirectory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="targetDirectory"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="numberOfMessagesToBuffer"/> is smaller than 1.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="flushAfter"/> is smaller than 0.
        /// </exception>
        public LoggerConfiguration(string targetDirectory, int numberOfMessagesToBuffer, int flushAfter)
        {
            {
                Enforce.Argument(() => targetDirectory);
                Enforce.Argument(() => targetDirectory, StringIs.NotEmpty);

                Enforce.With<ArgumentOutOfRangeException>(
                    numberOfMessagesToBuffer > 0,
                    Resources_NonTranslatable.Exception_Messages_MustBufferMoreThanZeroMessages_WithCount,
                    numberOfMessagesToBuffer);

                Enforce.With<ArgumentOutOfRangeException>(
                    flushAfter >= 0,
                    Resources_NonTranslatable.Exception_Messages_MustFlushAfterPositiveTimestep_WithTimestep,
                    flushAfter);
            }

            TargetDirectory = targetDirectory;
            NumberOfMessagesToBuffer = numberOfMessagesToBuffer;
            FlushAfter = flushAfter;
        }

        #region Implementation of ILoggerConfiguration

        /// <summary>
        /// Gets the target directory of the log file.
        /// </summary>
        /// <value>The target directory of the log file.</value>
        public string TargetDirectory
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of messages that should be buffered.
        /// </summary>
        /// <value>The number of messages that should be buffered.</value>
        public int NumberOfMessagesToBuffer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value which indicates after how many miliseconds the buffered
        /// messages should be flushed.
        /// </summary>
        /// <value>The time after which buffered messages should be flushed.</value>
        public int FlushAfter
        {
            get;
            private set;
        }

        #endregion
    }
}
