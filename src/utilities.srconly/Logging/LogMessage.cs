﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Lokad;
using Lokad.Rules;

namespace Apollo.Utilities.Logging
{
    /// <summary>
    /// Defines a message that should be logged by an <see cref="ILogger"/> object.
    /// </summary>
    [Serializable]
    internal sealed class LogMessage : ILogMessage
    {
        /// <summary>
        /// The collection that stores the properties for the message.
        /// </summary>
        private readonly IDictionary<string, object> m_Properties;

        /// <summary>
        /// The text for the log message.
        /// </summary>
        private readonly string m_Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="level">The level of the log message.</param>
        /// <param name="text">The text for the log message.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="level"/> is <see cref="LevelToLog.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="text"/> is <see langword="null" />.
        /// </exception>
        public LogMessage(LevelToLog level, string text)
            : this(level, text, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="level">The level of the log message.</param>
        /// <param name="text">The text for the log message.</param>
        /// <param name="properties">The dictionary that contains the additional properties for the current message.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="level"/> is <see cref="LevelToLog.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="text"/> is <see langword="null" />.
        /// </exception>
        public LogMessage(LevelToLog level, string text, IDictionary<string, object> properties)
        {
            {
                Enforce.With<ArgumentException>(level != LevelToLog.None, SrcOnlyResources.ExceptionMessagesCannotLogMessageWithLogLevelSetToNone);

                Enforce.Argument(() => text);
            }

            Level = level;
            m_Text = text;
            m_Properties = properties;
        }

        /// <summary>
        /// Gets the desired log level for this message.
        /// </summary>
        /// <value>The desired level.</value>
        public LevelToLog Level
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

        /// <summary>
        /// Gets a value indicating whether the message contains additional parameters that
        /// should be processed when the message is written to the log.
        /// </summary>
        public bool HasAdditionalInformation
        {
            get
            {
                return m_Properties != null;
            }
        }

        /// <summary>
        /// Gets a collection that contains additional parameters which should be
        /// processed when the message is written to the log.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Properties
        {
            get
            {
                return m_Properties;
            }
        }
    }
}