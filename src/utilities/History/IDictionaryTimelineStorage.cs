//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the base interface for <see cref="IDictionary{TKey, TValue}"/> objects that have to track the history of the
    /// collection.
    /// </summary>
    /// <typeparam name="TKey">The type of object that is used as key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of object that is stored in the collection.</typeparam>
    public interface IDictionaryTimelineStorage<TKey, TValue> : IDictionary<TKey, TValue>, IStoreTimelineValues
    {
    }
}
