//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
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
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Base class for history enabled lists. The name seems about right.")]
    public abstract partial class ListHistoryBase<TExternal, TStorage> : HistorySnapshotStorage<List<TStorage>>, IListTimelineStorage<TExternal>
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
            : base(old => (old != null) ? new List<TStorage>(old) : new List<TStorage>())
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
            Current.Add(storageItem);
            Changes.Add(new InsertIntoListChange(Current.Count - 1, storageItem));
        }

        /// <summary>
        /// Removes all items from the <see cref="IList{T}"/>.
        /// </summary>
        public void Clear()
        {
            Current.Clear();

            Changes.Clear();
            Changes.Add(new ClearListChange());
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
            Current.Insert(index, storageItem);
            Changes.Add(new InsertIntoListChange(index, storageItem));
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
            var index = Current.IndexOf(storageItem);
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

            Current.RemoveAt(index);
            Changes.Add(new RemoveFromListChange(index));
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
                return m_StorageToExternal(Current[index]);
            }

            set
            {
                var storageItem = m_ExternalToStorage(value);
                Current[index] = storageItem;
                Changes.Add(new ItemUpdatedChange(index, storageItem));
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList{T}"/>.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(TExternal item)
        {
            return Current.IndexOf(m_ExternalToStorage(item));
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
            return Current.Contains(m_ExternalToStorage(item));
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
                    Current.Count <= (array.Length - arrayIndex),
                    Resources.Exceptions_Messages_TheArrayIsShorterThanTheNumberOfItems_WithValues,
                    array.Length - arrayIndex,
                    Current.Count);
            }

            for (int i = 0, j = arrayIndex; i < Current.Count; i++, j++)
            {
                array[j] = m_StorageToExternal(Current[i]);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IList{T}"/>.
        /// </summary>
        public int Count
        {
            get 
            {
                return Current.Count;
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
            foreach (var item in Current)
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
    }
}
