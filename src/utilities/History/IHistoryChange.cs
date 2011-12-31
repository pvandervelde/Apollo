//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects that store a single change to a history object.
    /// </summary>
    /// <typeparam name="T">The type of history object.</typeparam>
    public interface IHistoryChange<T>
    {
        /// <summary>
        /// Applies the changes in the current change to the given history object.
        /// </summary>
        /// <param name="historyObject">The object to which the changes should be applied.</param>
        void ApplyTo(T historyObject);
    }
}
