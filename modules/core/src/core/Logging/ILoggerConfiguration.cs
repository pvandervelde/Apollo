//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines the configuration for 
    /// </summary>
    internal interface ILoggerConfiguration
    {
        /// <summary>
        /// Gets the target directory of the log file.
        /// </summary>
        /// <value>The target directory of the log file.</value>
        string TargetDirectory
        {
            get;
        }

        /// <summary>
        /// Gets the number of messages that should be buffered.
        /// </summary>
        /// <value>The number of messages that should be buffered.</value>
        int NumberOfMessagesToBuffer
        {
            get;
        }

        /// <summary>
        /// Gets a value which indicates after how many miliseconds the buffered
        /// messages should be flushed.
        /// </summary>
        /// <value>The time after which buffered messages should be flushed.</value>
        int FlushAfter
        {
            get;
        }
    }
}
