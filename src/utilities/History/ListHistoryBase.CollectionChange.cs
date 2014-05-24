//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <content>
    /// Defines the different <see cref="IHistoryChange{T}"/> classes that are used to track 
    /// changes to the collection.
    /// </content>
    public abstract partial class ListHistoryBase<TExternal, TStorage>
    {
        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates that a value was inserted into a
        /// <see cref="IList{TStorage}"/> collection.
        /// </summary>
        private sealed class InsertIntoListChange : IHistoryChange<List<TStorage>>
        {
            /// <summary>
            /// The index at which the item was inserted.
            /// </summary>
            private readonly int m_Index;

            /// <summary>
            /// The value that was inserted.
            /// </summary>
            private readonly TStorage m_Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="InsertIntoListChange"/> class.
            /// </summary>
            /// <param name="index">The index at which the item was inserted.</param>
            /// <param name="valueToAdd">The value that should be inserted.</param>
            public InsertIntoListChange(int index, TStorage valueToAdd)
            {
                m_Index = index;
                m_Value = valueToAdd;
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="historyObject"/> is <see langword="null" />.
            /// </exception>
            public void ApplyTo(List<TStorage> historyObject)
            {
                {
                    Lokad.Enforce.Argument(() => historyObject);
                }

                historyObject.Insert(m_Index, m_Value);
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates a <see cref="IList{T}"/> collection was cleared.
        /// </summary>
        private sealed class ClearListChange : IHistoryChange<List<TStorage>>
        {
            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="historyObject"/> is <see langword="null" />.
            /// </exception>
            public void ApplyTo(List<TStorage> historyObject)
            {
                {
                    Lokad.Enforce.Argument(() => historyObject);
                }

                historyObject.Clear();
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates an item was removed from an <see cref="IList{T}"/> collection.
        /// </summary>
        private sealed class RemoveFromListChange : IHistoryChange<List<TStorage>>
        {
            /// <summary>
            /// The index at which the item should be deleted.
            /// </summary>
            private readonly int m_Index;

            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveFromListChange"/> class.
            /// </summary>
            /// <param name="index">The index at which an item should be removed.</param>
            public RemoveFromListChange(int index)
            {
                m_Index = index;
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="historyObject"/> is <see langword="null" />.
            /// </exception>
            public void ApplyTo(List<TStorage> historyObject)
            {
                {
                    Lokad.Enforce.Argument(() => historyObject);
                }

                historyObject.RemoveAt(m_Index);
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{T}"/> which indicates that a value was inserted into a
        /// <see cref="IList{T}"/> collection.
        /// </summary>
        private sealed class ItemUpdatedChange : IHistoryChange<List<TStorage>>
        {
            /// <summary>
            /// The index at which the item was changed.
            /// </summary>
            private readonly int m_Index;

            /// <summary>
            /// The new value for the item.
            /// </summary>
            private readonly TStorage m_Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="ItemUpdatedChange"/> class.
            /// </summary>
            /// <param name="index">The index at which the item was changed.</param>
            /// <param name="valueToAdd">The new value for the item.</param>
            public ItemUpdatedChange(int index, TStorage valueToAdd)
            {
                m_Index = index;
                m_Value = valueToAdd;
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="historyObject"/> is <see langword="null" />.
            /// </exception>
            public void ApplyTo(List<TStorage> historyObject)
            {
                {
                    Lokad.Enforce.Argument(() => historyObject);
                }

                historyObject[m_Index] = m_Value;
            }
        }
    }
}
