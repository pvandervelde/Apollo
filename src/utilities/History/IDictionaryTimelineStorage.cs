//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the base interface for <see cref="IDictionary{TKey, TValue}"/> objects that have to track the history of the
    /// collection.
    /// </summary>
    /// <typeparam name="TKey">The type of object that is used as key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of object that is stored in the collection.</typeparam>
    /// <remarks>
    /// We're mirroring the <see cref="IDictionary{TKey, TValue}"/> type here because otherwise we can't differentiate 
    /// between dictionaries that should track their history and dictionaries that shouldn't ....
    /// </remarks>
    [DefineAsHistoryTrackingInterface]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "IDictionary isn't called collection either.")]
    public interface IDictionaryTimelineStorage<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// An event raised when the the stored value is changed externally.
        /// </summary>
        event EventHandler<EventArgs> OnExternalValueUpdate;
    }
}
