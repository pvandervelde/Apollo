//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Stores the timeline values for an <see cref="IDictionary{TKey, TExternalValue}"/> collection of 
    /// objects of type <typeparamref name="TExternalValue"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of object that is used as key in the dictionary.</typeparam>
    /// <typeparam name="TExternalValue">The type of object that is passed into the collection.</typeparam>
    /// <typeparam name="TStorageValue">The type of object for which the values are stored.</typeparam>
    internal abstract class DictionaryTimelineStorage<TKey, TExternalValue, TStorageValue> : IDictionaryTimelineStorage<TKey, TExternalValue>
    {
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
        public void Add(TKey key, TExternalValue value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds an item to the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="IDictionary{TKey,TValue}" />.</param>
        void ICollection<KeyValuePair<TKey, TExternalValue>>.Add(KeyValuePair<TKey, TExternalValue> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        bool ICollection<KeyValuePair<TKey, TExternalValue>>.Remove(KeyValuePair<TKey, TExternalValue> item)
        {
            throw new NotImplementedException();
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
        public TExternalValue this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a readonly <see cref="ICollection{T}" /> containing the keys of the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a readonly <see cref="ICollection{T}" /> containing the values of the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public ICollection<TExternalValue> Values
        {
            get { throw new NotImplementedException(); }
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
        public bool TryGetValue(TKey key, out TExternalValue value)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey,TValue}" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IDictionary{TKey,TValue}" />.</param>
        /// <returns>
        ///     <see langword="true"/> if item is found in the <see cref="IDictionary{TKey,TValue}" />; otherwise,
        ///     <see langword="false"/>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TExternalValue>>.Contains(KeyValuePair<TKey, TExternalValue> item)
        {
            throw new NotImplementedException();
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
        void ICollection<KeyValuePair<TKey, TExternalValue>>.CopyTo(KeyValuePair<TKey, TExternalValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public int Count
        {
            get { throw new NotImplementedException(); }
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
        public IEnumerator<KeyValuePair<TKey, TExternalValue>> GetEnumerator()
        {
            throw new NotImplementedException();
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
        /// Rolls the current value back to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollBackTo(TimeMarker marker)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rolls the current value back to the start point.
        /// </summary>
        public void RollBackToStart()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rolls the current value forward to the value stored with the given <see cref="TimeMarker"/>.
        /// </summary>
        /// <param name="marker">The marker that indicates to which point in the history the value should be restored to.</param>
        public void RollForwardTo(TimeMarker marker)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the current value in the history list with the given marker.
        /// </summary>
        /// <param name="marker">The marker which indicates at which point on the timeline the data is stored.</param>
        public void StoreCurrent(TimeMarker marker)
        {
            throw new NotImplementedException();
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
