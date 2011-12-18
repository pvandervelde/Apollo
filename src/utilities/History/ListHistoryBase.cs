//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an <see cref="IList{T}"/> collection of objects of type <typeparamref name="TStorage"/> but
    /// provides them to the outside world as objects of type <typeparamref name="TExternal"/>.
    /// </summary>
    /// <typeparam name="TExternal">The type of object that is passed into the collection.</typeparam>
    /// <typeparam name="TStorage">The type of object for which the values are stored.</typeparam>
    internal abstract partial class ListHistoryBase<TExternal, TStorage> : IListTimelineStorage<TExternal>, IStoreTimelineValues
    {
        /// <summary>
        /// The number of time markers between snapshots.
        /// </summary>
        private const int SnapshotInterval = 20;

        private static void ApplyChangeSet(List<TStorage> current, List<ICollectionChange<TStorage>> changeset)
        {
            foreach (var change in changeset)
            {
                change.ApplyTo(current);
            }
        }

        /// <summary>
        /// The past and future snapshots.
        /// </summary>
        private readonly ValueAtTimeStorage<List<TStorage>> m_SnapshotHistory 
            = new ValueAtTimeStorage<List<TStorage>>();

        /// <summary>
        /// The past and future changes.
        /// </summary>
        private readonly ValueAtTimeStorage<List<ICollectionChange<TStorage>>> m_ValueHistory 
            = new ValueAtTimeStorage<List<ICollectionChange<TStorage>>>();

        /// <summary>
        /// The function that maps an external object to a storage object.
        /// </summary>
        private readonly Func<TExternal, TStorage> m_ExternalToStorage;

        /// <summary>
        /// The function that maps a storage object to an external object.
        /// </summary>
        private readonly Func<TStorage, TExternal> m_StorageToExternal;

        /// <summary>
        /// The collection of current items.
        /// </summary>
        /// <design>
        /// This collection pointer is not readonly because when we roll-back or roll-forward
        /// we simply replace the collection pointer.
        /// </design>
        private List<TStorage> m_Current = new List<TStorage>();

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
        private List<ICollectionChange<TStorage>> m_Changes
            = new List<ICollectionChange<TStorage>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListHistoryBase{TExternal, TStorage}"/> class.
        /// </summary>
        /// <param name="externalToStorage">The function that maps an external object to a storage object.</param>
        /// <param name="storageToExternal">The function that maps a storage object to an external object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="externalToStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storageToExternal"/> is <see langword="null" />.
        /// </exception>
        protected ListHistoryBase(
            Func<TExternal, TStorage> externalToStorage,
            Func<TStorage, TExternal> storageToExternal)
        {
            {
                Lokad.Enforce.Argument(() => externalToStorage);
                Lokad.Enforce.Argument(() => storageToExternal);
            }

            m_ExternalToStorage = externalToStorage;
            m_StorageToExternal = storageToExternal;
        }

        /// <summary>
        /// Adds an item to the <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="IList{T}"/>.</param>
        public void Add(TExternal item)
        {
            var storageItem = m_ExternalToStorage(item);
            m_Current.Add(storageItem);
            m_Changes.Add(new InsertIntoListChange<TStorage>(m_Current.Count - 1, storageItem));
        }

        /// <summary>
        /// Removes all items from the <see cref="IList{T}"/>.
        /// </summary>
        public void Clear()
        {
            m_Current.Clear();

            m_Changes.Clear();
            m_Changes.Add(new ClearListChange<TStorage>());
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="IList{T}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is not a valid index in the <see cref="IList{T}"/>.</exception>
        public void Insert(int index, TExternal item)
        {
            var storageItem = m_ExternalToStorage(item);
            m_Current.Insert(index, storageItem);
            m_Changes.Add(new InsertIntoListChange<TStorage>(index, storageItem));
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="IList{T}"/>.</param>
        /// <returns>
        ///     <see langword="true"/> if item was successfully removed from the <see cref="IList{T}"/>;
        ///     otherwise, <see langword="false"/>. This method also returns false if item is not found in
        ///     the original <see cref="IList{T}"/>.
        /// </returns>
        public bool Remove(TExternal item)
        {
            var storageItem = m_ExternalToStorage(item);
            var index = m_Current.IndexOf(storageItem);
            if (index > -1)
            {
                RemoveAt(index);
            }

            return index != -1;
        }

        /// <summary>
        /// Removes the <see cref="IList{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is not a valid index in the <see cref="IList{T}"/>.</exception>
        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            m_Current.RemoveAt(index);
            m_Changes.Add(new RemoveFromListChange<TStorage>(index));
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is not a valid index in the <see cref="IList{T}"/>.</exception>
        public TExternal this[int index]
        {
            get
            {
                return m_StorageToExternal(m_Current[index]);
            }

            set
            {
                var storageItem = m_ExternalToStorage(value);
                m_Current[index] = storageItem;
                m_Changes.Add(new ItemUpdatedChange<TStorage>(index, storageItem));
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList{T}"/>.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(TExternal item)
        {
            return m_Current.IndexOf(m_ExternalToStorage(item));
        }

        /// <summary>
        /// Determines whether the <see cref="IList{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList{T}"/>.</param>
        /// <returns>
        ///     <see langword="true"/> if item is found in the <see cref="IList{T}"/>; otherwise,
        ///     <see langword="false"/>.
        /// </returns>
        public bool Contains(TExternal item)
        {
            return m_Current.Contains(m_ExternalToStorage(item));
        }

        /// <summary>
        /// Copies the elements of the <see cref="IList{T}"/> to an
        /// <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="System.Array"/> that is the destination of the elements
        /// copied from <see cref="IList{T}"/>. The <see cref="System.Array"/> must
        /// have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when the <paramref name="array"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when the arrayIndex is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when the number of elements in the source <see cref="IList{T}"/>
        ///     is greater than the available space from arrayIndex to the end of the destination array.
        /// </exception>
        public void CopyTo(TExternal[] array, int arrayIndex)
        {
            {
                Lokad.Enforce.Argument(() => array);
                Lokad.Enforce.With<ArgumentOutOfRangeException>(
                    arrayIndex >= 0, 
                    Resources.Exceptions_Messages_ArgumentOutOfRange_WithArgument, 
                    arrayIndex);

                Lokad.Enforce.With<ArgumentException>(
                    m_Current.Count <= (array.Length - arrayIndex),
                    Resources.Exceptions_Messages_TheArrayIsShorterThanTheNumberOfItems_WithValues,
                    array.Length - arrayIndex,
                    m_Current.Count);
            }

            for (int i = 0, j = arrayIndex; i < m_Current.Count; i++, j++)
            {
                array[j] = m_StorageToExternal(m_Current[i]);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IList{T}"/>.
        /// </summary>
        public int Count
        {
            get 
            {
                return m_Current.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IList{T}"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get 
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TExternal> GetEnumerator()
        {
            foreach (var item in m_Current)
            {
                yield return m_StorageToExternal(item);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator" /> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            RaiseOnValueChanged();
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
            var current = (snapshot != null) ? new List<TStorage>(snapshot) : new List<TStorage>();
            if (!m_ValueHistory.IsAtEndOfTime())
            {
                m_ValueHistory.RollForwardTo(
                    mark,
                    (t, v) =>
                    {
                        ApplyChangeSet(current, v);
                    });
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
                RaiseOnValueChanged();
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
            RaiseOnValueChanged();
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
            List<TStorage> current = null;
            if (snapshotTime >= m_ValueHistory.LastTime)
            {
                if (!m_ValueHistory.IsAtEndOfTime())
                {
                    m_ValueHistory.RollForwardTo(snapshotTime);
                }

                current = (snapshot != null) ? new List<TStorage>(snapshot) : new List<TStorage>();
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

            m_Current = current;
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

            m_Changes = new List<ICollectionChange<TStorage>>();
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
            var snapshot = new List<TStorage>(m_Current);
            m_SnapshotHistory.StoreCurrent(marker, snapshot);
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
            m_ValueHistory.ForgetAllHistory();
            m_SnapshotHistory.ForgetAllHistory();

            m_Current = new List<TStorage>();
            m_Changes = new List<ICollectionChange<TStorage>>();
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
