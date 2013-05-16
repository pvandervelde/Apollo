//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using Apollo.UI.Wpf.Properties;

namespace Apollo.UI.Wpf.Models
{
    /// <summary>
    /// Stores data about a file that was recently opened by the current application.
    /// </summary>
    [TypeConverter(typeof(MostRecentlyUsedConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.String)]
    public sealed class MostRecentlyUsed
    {
        /// <summary>
        /// Deserializes the most recently used information from a string.
        /// </summary>
        /// <param name="culture">The serialization culture.</param>
        /// <param name="mru">The string containing the serialized most recently used information.</param>
        /// <returns>The most recently used information.</returns>
        internal static MostRecentlyUsed Deserialize(CultureInfo culture, string mru)
        {
            string[] parts = mru.Split(new char[] { '|' });
            var result = new MostRecentlyUsed(parts[0], DateTimeOffset.ParseExact(parts[1], "o", culture));
            return result;
        }

        /// <summary>
        /// Serializes the most recently used information to a string.
        /// </summary>
        /// <param name="culture">The serialization culture.</param>
        /// <param name="mru">The most recently used item.</param>
        /// <returns>A string containing the most recently used information.</returns>
        internal static string Serialize(CultureInfo culture, MostRecentlyUsed mru)
        {
            return string.Format(culture, "{0}|{1:o}", mru.FilePath, mru.LastTimeOpened);
        }

        /// <summary>
        /// The full path of the file that was opened.
        /// </summary>
        private readonly string m_FilePath;

        /// <summary>
        /// The date and time on which the file was last opened.
        /// </summary>
        private readonly DateTimeOffset m_LastOpened;

        /// <summary>
        /// Initializes a new instance of the <see cref="MostRecentlyUsed"/> class.
        /// </summary>
        /// <param name="filePath">The full file path for the file.</param>
        /// <param name="lastOpened">The last time the file was opened by the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="filePath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="filePath"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="lastOpened"/> is <see langword="DateTimeOffset.MinValue" /> or <see cref="DateTimeOffset.MaxValue"/>.
        /// </exception>
        public MostRecentlyUsed(string filePath, DateTimeOffset lastOpened)
        {
            {
                Lokad.Enforce.Argument(() => filePath);
                Lokad.Enforce.Argument(() => filePath, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.With<ArgumentException>(
                    !lastOpened.Equals(DateTimeOffset.MinValue),
                    Resources.Exceptions_Messages_ArgumentException);
                Lokad.Enforce.With<ArgumentException>(
                    !lastOpened.Equals(DateTimeOffset.MaxValue),
                    Resources.Exceptions_Messages_ArgumentException);
            }

            m_FilePath = filePath;
            m_LastOpened = lastOpened;
        }

        /// <summary>
        /// Gets the full path for the file.
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_FilePath;
            }
        }

        /// <summary>
        /// Gets the date and time the file was last opened.
        /// </summary>
        public DateTimeOffset LastTimeOpened
        {
            get 
            {
                return m_LastOpened;
            }
        }
    }
}
