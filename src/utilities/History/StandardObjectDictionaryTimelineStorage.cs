//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an <see cref="IDictionary{TKey, TValue}"/> collection of objects of type <typeparamref name="TValue"/> 
    /// which are not <see cref="IAmHistoryEnabled"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of object that is used as key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of object that is passed into the collection.</typeparam>
    internal sealed class StandardObjectDictionaryTimelineStorage<TKey, TValue> : DictionaryTimelineStorage<TKey, TValue, TValue>
    {
        private static TValue Translate(TValue input)
        {
            return input;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardObjectDictionaryTimelineStorage{TKey, TValue}"/> class.
        /// </summary>
        public StandardObjectDictionaryTimelineStorage()
            : base(Translate, Translate)
        { 
        }
    }
}
