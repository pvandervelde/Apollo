//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Apollo.Core.Messaging;

namespace Apollo.Core.Logging
{
    [Serializable]
    internal sealed class LogLevelChangeRequestMessage : MessageBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogLevelChangeRequestMessage"/> class.
        /// </summary>
        /// <param name="newLevel">The new log level.</param>
        public LogLevelChangeRequestMessage(LogLevel newLevel)
            : base(false)
        {
            Level = newLevel;
        }

        /// <summary>
        /// Gets the new log level.
        /// </summary>
        /// <value>The new log level.</value>
        public LogLevel Level
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
            return new LogLevelChangeRequestMessage(Level);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MessageBody"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="MessageBody"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="MessageBody"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(MessageBody other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var message = other as LogLevelChangeRequestMessage;
            return (message != null) && (Level == message.Level);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ Level.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Log level change request to level: {0}", Level);
        }

        #endregion
    }
}
