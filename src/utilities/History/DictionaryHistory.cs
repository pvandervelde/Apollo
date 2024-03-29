﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an <see cref="IDictionary{TKey, TValue}"/> collection of objects of type <typeparamref name="TValue"/> 
    /// which are not <see cref="IAmHistoryEnabled"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of object that is used as key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of object that is passed into the collection.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "The original is called Dictionary<TKey, TValue> not collection something.")]
    public sealed class DictionaryHistory<TKey, TValue> : DictionaryHistoryBase<TKey, TValue, TValue>
    {
        private static TValue Translate(TValue input)
        {
            return input;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryHistory{TKey, TValue}"/> class.
        /// </summary>
        public DictionaryHistory()
            : base(Translate, Translate)
        { 
        }
    }
}
