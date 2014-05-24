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
    public abstract partial class DictionaryHistoryBase<TKey, TExternal, TStorage>
    {
        /// <summary>
        /// An <see cref="IHistoryChange{ TValue }"/> which indicates that a value was added to a
        /// <see cref="IDictionary{TKey, TValue}"/> collection.
        /// </summary>
        private sealed class AddToDictionaryChange : IHistoryChange<Dictionary<TKey, TStorage>>
        {
            /// <summary>
            /// The key with which the value was added.
            /// </summary>
            private readonly TKey m_Key;

            /// <summary>
            /// The value that was added.
            /// </summary>
            private readonly TStorage m_Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="AddToDictionaryChange"/> class.
            /// </summary>
            /// <param name="key">The key at which the item was added.</param>
            /// <param name="value">The value that should be added.</param>
            public AddToDictionaryChange(TKey key, TStorage value)
            {
                m_Key = key;
                m_Value = value;
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="historyObject"/> is <see langword="null" />.
            /// </exception>
            public void ApplyTo(Dictionary<TKey, TStorage> historyObject)
            {
                {
                    Lokad.Enforce.Argument(() => historyObject);
                }

                historyObject.Add(m_Key, m_Value);
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{ TValue }"/> which indicates that the 
        /// <see cref="IDictionary{TKey, TValue}"/> collection was cleared.
        /// </summary>
        private sealed class ClearDictionaryChange : IHistoryChange<Dictionary<TKey, TStorage>>
        {
            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="historyObject"/> is <see langword="null" />.
            /// </exception>
            public void ApplyTo(Dictionary<TKey, TStorage> historyObject)
            {
                {
                    Lokad.Enforce.Argument(() => historyObject);
                }

                historyObject.Clear();
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{ TValue }"/> which indicates that a value was removed from a
        /// <see cref="IDictionary{TKey, TValue}"/> collection.
        /// </summary>
        private sealed class RemoveFromDictionaryChange : IHistoryChange<Dictionary<TKey, TStorage>>
        {
            /// <summary>
            /// The key for which the item should be deleted.
            /// </summary>
            private readonly TKey m_Key;

            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveFromDictionaryChange"/> class.
            /// </summary>
            /// <param name="key">The key for which an item should be removed.</param>
            public RemoveFromDictionaryChange(TKey key)
            {
                m_Key = key;
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="historyObject"/> is <see langword="null" />.
            /// </exception>
            public void ApplyTo(Dictionary<TKey, TStorage> historyObject)
            {
                {
                    Lokad.Enforce.Argument(() => historyObject);
                }

                historyObject.Remove(m_Key);
            }
        }

        /// <summary>
        /// An <see cref="IHistoryChange{ TValue }"/> which indicates that a value was updated in a
        /// <see cref="IDictionary{TKey, TValue}"/> collection.
        /// </summary>
        private sealed class ItemUpdatedChange : IHistoryChange<Dictionary<TKey, TStorage>>
        {
            /// <summary>
            /// The index at which the item was changed.
            /// </summary>
            private readonly TKey m_Key;

            /// <summary>
            /// The new value for the item.
            /// </summary>
            private readonly TStorage m_Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="ItemUpdatedChange"/> class.
            /// </summary>
            /// <param name="key">The key at which the item was changed.</param>
            /// <param name="valueToAdd">The new value for the item.</param>
            public ItemUpdatedChange(TKey key, TStorage valueToAdd)
            {
                m_Key = key;
                m_Value = valueToAdd;
            }

            /// <summary>
            /// Applies the changes in the current change to the given history object.
            /// </summary>
            /// <param name="historyObject">The object to which the changes should be applied.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="historyObject"/> is <see langword="null" />.
            /// </exception>
            public void ApplyTo(Dictionary<TKey, TStorage> historyObject)
            {
                {
                    Lokad.Enforce.Argument(() => historyObject);
                }

                historyObject[m_Key] = m_Value;
            }
        }
    }
}
