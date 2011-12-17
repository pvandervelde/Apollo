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
    /// Stores the timeline values for an <see cref="IList{T}"/> collection of objects of type <typeparamref name="T"/> 
    /// which are <see cref="IAmHistoryEnabled"/>.
    /// </summary>
    /// <typeparam name="T">The type of object for which the values are stored.</typeparam>
    /// <remarks>
    ///     Objects that implement <see cref="IAmHistoryEnabled"/> get special treatment because we always want to return 
    ///     the object with a given ID even if that object has changed (i.e. the object reference has changed) due to 
    ///     changes in the timeline.
    /// </remarks>
    internal sealed class HistoryObjectListHistory<T> : ListHistoryBase<T, HistoryId> where T : IAmHistoryEnabled
    {
        private static HistoryId ToId(T obj)
        {
            return obj.HistoryId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryObjectListHistory{T}"/> class.
        /// </summary>
        /// <param name="lookupFunc">The function that is used to find the object that belongs to the given <see cref="HistoryId"/>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="lookupFunc"/> is <see langword="null" />.
        /// </exception>
        public HistoryObjectListHistory(Func<HistoryId, T> lookupFunc)
            : base(ToId, lookupFunc)
        { 
        }
    }
}
