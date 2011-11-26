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
using System.Linq;
using Apollo.Utilities.Properties;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an <see cref="IDictionary{TKey, TExternalValue}"/> collection of 
    /// objects of type <typeparamref name="TExternal"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of object that is used as key in the dictionary.</typeparam>
    /// <typeparam name="TExternal">The type of object that is passed into the collection.</typeparam>
    /// <typeparam name="TStorage">The type of object for which the values are stored.</typeparam>
    internal abstract partial class DictionaryTimelineStorage<TKey, TExternal, TStorage> : IDictionaryTimelineStorage<TKey, TExternal>
    {
        /// <summary>
        /// The number of time markers between snapshots.
        /// </summary>
        private const int SnapshotInterval = 20;

        /// <summary>
        /// The collection that contains the past snapshots of the current collection at different 
        /// past points in time.
        /// </summary>
        private readonly LinkedList<ValueAtTime<IDictionary<TKey, TStorage>>> m_PastSnapshots
            = new LinkedList<ValueAtTime<IDictionary<TKey, TStorage>>>();

        /// <summary>
        /// The collection that contains the future snapshots of the current collection at different
        /// future points in time.
        /// </summary>
        private readonly LinkedList<ValueAtTime<IDictionary<TKey, TStorage>>> m_FutureSnapshots
            = new LinkedList<ValueAtTime<IDictionary<TKey, TStorage>>>();

        /// <summary>
        /// The collection that contains the values that are in the past.
        /// </summary>
        private readonly LinkedList<ValueAtTime<List<ICollectionChange<KeyValuePair<TKey, TStorage>>>>> m_PastValues
            = new LinkedList<ValueAtTime<List<ICollectionChange<KeyValuePair<TKey, TStorage>>>>>();

        /// <summary>
        /// The collection that contains the values that are in the future.
        /// </summary>
        private readonly LinkedList<ValueAtTime<List<ICollectionChange<KeyValuePair<TKey, TStorage>>>>> m_FutureValues
            = new LinkedList<ValueAtTime<List<ICollectionChange<KeyValuePair<TKey, TStorage>>>>>();

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
        private Dictionary<TKey, TStorage> m_Current = new Dictionary<TKey, TStorage>();

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
        private List<ICollectionChange<KeyValuePair<TKey, TStorage>>> m_Changes
            = new List<ICollectionChange<KeyValuePair<TKey, TStorage>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryTimelineStorage{TKey, TExternal, TStorage}"/> class.
        /// </summary>
        /// <param name="externalToStorage">The function that maps an external object to a storage object.</param>
        /// <param name="storageToExternal">The function that maps a storage object to an external object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="externalToStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storageToExternal"/> is <see langword="null" />.
        /// </exception>
        protected DictionaryTimelineStorage(
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
        /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="key"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if an element with the same key already exists in the <see cref="IDictionary{TKey,TValue}" />.
        /// </exception>
        public void Add(TKey key, TExternal value)
        {
            var storageItem = m_ExternalToStorage(value);
            m_Current.Add(key, storageItem);
            m_Changes.Add(new AddToDictionaryChange<TKey, TStorage>(key, storageItem));
        }

        /// <summary>
        /// Adds an item to the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="IDictionary{TKey,TValue}" />.</param>
        void ICollection<KeyValuePair<TKey, TExternal>>.Add(KeyValuePair<TKey, TExternal> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all items from the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public void Clear()
        {
            m_Current.Clear();

            m_Changes.Clear();
            m_Changes.Add(new ClearDictionaryChange<TKey, TStorage>());
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        ///     <see langword="true"/> if item was successfully removed from the <see cref="IDictionary{TKey,TValue}" />;
        ///     otherwise, <see langword="false"/>. This method also returns false if item is not found in
        ///     the original <see cref="IDictionary{TKey,TValue}" />.
        /// </returns>
        public bool Remove(TKey key)
        {
            var result = ContainsKey(key);
            if (result)
            {
                result = result && m_Current.Remove(key);
                m_Changes.Add(new RemoveFromDictionaryChange<TKey, TStorage>(key));
            }

            return result;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="IDictionary{TKey,TValue}" />.</param>
        /// <returns>
        ///     <see langword="true"/> if item was successfully removed from the <see cref="IDictionary{TKey,TValue}" />;
        ///     otherwise, <see langword="false"/>. This method also returns false if item is not found in
        ///     the original <see cref="IDictionary{TKey,TValue}" />.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TExternal>>.Remove(KeyValuePair<TKey, TExternal> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="key"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        ///     Thrown if the specified <paramref name="key"/> is not found.
        /// </exception>
        public TExternal this[TKey key]
        {
            get
            {
                return m_StorageToExternal(m_Current[key]);
            }

            set
            {
                var storageItem = m_ExternalToStorage(value);
                m_Current[key] = storageItem;
                m_Changes.Add(new ItemUpdatedChange<TKey, TStorage>(key, storageItem));
            }
        }

        /// <summary>
        /// Gets a readonly <see cref="ICollection{T}" /> containing the keys of the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                return new List<TKey>(m_Current.Keys);
            }
        }

        /// <summary>
        /// Gets a readonly <see cref="ICollection{T}" /> containing the values of the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public ICollection<TExternal> Values
        {
            get
            {
                return (from storedValue in m_Current.Values select m_StorageToExternal(storedValue)).ToList();
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if 
        /// the key is found; otherwise, the default value for the type of the value
        /// parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the object that implements <see cref="IDictionary{TKey,TValue}" />
        /// contains an element with the specified key; otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetValue(TKey key, out TExternal value)
        {
            if (!ContainsKey(key))
            {
                value = default(TExternal);
                return false;
            }

            TStorage storedValue;
            var result = m_Current.TryGetValue(key, out storedValue);
            value = result ? m_StorageToExternal(storedValue) : default(TExternal);

            return result;
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey,TValue}" /> 
        /// contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IDictionary{TKey,TValue}" />.</param>
        /// <returns>
        ///     <see langword="true" /> if the <see cref="IDictionary{TKey,TValue}" /> contains
        ///     an element with the key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            if (ReferenceEquals(null, key))
            {
                return false;
            }

            return m_Current.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey,TValue}" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IDictionary{TKey,TValue}" />.</param>
        /// <returns>
        ///     <see langword="true"/> if item is found in the <see cref="IDictionary{TKey,TValue}" />; otherwise,
        ///     <see langword="false"/>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TExternal>>.Contains(KeyValuePair<TKey, TExternal> item)
        {
            return ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="IDictionary{TKey,TValue}" /> to an
        /// <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="System.Array"/> that is the destination of the elements
        /// copied from <see cref="IDictionary{TKey,TValue}" />. The <see cref="System.Array"/> must
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
        ///     Thrown when the number of elements in the source <see cref="IDictionary{TKey,TValue}" />
        ///     is greater than the available space from arrayIndex to the end of the destination array.
        /// </exception>
        void ICollection<KeyValuePair<TKey, TExternal>>.CopyTo(KeyValuePair<TKey, TExternal>[] array, int arrayIndex)
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

            int j = arrayIndex;
            foreach (var pair in m_Current)
            {
                array[j] = new KeyValuePair<TKey, TExternal>(pair.Key, m_StorageToExternal(pair.Value));
                j++;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public int Count
        {
            get
            {
                return m_Current.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary{TKey,TValue}" /> is read-only.
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
        /// A enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TExternal>> GetEnumerator()
        {
            foreach (var pair in m_Current)
            {
                yield return new KeyValuePair<TKey, TExternal>(pair.Key, m_StorageToExternal(pair.Value));
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
        private bool IsAtBeginOfTime()
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
        private bool IsAtEndOfTime()
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
            while (snapshotNode.Value.Time > mark)
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
            var current = new Dictionary<TKey, TStorage>(snapshotNode.Value.Value);
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

                // Store the stopping node because as soon as we move the firstRollBackNode (which
                // is the last node we move) then accesssing the 'Previous' will return a different
                // node.
                var stoppingNode = firstRollBackNode.Previous;

                // Move the last changes to the futures collection
                var movingNode = m_PastValues.Last;
                while (movingNode != stoppingNode)
                {
                    m_PastValues.RemoveLast();
                    m_FutureValues.AddFirst(movingNode);

                    movingNode = m_PastValues.Last;
                }
            }

            m_Current = current;
        }

        private void ApplyChangeSet(Dictionary<TKey, TStorage> current, List<ICollectionChange<KeyValuePair<TKey, TStorage>>> changeset)
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

            var mark = m_PastSnapshots.First.Value.Time;
            RollBackInTimeTo(mark);
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
            var current = new Dictionary<TKey, TStorage>(snapshotNode.Value.Value);
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
        public void StoreCurrent(TimeMarker marker)
        {
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
                    new LinkedListNode<ValueAtTime<List<ICollectionChange<KeyValuePair<TKey, TStorage>>>>>(
                        new ValueAtTime<List<ICollectionChange<KeyValuePair<TKey, TStorage>>>>(
                            marker,
                            m_Changes)));
            }

            m_Changes = new List<ICollectionChange<KeyValuePair<TKey, TStorage>>>();
            if (m_FutureValues.Count > 0)
            {
                m_FutureValues.Clear();
            }

            if (m_FutureSnapshots.Count > 0)
            {
                m_FutureSnapshots.Clear();
            }
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
            var snapshot = new Dictionary<TKey, TStorage>(m_Current);
            m_PastSnapshots.AddLast(
                new LinkedListNode<ValueAtTime<IDictionary<TKey, TStorage>>>(
                    new ValueAtTime<IDictionary<TKey, TStorage>>(
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
    }
}
