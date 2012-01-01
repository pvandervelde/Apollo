//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an <see cref="IList{T}"/> collection of objects of type <typeparamref name="T"/> 
    /// which are not <see cref="IAmHistoryEnabled"/>.
    /// </summary>
    /// <typeparam name="T">The type of object for which the values are stored.</typeparam>
    public sealed class ListHistory<T> : ListHistoryBase<T, T>
    {
        private static T Translate(T input)
        {
            return input;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListHistory{T}"/> class.
        /// </summary>
        public ListHistory()
            : base(Translate, Translate)
        { 
        }
    }
}
