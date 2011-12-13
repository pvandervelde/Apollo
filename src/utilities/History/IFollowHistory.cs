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
    /// Defines the interface for objects that control history storage.
    /// </summary>
    public interface IFollowHistory
    {
        /// <summary>
        /// Gets the current time marker.
        /// </summary>
        TimeMarker Current
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether it is possible to roll-back to a previous value.
        /// </summary>
        bool CanRollBack
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether it is possible to roll-forward to a next value.
        /// </summary>
        bool CanRollForward
        {
            get;
        }

        /// <summary>
        /// Rolls the state back to the given point in time and provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The point in time to roll-back to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-back.</param>
        void RollBackTo(TimeMarker mark, TimelineTraveller traveller = null);

        /// <summary>
        /// Rolls the state back to the named point in time and provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The named point in time to roll-back to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-back.</param>
        void RollBackTo(string mark, TimelineTraveller traveller = null);

        /// <summary>
        /// An event that is raised just before a roll-back takes place.
        /// </summary>
        event EventHandler<EventArgs> OnRollingBack;

        /// <summary>
        /// An event that is raised just after a roll-back has taken place.
        /// </summary>
        event EventHandler<EventArgs> OnRolledBack;

        /// <summary>
        /// Rolls the state forward to the given point in time an provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The point in time to roll-foward to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-forward.</param>
        void RollForwardTo(TimeMarker mark, TimelineTraveller traveller = null);

        /// <summary>
        /// Rolls the state forward to the named point in time an provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The named point in time to roll-foward to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-forward.</param>
        void RollForwardTo(string mark, TimelineTraveller traveller = null);

        /// <summary>
        /// An even that is raised just before a roll-forward takes place.
        /// </summary>
        event EventHandler<EventArgs> OnRollingForward;

        /// <summary>
        /// An even that is raised just after a roll-forward has taken place.
        /// </summary>
        event EventHandler<EventArgs> OnRolledForward;

        /// <summary>
        /// Stores the current state and returns the <see cref="TimeMarker"/> that belongs to this change set.
        /// </summary>
        /// <returns>The time marker for the stored state.</returns>
        TimeMarker Mark();

        /// <summary>
        /// Stores the current state and the dependencies and returns the <see cref="TimeMarker"/> that belongs to 
        /// this change set.
        /// </summary>
        /// <param name="dependencies">The dependencies that indicate if the current change set can be rolled back or rolled forward.</param>
        /// <returns>The time marker for the stored state.</returns>
        TimeMarker Mark(IEnumerable<UpdateFromHistoryDependency> dependencies);

        /// <summary>
        /// Stores the current state with the given name and returns the <see cref="TimeMarker"/> that 
        /// belongs to this change set.
        /// </summary>
        /// <param name="name">The name for the state.</param>
        /// <returns>The time marker for the stored state.</returns>
        TimeMarker Mark(string name);

        /// <summary>
        /// Stores the current state with the given name and the dependencies  and returns the <see cref="TimeMarker"/> that 
        /// belongs to this change set.
        /// </summary>
        /// <param name="name">The name for the state.</param>
        /// <param name="dependencies">The dependencies that indicate if the current change set can be rolled back or rolled forward.</param>
        /// <returns>The time marker for the stored state.</returns>
        TimeMarker Mark(string name, IEnumerable<UpdateFromHistoryDependency> dependencies);

        /// <summary>
        /// An event that is raised when the current change set is stored in the timeline.
        /// </summary>
        event EventHandler<TimelineMarkEventArgs> OnMark;
    }
}
