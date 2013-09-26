//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Defines the interface for objects that store history markers for the dataset.
    /// </summary>
    internal interface IStoreHistoryMarkers : IEnumerable<TimeMarker>
    {
        /// <summary>
        /// Adds a <see cref="TimeMarker"/> to the storage.
        /// </summary>
        /// <param name="marker">The time marker.</param>
        void Add(TimeMarker marker);
    }
}
