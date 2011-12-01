//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Apollo.Utilities.History
{
    internal sealed class Timeline : IFollowHistory, ITrackHistoryChanges, ICreateSnapshots
    {
        // Track all the objects and their ID's

        /// <summary>
        /// Gets the current time marker.
        /// </summary>
        public TimeMarker Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether it is possible to roll-back to a previous value.
        /// </summary>
        public bool CanRollBack
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether it is possible to roll-forward to a next value.
        /// </summary>
        public bool CanRollForward
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Rolls the state back to the given point in time and provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The point in time to roll-back to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-back.</param>
        public void RollBackTo(TimeMarker mark, TimelineTraveller traveller = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rolls the state back to the named point in time and provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The named point in time to roll-back to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-back.</param>
        public void RollBackTo(string mark, TimelineTraveller traveller = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An event that is raised just before a roll-back takes place.
        /// </summary>
        public event EventHandler<EventArgs> OnRollingBack;

        private void RaiseOnRollingBack()
        {
            var local = OnRollingBack;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event that is raised just after a roll-back has taken place.
        /// </summary>
        public event EventHandler<EventArgs> OnRolledBack;

        private void RaiseOnRolledBack()
        {
            var local = OnRolledBack;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Rolls the state forward to the given point in time an provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The point in time to roll-foward to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-forward.</param>
        public void RollForwardTo(TimeMarker mark, TimelineTraveller traveller = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rolls the state forward to the named point in time an provides the <see cref="TimelineTraveller"/>
        /// to the history objects that want it.
        /// </summary>
        /// <param name="mark">The named point in time to roll-foward to.</param>
        /// <param name="traveller">The object that stores state that needs to survive the roll-forward.</param>
        public void RollForwardTo(string mark, TimelineTraveller traveller = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An even that is raised just before a roll-forward takes place.
        /// </summary>
        public event EventHandler<EventArgs> OnRollingForward;

        private void RaiseOnRollingForward()
        {
            var local = OnRollingForward;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An even that is raised just after a roll-forward has taken place.
        /// </summary>
        public event EventHandler<EventArgs> OnRolledForward;

        private void RaiseOnRolledForward()
        {
            var local = OnRolledForward;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stores the current state and returns the <see cref="TimeMarker"/> that goes with that state.
        /// </summary>
        /// <returns>
        /// The time marker for the stored state.
        /// </returns>
        public TimeMarker Mark()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the current state with the given name and returns the <see cref="TimeMarker"/> that goes 
        /// with that state.
        /// </summary>
        /// <param name="name">The name for the state.</param>
        /// <returns>
        /// The time marker for the stored state.
        /// </returns>
        public TimeMarker Mark(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An event that is raised if a roll-back or roll-forward has taken place that has
        /// a <see cref="TimelineTraveller"/> object associated with it.
        /// </summary>
        public event EventHandler<TimeTravellerEventArgs> OnTimeTravellerArrived;

        private void RaiseOnTimeTravellerArrived(TimelineTraveller traveller)
        {
            {
                Debug.Assert(traveller != null, "This event should only be raised if there is a traveller");
            }

            var local = OnTimeTravellerArrived;
            if (local != null)
            {
                local(this, new TimeTravellerEventArgs(traveller));
            }
        }

        /// <summary>
        /// Creates a new snapshot and stores it.
        /// </summary>
        public void CreateSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts recording history and returns a change set for those changes.
        /// </summary>
        /// <returns>
        /// The change set that records the changes when it is disposed.
        /// </returns>
        public IChangeSet RecordHistory()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts recording history and returns a named change set for those changes.
        /// </summary>
        /// <param name="name">The name of the change set.</param>
        /// <returns>
        /// The change set that records the changes when it is disposed.
        /// </returns>
        public IChangeSet RecordHistory(string name)
        {
            throw new NotImplementedException();
        }
    }
}
