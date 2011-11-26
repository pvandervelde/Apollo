//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Apollo.Utilities.History
{
    internal abstract partial class DictionaryTimelineStorage<TKey, TExternal, TStorage>
    {
        /// <summary>
        /// An <see cref="ICollectionChange{ TValue }"/> which indicates that a value was added to a
        /// <see cref="IDictionary{TKey, TValue}"/> collection.
        /// </summary>
        /// <typeparam name="TChangeKey">The type of the key with which the value was added.</typeparam>
        /// <typeparam name="TChangeValue">The type of the value that was added.</typeparam>
        private sealed class AddToDictionaryChange<TChangeKey, TChangeValue> : ICollectionChange<KeyValuePair<TChangeKey, TChangeValue>>
        {
            /// <summary>
            /// The key with which the value was added.
            /// </summary>
            private readonly TChangeKey m_Key;

            /// <summary>
            /// The value that was added.
            /// </summary>
            private readonly TChangeValue m_Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="AddToDictionaryChange{TChangeKey, TChangeValue}"/> class.
            /// </summary>
            /// <param name="key">The key at which the item was added.</param>
            /// <param name="value">The value that should be added.</param>
            public AddToDictionaryChange(TChangeKey key, TChangeValue value)
            {
                m_Key = key;
                m_Value = value;
            }

            /// <summary>
            /// Applies the changes in the current change to the given collection.
            /// </summary>
            /// <param name="collection">The collection to which the changes should be applied.</param>
            public void ApplyTo(ICollection<KeyValuePair<TChangeKey, TChangeValue>> collection)
            {
                var dict = collection as Dictionary<TChangeKey, TChangeValue>;
                Debug.Assert(dict != null, "This change can only be applied to a Dictionary<TKey, TValue> collection.");

                dict.Add(m_Key, m_Value);
            }
        }

        /// <summary>
        /// An <see cref="ICollectionChange{ TValue }"/> which indicates that the 
        /// <see cref="IDictionary{TKey, TValue}"/> collection was cleared.
        /// </summary>
        /// <typeparam name="TChangeKey">The type of the key with which the value was added.</typeparam>
        /// <typeparam name="TChangeValue">The type of the value that was added.</typeparam>
        private sealed class ClearDictionaryChange<TChangeKey, TChangeValue> : ICollectionChange<KeyValuePair<TChangeKey, TChangeValue>>
        {
            /// <summary>
            /// Applies the changes in the current change to the given collection.
            /// </summary>
            /// <param name="collection">The collection to which the changes should be applied.</param>
            public void ApplyTo(ICollection<KeyValuePair<TChangeKey, TChangeValue>> collection)
            {
                var dict = collection as Dictionary<TChangeKey, TChangeValue>;
                Debug.Assert(dict != null, "This change can only be applied to a Dictionary<TKey, TValue> collection.");

                dict.Clear();
            }
        }

        /// <summary>
        /// An <see cref="ICollectionChange{ TValue }"/> which indicates that a value was removed from a
        /// <see cref="IDictionary{TKey, TValue}"/> collection.
        /// </summary>
        /// <typeparam name="TChangeKey">The type of the key with which the value was added.</typeparam>
        /// <typeparam name="TChangeValue">The type of the value that was added.</typeparam>
        private sealed class RemoveFromDictionaryChange<TChangeKey, TChangeValue> : ICollectionChange<KeyValuePair<TChangeKey, TChangeValue>>
        {
            /// <summary>
            /// The key for which the item should be deleted.
            /// </summary>
            private readonly TChangeKey m_Key;

            /// <summary>
            /// Initializes a new instance of the <see cref="RemoveFromDictionaryChange{TChangeKey, TChangeValue}"/> class.
            /// </summary>
            /// <param name="key">The key for which an item should be removed.</param>
            public RemoveFromDictionaryChange(TChangeKey key)
            {
                m_Key = key;
            }

            /// <summary>
            /// Applies the changes in the current change to the given collection.
            /// </summary>
            /// <param name="collection">The collection to which the changes should be applied.</param>
            public void ApplyTo(ICollection<KeyValuePair<TChangeKey, TChangeValue>> collection)
            {
                var dict = collection as Dictionary<TChangeKey, TChangeValue>;
                Debug.Assert(dict != null, "This change can only be applied to a Dictionary<TKey, TValue> collection.");

                dict.Remove(m_Key);
            }
        }

        /// <summary>
        /// An <see cref="ICollectionChange{ TValue }"/> which indicates that a value was updated in a
        /// <see cref="IDictionary{TKey, TValue}"/> collection.
        /// </summary>
        /// <typeparam name="TChangeKey">The type of the key with which the value was added.</typeparam>
        /// <typeparam name="TChangeValue">The type of the value that was added.</typeparam>
        private sealed class ItemUpdatedChange<TChangeKey, TChangeValue> : ICollectionChange<KeyValuePair<TChangeKey, TChangeValue>>
        {
            /// <summary>
            /// The index at which the item was changed.
            /// </summary>
            private readonly TChangeKey m_Key;

            /// <summary>
            /// The new value for the item.
            /// </summary>
            private readonly TChangeValue m_Value;

            /// <summary>
            /// Initializes a new instance of the <see cref="ItemUpdatedChange{TChangeKey, TChangeValue}"/> class.
            /// </summary>
            /// <param name="key">The key at which the item was changed.</param>
            /// <param name="valueToAdd">The new value for the item.</param>
            public ItemUpdatedChange(TChangeKey key, TChangeValue valueToAdd)
            {
                m_Key = key;
                m_Value = valueToAdd;
            }

            /// <summary>
            /// Applies the changes in the current change to the given collection.
            /// </summary>
            /// <param name="collection">The collection to which the changes should be applied.</param>
            public void ApplyTo(ICollection<KeyValuePair<TChangeKey, TChangeValue>> collection)
            {
                var dict = collection as Dictionary<TChangeKey, TChangeValue>;
                Debug.Assert(dict != null, "This change can only be applied to a Dictionary<TKey, TValue> collection.");

                dict[m_Key] = m_Value;
            }
        }
    }
}
