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
    [Serializable]
    internal sealed class LogMessage : ILogMessage
    {
        /// <summary>
        /// The text for the log message.
        /// </summary>
        private readonly string m_Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="origin">The origin of the log message.</param>
        /// <param name="level">The level of the log message.</param>
        /// <param name="text">The text for the log message.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="origin"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="level"/> is <see cref="LogLevel.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="text"/> is <see langword="null" />.
        /// </exception>
        public LogMessage(string origin, LogLevel level, string text)
        {
            {
                Enforce.Argument(() => origin);
                Enforce.Argument(() => origin, StringIs.NotEmpty);

                Enforce.With<ArgumentException>(level != LogLevel.None, Resources_NonTranslatable.Exceptions_Messages_CannotLogMessageWithLogLevelSetToNone);

                Enforce.Argument(() => text);
            }

            Origin = origin;
            Level = level;
            m_Text = text;
        }

        #region Implementation of ILogMessage

        /// <summary>
        /// Gets the origin of the message. The origin can for instance be the
        /// type from which the message came.
        /// </summary>
        /// <value>The type of the owner.</value>
        public string Origin
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the desired log level for this message.
        /// </summary>
        /// <value>The desired level.</value>
        public LogLevel Level
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the message text for this message.
        /// </summary>
        /// <returns>
        /// The text for this message.
        /// </returns>
        public string Text()
        {
            return m_Text;
        }

        #endregion
    }
}
