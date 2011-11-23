//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects that store a single change to a collection.
    /// </summary>
    /// <typeparam name="T">The type of object that is stored by the collection that underwent the change.</typeparam>
    internal interface ICollectionChange<T>
    {
        /// <summary>
        /// Applies the changes in the current change to the given collection.
        /// </summary>
        /// <param name="collection">The collection to which the changes should be applied.</param>
        void ApplyTo(ICollection<T> collection);
    }
}
