//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Apollo.Utilities.History
{
    internal abstract partial class ListHistoryBase<TExternal, TStorage>
    {
        /// <summary>
        /// An <see cref="ICollectionChange{T}"/> which indicates that a value was inserted into a
        /// <see cref="IList{T}"/> collection.
        /// </summary>
        /// <typeparam name="T">The type of the value that was inserted.</typeparam>
        private sealed class InsertIntoListChange<T> : ICollectionChange<T>
        {
            /// <summary>
            /// The index at which the item was inserted.
            /// </summary>
            private readonly int m_Index;

            /// <summary>
            /// The value that was inserted.
            /// </summary>
            private readonly T m_Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="InsertIntoListChange{T}"/> class.
            /// </summary>
            /// <param name="index">The index at which the item was inserted.</param>
            /// <param name="valueToAdd">The value that should be inserted.</param>
            public InsertIntoListChange(int index, T valueToAdd)
            {
                m_Index = index;
                m_Value = valueToAdd;
            }

            /// <summary>
            /// Applies the changes in the current change to the given collection.
            /// </summary>
            /// <param name="collection">The collection to which the changes should be applied.</param>
            public void ApplyTo(ICollection<T> collection)
            {
                var list = collection as List<T>;
                Debug.Assert(list != null, "This change can only be applied to a List<T> collection.");

                list.Insert(m_Index, m_Value);
            }
        }

        /// <summary>
        /// An <see cref="ICollectionChange{T}"/> which indicates a <see cref="IList{T}"/> collection was cleared.
        /// </summary>
        /// <typeparam name="T">The type of object that is stored by the collection that underwent the change.</typeparam>
        private sealed class ClearListChange<T> : ICollectionChange<T>
        {
            /// <summary>
            /// Applies the changes in the current change to the given collection.
            /// </summary>
            /// <param name="collection">The collection to which the changes should be applied.</param>
            public void ApplyTo(ICollection<T> collection)
            {
                var list = collection as List<T>;
                Debug.Assert(list != null, "This change can only be applied to a List<T> collection.");

                list.Clear();
            }
        }

        /// <summary>
        /// An <see cref="ICollectionChange{T}"/> which indicates an item was removed from an <see cref="IList{T}"/> collection.
        /// </summary>
        /// <typeparam name="T">The type of object that was removed from the collection.</typeparam>
        private sealed class RemoveFromListChange<T> : ICollectionChange<T>
        {
            /// <summary>
            /// The index at which the item should be deleted.
            /// </summary>
            private readonly int m_Index;

            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveFromListChange{T}"/> class.
            /// </summary>
            /// <param name="index">The index at which an item should be removed.</param>
            public RemoveFromListChange(int index)
            {
                m_Index = index;
            }

            /// <summary>
            /// Applies the changes in the current change to the given collection.
            /// </summary>
            /// <param name="collection">The collection to which the changes should be applied.</param>
            public void ApplyTo(ICollection<T> collection)
            {
                var list = collection as List<T>;
                Debug.Assert(list != null, "This change can only be applied to a List<T> collection.");

                list.RemoveAt(m_Index);
            }
        }

        /// <summary>
        /// An <see cref="ICollectionChange{T}"/> which indicates that a value was inserted into a
        /// <see cref="IList{T}"/> collection.
        /// </summary>
        /// <typeparam name="T">The type of the value that was updated.</typeparam>
        private sealed class ItemUpdatedChange<T> : ICollectionChange<T>
        {
            /// <summary>
            /// The index at which the item was changed.
            /// </summary>
            private readonly int m_Index;

            /// <summary>
            /// The new value for the item.
            /// </summary>
            private readonly T m_Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="ItemUpdatedChange{T}"/> class.
            /// </summary>
            /// <param name="index">The index at which the item was changed.</param>
            /// <param name="valueToAdd">The new value for the item.</param>
            public ItemUpdatedChange(int index, T valueToAdd)
            {
                m_Index = index;
                m_Value = valueToAdd;
            }

            /// <summary>
            /// Applies the changes in the current change to the given collection.
            /// </summary>
            /// <param name="collection">The collection to which the changes should be applied.</param>
            public void ApplyTo(ICollection<T> collection)
            {
                var list = collection as List<T>;
                Debug.Assert(list != null, "This change can only be applied to a List<T> collection.");

                list[m_Index] = m_Value;
            }
        }
    }
}
