//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects that store historical and future values of a 
    /// object member variable.
    /// </summary>
    public interface IStoreTimelineValues
    {
        /// <summary>
        /// Rolls the current value back to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        void RollBackTo(TimeMarker marker);

        /// <summary>
        /// Rolls the current value back to the start point.
        /// </summary>
        void RollBackToStart();

        /// <summary>
        /// Rolls the current value forward to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        void RollForwardTo(TimeMarker marker);

        /// <summary>
        /// Stores the current value in the history list with the given marker.
        /// </summary>
        /// <param name="marker">The marker which indicates at which point on the timeline the data is stored.</param>
        void StoreCurrent(TimeMarker marker);

        /// <summary>
        /// An event that is raised if a roll-back or roll-forward has taken place.
        /// </summary>
        event EventHandler<EventArgs> OnValueChanged;
    }
}
