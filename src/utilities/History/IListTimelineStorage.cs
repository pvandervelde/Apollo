//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the base interface for <see cref="IList{T}"/> objects that have to track the history of the
    /// collection.
    /// </summary>
    /// <typeparam name="T">The type of object that is stored in the collection.</typeparam>
    [DefineAsHistoryTrackingInterface]
    public interface IListTimelineStorage<T> : IList<T>
    {
        // We're mirroring the IList<T> type here because otherwise we can't differentiate between lists that should
        // track their history and lists that shouldn't ....
    }
}
