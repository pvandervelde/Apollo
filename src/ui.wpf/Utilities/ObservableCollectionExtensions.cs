//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;

namespace Apollo.UI.Wpf.Utilities
{
    /// <summary>
    /// Defines extension methods for the <see cref="ObservableCollection{T}"/>.
    /// </summary>
    internal static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Searches through the collection until the first item that matches the predicate is found.
        /// </summary>
        /// <typeparam name="T">The type of object that is stored in the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="predicate">The predicate which indicates what object should be searched for.</param>
        /// <returns>The index of the first object to match the predicate; otherwise, -1.</returns>
        public static int FindIndex<T>(this ObservableCollection<T> collection, Predicate<T> predicate)
        {
            int index = -1;
            if (collection != null)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (predicate(collection[i]))
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }
    }
}
