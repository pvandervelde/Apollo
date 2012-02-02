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
    /// The base class for all classes that store history values and require snapshots to be
    /// made at regular intervals.
    /// </summary>
    /// <typeparam name="T">The type of object stored by the storage.</typeparam>
    public abstract class HistorySnapshotStorage<T> : IStoreTimelineValues where T : class
    {
        /// <summary>
        /// The number of time markers between snapshots.
        /// </summary>
        private const int SnapshotInterval = 20;

        private static void ApplyChangeSet(T current, List<IHistoryChange<T>> changeset)
        {
            foreach (var change in changeset)
            {
                change.ApplyTo(current);
            }
        }

        /// <summary>
        /// The past and future snapshots.
        /// </summary>
        private readonly ValueAtTimeStorage<T> m_SnapshotHistory
            = new ValueAtTimeStorage<T>();

        /// <summary>
        /// The past and future changes.
        /// </summary>
        private readonly ValueAtTimeStorage<List<IHistoryChange<T>>> m_ValueHistory
            = new ValueAtTimeStorage<List<IHistoryChange<T>>>();

        /// <summary>
        /// The function that creates a new container, possibly accepting an old
        /// container object to clone.
        /// </summary>
        private readonly Func<T, T> m_ContainerBuilder;

        /// <summary>
        /// The collection of current items.
        /// </summary>
        /// <design>
        /// This pointer is not readonly because when we roll-back or roll-forward
        /// we simply replace the pointer.
        /// </design>
        private T m_Current;

        /// <summary>
        /// The collection that tracks all the changes that have taken place since the 
        /// last time rolled-back, rolled-forward or stored the current values.
        /// </summary>
        /// <design>
        /// This collection pointer is not readonly because we push this collection to the
        /// <c>m_PastValues</c> collection once the <see cref="StoreCurrent"/> method is called.
        /// After that we simply replace the current collection with a new one which is more efficient
        /// than copying the collection and then clearing it.
        /// </design>
        private List<IHistoryChange<T>> m_Changes
            = new List<IHistoryChange<T>>();

        /// <summary>
        /// A flag that indicates if the current value has changed.
        /// </summary>
        private bool m_HasChanged = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistorySnapshotStorage{T}"/> class.
        /// </summary>
        /// <param name="containerBuilder">
        /// The function that creates a new container, possibly based on an existing container.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="containerBuilder"/> is <see langword="null" />.
        /// </exception>
        protected HistorySnapshotStorage(Func<T, T> containerBuilder)
        {
            {
                Lokad.Enforce.Argument(() => containerBuilder);
            }

            m_ContainerBuilder = containerBuilder;
            m_Current = m_ContainerBuilder(null);

            m_SnapshotHistory.OnCurrentValueChange += (s, e) => m_HasChanged = true;
            m_ValueHistory.OnCurrentValueChange += (s, e) => m_HasChanged = true;
        }

        /// <summary>
        /// A method called when the current value is changed externally, e.g. through a
        /// roll-back or roll-forward.
        /// </summary>
        private void IndicateExternalChangeToCurrentValue()
        {
            var local = OnExternalValueUpdate;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the the stored value is changed externally.
        /// </summary>
        public event EventHandler<EventArgs> OnExternalValueUpdate;

        /// <summary>
        /// Gets the current value.
        /// </summary>
        protected T Current
        {
            get
            {
                return m_Current;
            }
        }

        /// <summary>
        /// Gets the collection of changes that have occurred since the last 
        /// time the changes were stored.
        /// </summary>
        protected List<IHistoryChange<T>> Changes
        {
            get
            {
                return m_Changes;
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
            return m_ValueHistory.IsAtBeginOfTime() && m_SnapshotHistory.IsAtBeginOfTime();
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
            return m_SnapshotHistory.WouldRollBackPastTheBeginningOfTime(mark);
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
            return m_ValueHistory.IsAtEndOfTime() && m_SnapshotHistory.IsAtEndOfTime();
        }

        /// <summary>
        /// Rolls the current value back to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollBackTo(TimeMarker marker)
        {
            if (IsAtBeginOfTime())
            {
                return;
            }

            RollBackInTimeTo(marker);
        }

        /// <summary>
        /// Moves the history back enough steps to make the last known time is equal to or smaller than the
        /// provided mark.
        /// </summary>
        /// <param name="mark">The mark that indicates to which point in time the roll-back should be performed.</param>
        private void RollBackInTimeTo(TimeMarker mark)
        {
            {
                Debug.Assert(!IsAtBeginOfTime() && !WouldRollBackPastTheBeginningOfTime(mark), "Should not roll back past the beginning of time.");
            }

            // Find the latest snapshot that we can use as base for the roll-back
            var snapshot = !m_SnapshotHistory.IsAtBeginOfTime() ? m_SnapshotHistory.RollBackTo(mark) : m_SnapshotHistory.LastValue;
            var snapshotTime = m_SnapshotHistory.LastTime;

            // Find the nodes that we need to apply to the snap shot to get to the desired state
            if (!m_ValueHistory.IsAtBeginOfTime())
            {
                m_ValueHistory.RollBackTo(snapshotTime);
            }

            // Roll the snap-shot over the current values
            var current = m_ContainerBuilder(snapshot);
            if (!m_ValueHistory.IsAtEndOfTime())
            {
                m_ValueHistory.RollForwardTo(
                    mark,
                    (t, v) =>
                    {
                        ApplyChangeSet(current, v);
                    });
            }

            if (m_HasChanged)
            {
                IndicateExternalChangeToCurrentValue();
                m_HasChanged = false;
            }

            m_Current = current;
        }

        /// <summary>
        /// Rolls the current value back to the start point.
        /// </summary>
        public void RollBackToStart()
        {
            if (!IsAtBeginOfTime())
            {
                RollBackInTimeTo(TimeMarker.TheBeginOfTime);
            }
        }

        /// <summary>
        /// Rolls the current value forward to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollForwardTo(TimeMarker marker)
        {
            if (IsAtEndOfTime())
            {
                return;
            }

            RollForwardInTimeTo(marker);
        }

        /// <summary>
        /// Moves the history forward enough steps to make the next future time is equal to or larger than the
        /// provided mark.
        /// </summary>
        /// <param name="mark">The mark that indicates to which point in time the roll-forward should be performed.</param>
        private void RollForwardInTimeTo(TimeMarker mark)
        {
            {
                Debug.Assert(!IsAtEndOfTime(), "Should not be at the end of time.");
            }

            // Find the latest snapshot that we can use as base for the roll-back
            var snapshot = !m_SnapshotHistory.IsAtEndOfTime() ? m_SnapshotHistory.RollForwardTo(mark) : m_SnapshotHistory.LastValue;
            var snapshotTime = m_SnapshotHistory.LastTime;

            // Roll forward to where we want to go first
            T current = null;
            if (snapshotTime >= m_ValueHistory.LastTime)
            {
                if (!m_ValueHistory.IsAtEndOfTime())
                {
                    m_ValueHistory.RollForwardTo(snapshotTime);
                }

                current = m_ContainerBuilder(snapshot);
            }
            else
            {
                current = m_Current;
            }

            // Roll the snap-shot over the current values
            if (!m_ValueHistory.IsAtEndOfTime())
            {
                m_ValueHistory.RollForwardTo(
                    mark,
                    (t, v) =>
                    {
                        ApplyChangeSet(current, v);
                    });
            }

            if (m_HasChanged)
            {
                IndicateExternalChangeToCurrentValue();
                m_HasChanged = false;
            }

            m_Current = current;
        }

        /// <summary>
        /// Stores the current value as the default value which will be returned if there are no values stored.
        /// </summary>
        /// <exception cref="CannotSetDefaultAfterStartOfTimeException">
        /// Thrown when the user tries to store the default value after one or more value have been stored.
        /// </exception>
        public void StoreCurrentAsDefault()
        {
            {
                Lokad.Enforce.With<CannotSetDefaultAfterStartOfTimeException>(
                    m_SnapshotHistory.LastValue == null, 
                    Resources.Exceptions_Messages_CannotSetDefaultAfterStartOfTime);
            }

            var snapshot = m_ContainerBuilder(m_Current);
            m_SnapshotHistory.StoreCurrent(TimeMarker.TheBeginOfTime, snapshot);
            m_Changes = new List<IHistoryChange<T>>();
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

            if (m_Changes.Count == 0)
            {
                return;
            }

            if (ShouldSnapshot())
            {
                CreateSnapshot(marker);
            }
            else
            {
                m_ValueHistory.StoreCurrent(marker, m_Changes);
            }

            m_Changes = new List<IHistoryChange<T>>();
            ForgetTheFuture();
        }

        private bool ShouldSnapshot()
        {
            if (m_SnapshotHistory.LastValue == null)
            {
                return true;
            }

            Debug.Assert(m_SnapshotHistory.LastValue != null, "We should always have at least one snapshot.");
            var lastSnapshotTime = m_SnapshotHistory.LastTime;

            int count = 0;
            m_ValueHistory.TrackBackwardsInTime(
                (t, v) =>
                {
                    count++;
                    return lastSnapshotTime < t;
                });

            return SnapshotInterval <= count;
        }

        private void CreateSnapshot(TimeMarker marker)
        {
            // We're creating a copy here so that we can keep the current 
            // collection as current. One way or the other we'll need to make
            // a copy anyway so might as well do it here.
            var snapshot = m_ContainerBuilder(m_Current);
            m_SnapshotHistory.StoreCurrent(marker, snapshot);
        }

        /// <summary>
        /// Clears all the history storage and forgets all the 
        /// stored historic information.
        /// </summary>
        public void ForgetAllHistory()
        {
            m_ValueHistory.ForgetAllHistory();
            m_SnapshotHistory.ForgetAllHistory();

            m_Current = m_ContainerBuilder(null);
            m_Changes = new List<IHistoryChange<T>>();
        }

        /// <summary>
        /// Clears all the history information that is in the future.
        /// </summary>
        public void ForgetTheFuture()
        {
            m_ValueHistory.ForgetTheFuture();
            m_SnapshotHistory.ForgetTheFuture();
        }
    }
}
