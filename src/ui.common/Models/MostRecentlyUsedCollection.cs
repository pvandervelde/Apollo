//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Apollo.UI.Wpf.Models
{
    /// <summary>
    /// Stores a collection of most recently used items.
    /// </summary>
    [TypeConverter(typeof(MostRecentlyUsedCollectionConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.String)]
    public sealed class MostRecentlyUsedCollection : IEnumerable<MostRecentlyUsed>
    {
        /// <summary>
        /// The default number of items that will be remembered by the list.
        /// </summary>
        private const int DefaultSize = 10;

        /// <summary>
        /// Deserializes the collection from a string representation.
        /// </summary>
        /// <param name="culture">The culture that should be used for the deserialization.</param>
        /// <param name="serializedCollection">The string that contains the serialized most recently used information.</param>
        /// <returns>A new collection with the deserialized information.</returns>
        internal static MostRecentlyUsedCollection Deserialize(CultureInfo culture, string serializedCollection)
        {
            var result = new MostRecentlyUsedCollection();

            string[] parts = serializedCollection.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            result.MaximumSize = int.Parse(parts[0], culture);

            for (int i = 1; i < parts.Length; i++)
            {
                var item = MostRecentlyUsed.Deserialize(culture, parts[i]);
                result.m_Collection.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Serializes the collection to a string representation.
        /// </summary>
        /// <param name="culture">The culture that should be used for the deserialization.</param>
        /// <param name="serializedCollection">The collection that contains the most recently used information.</param>
        /// <returns>A new string with the serialized information.</returns>
        internal static string Serialize(CultureInfo culture, MostRecentlyUsedCollection serializedCollection)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Format(culture, "{0}", serializedCollection.MaximumSize));
            foreach (var mru in serializedCollection)
            {
                builder.AppendLine(MostRecentlyUsed.Serialize(culture, mru));
            }

            return builder.ToString();
        }

        /// <summary>
        /// The collection that holds the most recently opened files.
        /// </summary>
        private readonly List<MostRecentlyUsed> m_Collection
            = new List<MostRecentlyUsed>();

        /// <summary>
        /// The maximum size of the collection.
        /// </summary>
        private int m_MaximumSize = DefaultSize;

        /// <summary>
        /// Gets or sets the maximum size of the collection.
        /// </summary>
        public int MaximumSize
        {
            get
            {
                return m_MaximumSize;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                m_MaximumSize = value;
                while (m_Collection.Count > m_MaximumSize)
                {
                    m_Collection.RemoveAt(m_Collection.Count - 1);
                }
            }
        }

        /// <summary>
        /// Adds a new file to the list of most recently used items.
        /// </summary>
        /// <param name="filePath">The path of the file that was opened.</param>
        public void Add(string filePath)
        {
            // check if it's already in there. If so remove it
            var index = m_Collection.FindIndex(m => string.Equals(m.FilePath, filePath, StringComparison.OrdinalIgnoreCase));
            if (index == -1)
            {
                // Not there, see if we have reached the maximum size of the collection
                if (m_Collection.Count == MaximumSize)
                {
                    index = MaximumSize - 1;
                }
            }

            if (index > -1)
            {
                m_Collection.RemoveAt(index);
            }

            var item = new MostRecentlyUsed(filePath, DateTimeOffset.Now);
            m_Collection.Insert(0, item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<MostRecentlyUsed> GetEnumerator()
        {
            foreach (var mru in m_Collection)
            {
                yield return mru;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
