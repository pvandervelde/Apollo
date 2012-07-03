//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset.Utilities
{
    /// <summary>
    /// Stores history markers for the dataset.
    /// </summary>
    internal sealed class HistoryMarkerStorage : IStoreHistoryMarkers
    {
        /// <summary>
        /// The collection of markers.
        /// </summary>
        private readonly List<TimeMarker> m_Markers = new List<TimeMarker>();

        /// <summary>
        /// Adds a <see cref="TimeMarker"/> to the storage.
        /// </summary>
        /// <param name="marker">The time marker.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="marker"/> is <see langword="null" />.
        /// </exception>
        public void Add(TimeMarker marker)
        {
            {
                Lokad.Enforce.Argument(() => marker);
            }

            m_Markers.Add(marker);
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<TimeMarker> GetEnumerator()
        {
            foreach (var marker in m_Markers)
            {
                yield return marker;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through 
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
