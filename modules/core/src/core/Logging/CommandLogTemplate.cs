//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Lokad;

namespace Apollo.Core.Logging
{
    /// <summary>
    /// An <see cref="ILogTemplate"/> object that writes to the command log.
    /// </summary>
    internal sealed class CommandLogTemplate : ILogTemplate, IEquatable<CommandLogTemplate>
    {
        /// <summary>
        /// A function that returns the current time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_GetCurrentTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLogTemplate"/> class.
        /// </summary>
        /// <param name="getCurrentTime">A function that gets the current time.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="getCurrentTime"/> is <see langword="null" />.
        /// </exception>
        public CommandLogTemplate(Func<DateTimeOffset> getCurrentTime)
        {
            {
                Enforce.Argument(() => getCurrentTime);
            }

            m_GetCurrentTime = getCurrentTime;
        }

        #region Implementation of ILogTemplate

        /// <summary>
        /// Gets the type of the log for which this template functions.
        /// </summary>
        /// <value>The type of the log.</value>
        public LogType LogType
        {
            get
            {
                return LogType.Command;
            }
        }

        /// <summary>
        /// Gets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        public string Name
        {
            get
            {
                return "CommandLog";
            }
        }

        /// <summary>
        /// Translates the specified message.
        /// </summary>
        /// <param name="message">The message that must be translated.</param>
        /// <returns>
        /// The desired string representation of the log message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public string Translate(ILogMessage message)
        {
            {
                Enforce.Argument(() => message);
            }

            return string.Format(CultureInfo.CurrentCulture, "{0}\tCommand - {1}: {2}", m_GetCurrentTime(), message.Origin, message.Text());
        }

        #endregion

        #region Implementation of IEquatable<CommandLogTemplate>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        public bool Equals(CommandLogTemplate other)
        {
            // All CommandLogTemplate objects are created equal, except for the null objects.
            return other != null;
        }

        #endregion

        #region Implementation of IEquatable<ILogTemplate>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(ILogTemplate other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var template = other as CommandLogTemplate;
            return template != null && Equals(template);
        }

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        /// </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var template = obj as CommandLogTemplate;
            return template != null && Equals(template);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ Name.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
