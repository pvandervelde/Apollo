//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an <see cref="IDictionary{TKey, TValue}"/> collection of objects of type <typeparamref name="TValue"/> 
    /// which are <see cref="IAmHistoryEnabled"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of object that is used as key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of object that is passed into the collection.</typeparam>
    /// <remarks>
    ///     Objects that implement <see cref="IAmHistoryEnabled"/> get special treatment because we always want to return 
    ///     the object with a given ID even if that object has changed (i.e. the object reference has changed) due to 
    ///     changes in the timeline.
    /// </remarks>
    public sealed class HistoryObjectDictionaryHistory<TKey, TValue> 
        : DictionaryHistoryBase<TKey, TValue, HistoryId> where TValue : IAmHistoryEnabled
    {
        private static HistoryId ToId(TValue obj)
        {
            return obj.HistoryId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryObjectDictionaryHistory{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="lookupFunc">The function that is used to find the object that belongs to the given <see cref="HistoryId"/>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="lookupFunc"/> is <see langword="null" />.
        /// </exception>
        public HistoryObjectDictionaryHistory(Func<HistoryId, TValue> lookupFunc)
            : base(ToId, lookupFunc)
        { 
        }
    }
}
