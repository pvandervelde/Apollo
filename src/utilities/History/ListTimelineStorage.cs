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
    internal abstract partial class ListTimelineStorage<TExternal, TStorage> : IListTimelineStorage<TExternal>, IStoreTimelineValues
    {
        /// <summary>
        /// The number of time markers between snapshots.
        /// </summary>
        private const int SnapshotInterval = 20;

        /// <summary>
        /// The collection that contains the past snapshots of the current collection at different 
        /// past points in time.
        /// </summary>
        private readonly LinkedList<ValueAtTime<List<TStorage>>> m_PastSnapshots
            = new LinkedList<ValueAtTime<List<TStorage>>>();

        /// <summary>
        /// The collection that contains the future snapshots of the current collection at different
        /// future points in time.
        /// </summary>
        private readonly LinkedList<ValueAtTime<List<TStorage>>> m_FutureSnapshots
            = new LinkedList<ValueAtTime<List<TStorage>>>();

        /// <summary>
        /// The collection that contains the values that are in the past.
        /// </summary>
        private readonly LinkedList<ValueAtTime<List<ICollectionChange<TStorage>>>> m_PastValues
            = new LinkedList<ValueAtTime<List<ICollectionChange<TStorage>>>>();

        /// <summary>
        /// The collection that contains the values that are in the future.
        /// </summary>
        private readonly LinkedList<ValueAtTime<List<ICollectionChange<TStorage>>>> m_FutureValues
            = new LinkedList<ValueAtTime<List<ICollectionChange<TStorage>>>>();

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
        /// Initializes a new instance of the <see cref="ListTimelineStorage{TExternal, TStorage}"/> class.
        /// </summary>
        /// <param name="externalToStorage">The function that maps an external object to a storage object.</param>
        /// <param name="storageToExternal">The function that maps a storage object to an external object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="externalToStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storageToExternal"/> is <see langword="null" />.
        /// </exception>
        protected ListTimelineStorage(
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
            return (m_PastValues.Count == 0) && (m_PastSnapshots.Count == 0);
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

            var node = m_PastSnapshots.First;
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
            return (m_FutureValues.Count == 0) && (m_FutureSnapshots.Count == 0);
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
                    RollBackInTimeTo(marker);
                }
                else
                {
                    RollBackToBeginning();
                }

                RaiseOnValueChanged();
            }
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
            var snapshotNode = m_PastSnapshots.Last;
            while ((snapshotNode != null) && (snapshotNode.Value.Time > mark))
            {
                m_PastSnapshots.RemoveLast();
                m_FutureSnapshots.AddFirst(snapshotNode);

                snapshotNode = m_PastSnapshots.Last;
            }

            // Find the nodes that we need to apply to the snap shot to get to the desired state
            var lastRollBackNode = m_PastValues.Last;
            while ((lastRollBackNode != null) && (lastRollBackNode.Value.Time > mark))
            {
                m_PastValues.RemoveLast();
                m_FutureValues.AddFirst(lastRollBackNode);

                lastRollBackNode = m_PastValues.Last;
            }

            // Roll the snap-shot over the current values
            var current = new List<TStorage>();
            if (snapshotNode != null)
            {
                current.AddRange(snapshotNode.Value.Value);
                if ((lastRollBackNode != null) && (snapshotNode.Value.Time < lastRollBackNode.Value.Time))
                {
                    var firstRollBackNode = lastRollBackNode;
                    while ((firstRollBackNode.Previous != null) && (firstRollBackNode.Value.Time > snapshotNode.Value.Time))
                    {
                        firstRollBackNode = firstRollBackNode.Previous;
                    }

                    // We overshot by 1 node so ...
                    if (firstRollBackNode.Value.Time < snapshotNode.Value.Time)
                    {
                        firstRollBackNode = firstRollBackNode.Next;
                    }

                    // Roll-back the changes
                    var rollbackNode = firstRollBackNode;
                    while (rollbackNode != lastRollBackNode.Next)
                    {
                        var changeset = rollbackNode.Value.Value;
                        ApplyChangeSet(current, changeset);

                        rollbackNode = rollbackNode.Next;
                    }

                    // Move the last changes to the futures collection
                    var movingNode = m_PastValues.Last;
                    while (movingNode != lastRollBackNode)
                    {
                        m_PastValues.RemoveLast();
                        m_FutureValues.AddFirst(movingNode);

                        movingNode = m_PastValues.Last;
                    }
                }
            }

            m_Current = current;
        }

        private void ApplyChangeSet(List<TStorage> current, List<ICollectionChange<TStorage>> changeset)
        {
            foreach (var change in changeset)
            {
                change.ApplyTo(current);
            }
        }

        /// <summary>
        /// Moves the history back enough steps to make the last known time is equal to the first recorded value
        /// in time.
        /// </summary>
        private void RollBackToBeginning()
        {
            {
                Debug.Assert(!IsAtBeginOfTime(), "Cannot roll-back to the beginning of time if without data stored.");
            }

            RollBackInTimeTo(TimeMarker.TheBeginOfTime);
        }

        /// <summary>
        /// Rolls the current value back to the start point.
        /// </summary>
        public void RollBackToStart()
        {
            if (!IsAtBeginOfTime())
            {
                RollBackToBeginning();
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
                RollForwardInTimeTo(marker);
                RaiseOnValueChanged();
            }
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

            // Find the last snapshot that we need to move
            var snapshotNode = m_FutureSnapshots.First;
            while ((snapshotNode != null) && (snapshotNode.Value.Time <= mark))
            {
                m_FutureSnapshots.RemoveFirst();
                m_PastSnapshots.AddLast(snapshotNode);

                snapshotNode = m_FutureSnapshots.First;
            }

            // Move all the nodes
            var lastRollBackNode = m_FutureValues.First;
            while ((lastRollBackNode != null) && (lastRollBackNode.Value.Time <= mark))
            {
                m_FutureValues.RemoveFirst();
                m_PastValues.AddLast(lastRollBackNode);

                lastRollBackNode = m_FutureValues.First;
            }

            // Reset the values to the last past values because we just pushed all the
            // desired data back on to the past collections.
            snapshotNode = m_PastSnapshots.Last;
            lastRollBackNode = m_PastValues.Last;

            // Roll the whole thing back
            var current = new List<TStorage>(snapshotNode.Value.Value);
            if ((lastRollBackNode != null) && (snapshotNode.Value.Time < lastRollBackNode.Value.Time))
            {
                var firstRollBackNode = lastRollBackNode;
                while ((firstRollBackNode.Previous != null) && (firstRollBackNode.Value.Time > snapshotNode.Value.Time))
                {
                    firstRollBackNode = firstRollBackNode.Previous;
                }

                // We overshot by 1 node so ...
                if (firstRollBackNode.Value.Time < snapshotNode.Value.Time)
                {
                    firstRollBackNode = firstRollBackNode.Next;
                }

                // update the new collection
                var rollbackNode = firstRollBackNode;
                while (rollbackNode != lastRollBackNode.Next)
                {
                    var changeset = rollbackNode.Value.Value;
                    ApplyChangeSet(current, changeset);

                    rollbackNode = rollbackNode.Next;
                }
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
                m_PastValues.AddLast(
                    new LinkedListNode<ValueAtTime<List<ICollectionChange<TStorage>>>>(
                        new ValueAtTime<List<ICollectionChange<TStorage>>>(
                            marker,
                            m_Changes)));
            }

            m_Changes = new List<ICollectionChange<TStorage>>();
            ForgetTheFuture();
        }

        private bool ShouldSnapshot()
        {
            if (m_PastSnapshots.Count == 0)
            {
                return true;
            }

            Debug.Assert(m_PastSnapshots.Count > 0, "We should always have at least one snapshot.");
            var lastSnapshotTime = m_PastSnapshots.Last.Value.Time;

            int count = 0;
            var node = m_PastValues.Last;
            while ((node != null) && (lastSnapshotTime < node.Value.Time))
            {
                count++;
                node = node.Previous;
            }

            return SnapshotInterval <= count;
        }

        private void CreateSnapshot(TimeMarker marker)
        {
            var snapshot = new List<TStorage>(m_Current);
            m_PastSnapshots.AddLast(
                new LinkedListNode<ValueAtTime<List<TStorage>>>(
                    new ValueAtTime<List<TStorage>>(
                        marker,
                        snapshot)));
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

            m_Current = new List<TStorage>();
            m_Changes = new List<ICollectionChange<TStorage>>();
        }

        private void ForgetThePast()
        {
            m_PastValues.Clear();
            m_PastSnapshots.Clear();
        }

        /// <summary>
        /// Clears all the history information that is in the future.
        /// </summary>
        public void ForgetTheFuture()
        {
            m_FutureValues.Clear();
            m_FutureSnapshots.Clear();
        }
    }
}
