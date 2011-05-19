//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Utilities.Logging
{
    /// <summary>
    /// Defines a log message.
    /// </summary>
    internal interface ILogMessage
    {
        /// <summary>
        /// Gets the desired log level for this message.
        /// </summary>
        /// <value>The desired level.</value>
        LevelToLog Level
        {
            get;
        }

        /// <summary>
        /// Returns the message text for this message.
        /// </summary>
        /// <returns>
        /// The text for this message.
        /// </returns>
        string Text();

        /// <summary>
        /// Gets a value indicating whether the message contains additional parameters that
        /// should be processed when the message is written to the log.
        /// </summary>
        bool HasAdditionalInformation 
        { 
            get;
        }

        /// <summary>
        /// Gets a collection that contains additional parameters which should be
        /// processed when the message is written to the log.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> Properties 
        { 
            get;
        }
    }
}