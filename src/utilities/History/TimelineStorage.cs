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
    /// Stores the timeline values for a given object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object for which the values are stored.</typeparam>
    /// <design>
    /// At the moment we just use 2 linked lists for the undo / redo collections. This means that we
    /// have the two lists and a LinkedListNode for each level (with 2 pointers + a value pointer) allocated.
    /// If we find that this is too much then we can always change the design to something more 'cunning'.
    /// e.g. a system where we store everything in an array or something.
    /// </design>
    internal abstract class TimelineStorage<T> : IStoreTimelineValues
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
        /// The current value of the variable.
        /// </summary>
        private T m_Current;

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
        private bool WouldRollBackPastTheBeginningOfTime(TimeMarker mark)
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
        public void RollBackTo(TimeMarker marker)
        {
            if (!IsAtBeginOfTime())
            {
                if (!WouldRollBackPastTheBeginningOfTime(marker))
                {
                    m_Current = RollBackInTimeTo(marker);
                }
                else
                {
                    m_Current = RollBackToBeginning();
                }

                RaiseOnValueChanged();
            }
        }

        /// <summary>
        /// Moves the history back enough steps to make the last known time is equal to or smaller than the
        /// provided mark.
        /// </summary>
        /// <param name="mark">The mark that indicates to which point in time the roll-back should be performed.</param>
        /// <returns>
        ///     The last recorded value.
        /// </returns>
        private T RollBackInTimeTo(TimeMarker mark)
        {
            {
                Debug.Assert(!IsAtBeginOfTime() && !WouldRollBackPastTheBeginningOfTime(mark), "Should not roll back past the beginning of time.");
            }

            // We assume that it is more likely that we have a more recent
            // point in time that we want to roll back to. So start searching
            // from the end of the collection
            var lastNode = m_PastValues.Last;
            while ((lastNode != null) && (lastNode.Value.Time > mark))
            {
                m_PastValues.RemoveLast();
                m_FutureValues.AddFirst(lastNode);

                lastNode = m_PastValues.Last;
            }

            return (m_PastValues.Last != null) ? m_PastValues.Last.Value.Value : default(T);
        }

        /// <summary>
        /// Moves the history back enough steps to make the last known time is equal to the first recorded value
        /// in time.
        /// </summary>
        /// <returns>
        ///     The first recorded value in time.
        /// </returns>
        private T RollBackToBeginning()
        {
            {
                Debug.Assert(!IsAtBeginOfTime(), "Cannot roll-back to the beginning of time if without data stored.");
            }

            return RollBackInTimeTo(TimeMarker.TheBeginOfTime);
        }

        /// <summary>
        /// Rolls the current value back to the start point.
        /// </summary>
        public void RollBackToStart()
        {
            if (!IsAtBeginOfTime())
            {
                m_Current = RollBackToBeginning();
                RaiseOnValueChanged();
            }
        }

        /// <summary>
        /// Rolls the current value forward to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollForwardTo(TimeMarker marker)
        {
            if (!IsAtEndOfTime())
            {
                m_Current = RollForwardInTimeTo(marker);
                RaiseOnValueChanged();
            }
        }

        /// <summary>
        /// Moves the history forward enough steps to make the next future time is equal to or larger than the
        /// provided mark.
        /// </summary>
        /// <param name="mark">The mark that indicates to which point in time the roll-forward should be performed.</param>
        /// <returns>
        ///     The last recorded value.
        /// </returns>
        private T RollForwardInTimeTo(TimeMarker mark)
        {
            {
                Debug.Assert(!IsAtEndOfTime(), "Should not be at the end of time.");
            }

            // We assume that it is more likely that we have a more recent
            // point in time that we want to roll forward to. So start searching
            // from the start of the collection
            while ((m_FutureValues.First != null) && (m_FutureValues.First.Value.Time <= mark))
            {
                var lastNode = m_FutureValues.First;
                m_FutureValues.RemoveFirst();
                m_PastValues.AddLast(lastNode);
            }

            return m_PastValues.Last.Value.Value;
        }

        /// <summary>
        /// Stores the current value in the history list with the given marker.
        /// </summary>
        /// <param name="marker">The marker which indicates at which point on the timeline the data is stored.</param>
        /// <exception cref="CannotStoreValuesAtTheStartOfTimeException">
        ///     Thrown when <paramref name="marker"/> is equal to <see cref="TimeMarker.TheBeginOfTime"/>.
        /// </exception>
        public void StoreCurrent(TimeMarker marker)
        {
            {
                Lokad.Enforce.With<CannotStoreValuesAtTheStartOfTimeException>(
                    marker > TimeMarker.TheBeginOfTime,
                    Resources.Exceptions_Messages_CannotStoreValuesAtTheStartOfTime);
            }

            if (IsLastValueDifferent())
            {
                m_PastValues.AddLast(new LinkedListNode<ValueAtTime<T>>(new ValueAtTime<T>(marker, m_Current)));
                ForgetTheFuture();
            }
        }

        private bool IsLastValueDifferent()
        {
            if (IsAtBeginOfTime())
            {
                return true;
            }

            var lastValue = m_PastValues.Last.Value.Value;
            if (ReferenceEquals(lastValue, null))
            {
                return !ReferenceEquals(m_Current, null);
            }
            else 
            {
                return !lastValue.Equals(m_Current);
            }
        }

        /// <summary>
        /// An event that is raised if a roll-back or roll-forward has taken place.
        /// </summary>
        public event EventHandler<EventArgs> OnValueChanged;

        private void RaiseOnValueChanged()
        {
            var local = OnValueChanged;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Clears all the history storage and forgets all the 
        /// stored historic information.
        /// </summary>
        public void ForgetAllHistory()
        {
            ForgetThePast();
            ForgetTheFuture();

            m_Current = default(T);
        }

        private void ForgetThePast()
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

        /// <summary>
        /// Gets or sets the current value for the variable.
        /// </summary>
        /// <remarks>
        /// Note that setting the current value does not directly store 
        /// this value in the timeline. Values are only stored in the 
        /// timeline when the <see cref="IStoreTimelineValues.StoreCurrent(TimeMarker)"/>
        /// method is invoked.
        /// </remarks>
        protected T CurrentInternalValue
        {
            [DebuggerStepThrough]
            get
            {
                return m_Current;
            }

            [DebuggerStepThrough]
            set
            {
                m_Current = value;
            }
        }
    }
}
