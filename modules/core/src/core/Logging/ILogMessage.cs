//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Logging
{
    /// <summary>
    /// Defines a log message.
    /// </summary>
    internal interface ILogMessage
    {
        /// <summary>
        /// Gets the origin of the message. The origin can for instance be the
        /// type from which the message came.
        /// </summary>
        /// <value>The type of the owner.</value>
        string Origin
        {
            get;
        }

        /// <summary>
        /// Gets the desired log level for this message.
        /// </summary>
        /// <value>The desired level.</value>
        LogLevel Level
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
    }
}