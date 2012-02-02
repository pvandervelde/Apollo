//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Provides the history for a specific type of object.
    /// </summary>
    /// <typeparam name="T">The type of object stored in the storage.</typeparam>
    public sealed class ValueAtTimeStorage<T>
    {
        /// <summary>
        /// The collection that contains the values that are in the past.
        /// </summary>
        private readonly LinkedList<ValueAtTime<T>> m_PastValues
            = new LinkedList<ValueAtTime<T>>();

        /// <summary>
        /// The collection that contains the values that are in the future.
        /// </summary>
        private readonly LinkedList<ValueAtTime<T>> m_FutureValues
            = new LinkedList<ValueAtTime<T>>();

        /// <summary>
        /// Gets the last value that was stored.
        /// </summary>
        public T LastValue
        {
            get
            {
                if (m_PastValues.Last == null)
                {
                    return default(T);
                }

                return m_PastValues.Last.Value.Value;
            }
        }

        /// <summary>
        /// Gets the last time at which a value was stored.
        /// </summary>
        public TimeMarker LastTime
        {
            get
            {
                if (m_PastValues.Last == null)
                {
                    return TimeMarker.TheBeginOfTime;
                }

                return m_PastValues.Last.Value.Time;
            }
        }

        /// <summary>
        /// Iterates over the past values starting at the most recent value.
        /// </summary>
        /// <param name="visitor">
        /// The function that is invoked for each stored value. Should return
        /// <see langword="false"/> to stop the iteration; otherwise, <see langword="true" />.
        /// </param>
        public void TrackBackwardsInTime(Func<TimeMarker, T, bool> visitor)
        {
            bool shouldContinue = true;
            var node = m_PastValues.Last;
            while ((node != null) && shouldContinue)
            {
                shouldContinue = visitor(node.Value.Time, node.Value.Value);
                node = node.Previous;
            }
        }

        /// <summary>
        /// Iterates over the future values starting at the most recent value.
        /// </summary>
        /// <param name="visitor">
        /// The function that is invoked for each stored value. Should return
        /// <see langword="false"/> to stop the iteration; otherwise, <see langword="true" />.
        /// </param>
        public void TrackForwardsInTime(Func<TimeMarker, T, bool> visitor)
        {
            bool shouldContinue = true;
            var node = m_FutureValues.First;
            while ((node != null) && shouldContinue)
            {
                shouldContinue = visitor(node.Value.Time, node.Value.Value);
                node = node.Next;
            }
        }

        /// <summary>
        /// Returns a value indicating if the history is currently at the beginning of known time, 
        /// meaning that we can only move forward.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if history is at the beginning of known time; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsAtBeginOfTime()
        {
            return m_PastValues.Count == 0;
        }

        /// <summary>
        /// Returns a value indicating if moving back to the given time marker would 
        /// require moving to a point prior to the known beginning of time.
        /// </summary>
        /// <param name="mark">The mark to move back to.</param>
        /// <returns>
        ///     <see langword="true" /> if moving back to the marker requires moving to a point prior to the 
        ///     known beginning of time; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool WouldRollBackPastTheBeginningOfTime(TimeMarker mark)
        {
            if (IsAtBeginOfTime())
            {
                return true;
            }

            var node = m_PastValues.First;
            return node.Value.Time > mark;
        }

        /// <summary>
        /// Returns a value indicating if the history is currently at the end of known time, 
        /// meaning that we can only move backward.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if history is at the end of known time; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsAtEndOfTime()
        {
            return m_FutureValues.Count == 0;
        }

        /// <summary>
        /// Rolls the current value back to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        /// <param name="action">The action that should be executed on each single time step which is rolled back.</param>
        /// <returns>
        /// The value on which the roll-back finishes. Essentially the new current value.
        /// </returns>
        /// <exception cref="NoPreviousValueRegisteredException">
        /// Thrown when the collection is already at the start of time.
        /// </exception>
        public T RollBackTo(TimeMarker marker, Action<TimeMarker, T> action = null)
        {
            if (IsAtBeginOfTime())
            {
                throw new NoPreviousValueRegisteredException();
            }

            if (!WouldRollBackPastTheBeginningOfTime(marker))
            {
                return RollBackInTimeTo(marker, action);
            }
            else
            {
                return RollBackInTimeTo(TimeMarker.TheBeginOfTime, action);
            }
        }

        /// <summary>
        /// Moves the history back enough steps to make the last known time is equal to or smaller than the
        /// provided mark.
        /// </summary>
        /// <param name="mark">The mark that indicates to which point in time the roll-back should be performed.</param>
        /// <param name="action">The action that should be executed on each single time step which is rolled back.</param>
        /// <returns>
        ///     The last recorded value.
        /// </returns>
        private T RollBackInTimeTo(TimeMarker mark, Action<TimeMarker, T> action)
        {
            {
                Debug.Assert(!IsAtBeginOfTime(), "Should not roll back past the beginning of time.");
            }

            bool hasChanged = false;

            // We assume that it is more likely that we have a more recent
            // point in time that we want to roll back to. So start searching
            // from the end of the collection
            var lastNode = m_PastValues.Last;
            while ((lastNode != null) && (lastNode.Value.Time > mark))
            {
                m_PastValues.RemoveLast();
                m_FutureValues.AddFirst(lastNode);
                
                if (action != null)
                {
                    action(lastNode.Value.Time, lastNode.Value.Value);
                }

                lastNode = m_PastValues.Last;
                hasChanged = true;
            }

            if (hasChanged)
            {
                RaiseOnCurrentValueChange();
            }

            return (m_PastValues.Last != null) ? m_PastValues.Last.Value.Value : default(T);
        }

        /// <summary>
        /// Rolls the current value back to the start point.
        /// </summary>
        /// <param name="action">The action that should be executed on each single time step which is rolled back.</param>
        /// <returns>
        /// The value on which the roll-back finishes. Essentially the new current value.
        /// </returns>
        /// <exception cref="NoPreviousValueRegisteredException">
        /// Thrown when the collection is already at the start of time.
        /// </exception>
        public T RollBackToStart(Action<TimeMarker, T> action = null)
        {
            if (IsAtBeginOfTime())
            {
                throw new NoPreviousValueRegisteredException();
            }

            return RollBackInTimeTo(TimeMarker.TheBeginOfTime, action);
        }

        /// <summary>
        /// Rolls the current value forward to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        /// <param name="action">The action that should be executed on each single time step which is rolled back.</param>
        /// <returns>
        /// The value on which the roll-back finishes. Essentially the new current value.
        /// </returns>
        /// <exception cref="NoPreviousValueRegisteredException">
        /// Thrown when the collection is already at the start of time.
        /// </exception>
        public T RollForwardTo(TimeMarker marker, Action<TimeMarker, T> action = null)
        {
            if (IsAtEndOfTime())
            {
                throw new NoFutureValueRegisteredException();
            }

            return RollForwardInTimeTo(marker, action);
        }

        /// <summary>
        /// Moves the history forward enough steps to make the next future time is equal to or larger than the
        /// provided mark.
        /// </summary>
        /// <param name="mark">The mark that indicates to which point in time the roll-forward should be performed.</param>
        /// <param name="action">The action that should be executed on each single time step which is rolled back.</param>
        /// <returns>
        ///     The last recorded value.
        /// </returns>
        private T RollForwardInTimeTo(TimeMarker mark, Action<TimeMarker, T> action)
        {
            {
                Debug.Assert(!IsAtEndOfTime(), "Should not be at the end of time.");
            }

            bool hasChanged = false;

            // We assume that it is more likely that we have a more recent
            // point in time that we want to roll forward to. So start searching
            // from the start of the collection
            while ((m_FutureValues.First != null) && (m_FutureValues.First.Value.Time <= mark))
            {
                var lastNode = m_FutureValues.First;
                m_FutureValues.RemoveFirst();
                m_PastValues.AddLast(lastNode);

                if (action != null)
                {
                    action(lastNode.Value.Time, lastNode.Value.Value);
                }

                hasChanged = true;
            }

            if (hasChanged)
            {
                RaiseOnCurrentValueChange();
            }

            return (m_PastValues.Last != null) ? m_PastValues.Last.Value.Value : default(T);
        }

        /// <summary>
        /// An event raised when the current value is changed due to a roll-back or roll-forward.
        /// </summary>
        public event EventHandler<EventArgs> OnCurrentValueChange;

        private void RaiseOnCurrentValueChange()
        {
            var local = OnCurrentValueChange;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stores the current value in the history list with the given marker.
        /// </summary>
        /// <param name="marker">The marker which indicates at which point on the timeline the data is stored.</param>
        /// <param name="current">The current value.</param>
        public void StoreCurrent(TimeMarker marker, T current)
        {
            m_PastValues.AddLast(new LinkedListNode<ValueAtTime<T>>(new ValueAtTime<T>(marker, current)));
            ForgetTheFuture();
        }

        /// <summary>
        /// Clears all the history storage and forgets all the 
        /// stored historic information.
        /// </summary>
        public void ForgetAllHistory()
        {
            ForgetThePast();
            ForgetTheFuture();
        }

        /// <summary>
        /// Clear all the history information that is in the past.
        /// </summary>
        public void ForgetThePast()
        {
            m_PastValues.Clear();
        }

        /// <summary>
        /// Clears all the history information that is in the future.
        /// </summary>
        public void ForgetTheFuture()
        {
            m_FutureValues.Clear();
        }
    }
}
