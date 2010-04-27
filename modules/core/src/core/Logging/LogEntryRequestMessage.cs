//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Messaging;
using Apollo.Core.Properties;
using Lokad;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// A message that is send when a log entry must be written.
    /// </summary>
    [Serializable]
    internal sealed class LogEntryRequestMessage : MessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryRequestMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logType">The log type for this message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public LogEntryRequestMessage(ILogMessage message, LogType logType)
            : base(false)
        {
            {
                Enforce.Argument(() => message);
                Enforce.With<ArgumentException>(logType != LogType.None, Resources_NonTranslatable.Exceptions_Messages_IncorrectLogType, logType);
            }

            Message = message;
            LogType = logType;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public ILogMessage Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the log type for this message.
        /// </summary>
        /// <value>The log type.</value>
        public LogType LogType
        {
            get;
            private set;
        }

        #region Overrides of MessageBody

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>
        /// A new copy of the current <c>MessageBody</c>.
        /// </returns>
        public override MessageBody Copy()
        {
            return new LogEntryRequestMessage(Message, LogType);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MessageBody"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="MessageBody"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="MessageBody"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(MessageBody other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var message = other as LogEntryRequestMessage;
            return message != null && (LogType == message.LogType) && Message.Equals(message.Message);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ LogType.GetHashCode() ^ Message.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Log entry request for {0} with text: {1}", LogType, Message);
        }

        #endregion
    }
}
