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
    /// Defines the interface for objects that store <see cref="IAmHistoryEnabled"/> objects on a timeline.
    /// </summary>
    public interface IConnectObjectsToHistory
    {
        /// <summary>
        /// Adds a new object to the timeline.
        /// </summary>
        /// <typeparam name="T">The type of object that should be added to the timeline.</typeparam>
        /// <param name="objectBuilder">
        /// The function that is used to create a new object of type <typeparamref name="T"/> with the given
        /// collection of member field objects.
        /// </param>
        /// <returns>The newly created object of type <typeparamref name="T"/>.</returns>
        T AddToTimeline<T>(Func<HistoryId, IEnumerable<Tuple<string, IStoreTimelineValues>>, T> objectBuilder)
            where T : class, IAmHistoryEnabled;

        /// <summary>
        /// Removes the object with the given <see cref="HistoryId"/> from the timeline.
        /// </summary>
        /// <param name="id">The ID of the object that should be removed.</param>
        void RemoveFromTimeline(HistoryId id);

        /// <summary>
        /// Returns the object that belongs to the given <see cref="HistoryId"/>.
        /// </summary>
        /// <typeparam name="T">The type of object that should be returned.</typeparam>
        /// <param name="id">The ID of the object that should be returned.</param>
        /// <returns>
        ///     The object that belongs to the given <see cref="HistoryId"/> if the object exists at the current
        ///     <see cref="TimeMarker"/>; otherwise, <see langword="null" />.
        /// </returns>
        T IdToObject<T>(HistoryId id) where T : class, IAmHistoryEnabled;

        /// <summary>
        /// Returns a value indicating if the object that belongs to the given <see cref="HistoryId"/>
        /// exist at the current <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="id">The ID of the object.</param>
        /// <returns>
        /// <see langword="true" /> if the object exists at the current <see cref="TimeMarker"/>; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool DoesObjectExistCurrently(HistoryId id);

        /// <summary>
        /// Returns a value indicating if the object that belongs to the given <see cref="HistoryId"/>
        /// exist at any point in time.
        /// </summary>
        /// <param name="id">The ID of the object.</param>
        /// <returns>
        /// <see langword="true" /> if the object exists at any point in time; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasObjectEverExisted(HistoryId id);
    }
}
