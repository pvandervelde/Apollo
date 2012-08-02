//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
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
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "Unfortunately we need the 3 parameters to allow for storing history enabled objects.")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "The original is called Dictionary<TKey, TValue> not collection something.")]
    public abstract partial class DictionaryHistoryBase<TKey, TExternal, TStorage>
        : HistorySnapshotStorage<Dictionary<TKey, TStorage>>, IDictionaryTimelineStorage<TKey, TExternal>
    {
        /// <summary>
        /// The function that maps an external object to a storage object.
        /// </summary>
        private readonly Func<TExternal, TStorage> m_ExternalToStorage;

        /// <summary>
        /// The function that maps a storage object to an external object.
        /// </summary>
        private readonly Func<TStorage, TExternal> m_StorageToExternal;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryHistoryBase{TKey, TExternal, TStorage}"/> class.
        /// </summary>
        /// <param name="externalToStorage">The function that maps an external object to a storage object.</param>
        /// <param name="storageToExternal">The function that maps a storage object to an external object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="externalToStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storageToExternal"/> is <see langword="null" />.
        /// </exception>
        protected DictionaryHistoryBase(
            Func<TExternal, TStorage> externalToStorage,
            Func<TStorage, TExternal> storageToExternal)
            : base(old => (old != null) ? new Dictionary<TKey, TStorage>(old) : new Dictionary<TKey, TStorage>())
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
            Current.Add(key, storageItem);
            Changes.Add(new AddToDictionaryChange(key, storageItem));
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
            var hadItems = Current.Count > 0;
            Current.Clear();

            Changes.Clear();
            if (hadItems)
            {
                Changes.Add(new ClearDictionaryChange());
            }
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
                result = result && Current.Remove(key);
                Changes.Add(new RemoveFromDictionaryChange(key));
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
                return m_StorageToExternal(Current[key]);
            }

            set
            {
                var storageItem = m_ExternalToStorage(value);
                Current[key] = storageItem;
                Changes.Add(new ItemUpdatedChange(key, storageItem));
            }
        }

        /// <summary>
        /// Gets a readonly <see cref="ICollection{T}" /> containing the keys of the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                return new List<TKey>(Current.Keys);
            }
        }

        /// <summary>
        /// Gets a readonly <see cref="ICollection{T}" /> containing the values of the <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        public ICollection<TExternal> Values
        {
            get
            {
                return (from storedValue in Current.Values select m_StorageToExternal(storedValue)).ToList();
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
            var result = Current.TryGetValue(key, out storedValue);
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

            return Current.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey,TValue}" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IDictionary{TKey,TValue}" />.</param>
        /// <returns>
        ///     <see langword="true"/> if item is found in the <see cref="IDictionary{TKey,TValue}" />; otherwise,
        ///     <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Justification = "The whole goal is to hide this method from use. We don't really want people calling it.")]
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
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Justification = "The whole goal is to hide this method from use. We don't really want people calling it.")]
        void ICollection<KeyValuePair<TKey, TExternal>>.CopyTo(KeyValuePair<TKey, TExternal>[] array, int arrayIndex)
        {
            {
                Lokad.Enforce.Argument(() => array);
                Lokad.Enforce.With<ArgumentOutOfRangeException>(
                    arrayIndex >= 0,
                    Resources.Exceptions_Messages_ArgumentOutOfRange_WithArgument,
                    arrayIndex);

                Lokad.Enforce.With<ArgumentException>(
                    Current.Count <= (array.Length - arrayIndex),
                    Resources.Exceptions_Messages_TheArrayIsShorterThanTheNumberOfItems_WithValues,
                    array.Length - arrayIndex,
                    Current.Count);
            }

            int j = arrayIndex;
            foreach (var pair in Current)
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
                return Current.Count;
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
            foreach (var pair in Current)
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
    }
}
