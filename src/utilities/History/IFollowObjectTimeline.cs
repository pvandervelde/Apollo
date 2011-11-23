//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for an object that stores the timeline information for another object.
    /// </summary>
    internal interface IFollowObjectTimeline
    {
        /// <summary>
        /// Gets the ID number of the timeline object.
        /// </summary>
        HistoryId Id
        {
            get;
        }

        /// <summary>
        /// Gets the point on the timeline where the object was created.
        /// </summary>
        TimeMarker CreationTime
        {
            get;
        }

        /// <summary>
        /// Gets the point on the timeline where the object was destroyed.
        /// </summary>
        TimeMarker DeletionTime
        {
            get;
        }

        /// <summary>
        /// Returns a value indicating if the object is currently alive or not.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the object is currently alive; otherwise, <see langword="false" />.
        /// </returns>
        bool IsAlive();

        /// <summary>
        /// Adds the object to the timeline and sets the creation time.
        /// </summary>
        /// <param name="creationTime">The creation time for the object.</param>
        void AddToTimeline(TimeMarker creationTime);

        /// <summary>
        /// Deletes the object from the timeline.
        /// </summary>
        /// <param name="deletionTime">The time at which the object is deleted from the timeline.</param>
        void DeleteFromTimeline(TimeMarker deletionTime);

        /// <summary>
        /// Rolls the current value back to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        void RollBackTo(TimeMarker marker);

        /// <summary>
        /// An event raised when the object state is rolled back.
        /// </summary>
        event EventHandler<EventArgs> OnRollBack;

        /// <summary>
        /// Rolls the current value forward to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        void RollForwardTo(TimeMarker marker);

        /// <summary>
        /// An event raised when the object state is rolled forward.
        /// </summary>
        event EventHandler<EventArgs> OnRollForward;

        /// <summary>
        /// Stores the current object state for the given marker.
        /// </summary>
        /// <param name="marker">The marker which indicates at which point on the timeline the data is stored.</param>
        void Mark(TimeMarker marker);
    }
}
