//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class HistoryObjectDictionaryTimelineStorageTest
    {
        private sealed class MockHistoryObject : IAmHistoryEnabled, IEquatable<MockHistoryObject>
        {
            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(MockHistoryObject first, MockHistoryObject second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return true;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return nonNullObject.Equals(possibleNullObject);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="first">The first object.</param>
            /// <param name="second">The second object.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(MockHistoryObject first, MockHistoryObject second)
            {
                // Check if first is a null reference by using ReferenceEquals because
                // we overload the == operator. If first isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
                {
                    return false;
                }

                var nonNullObject = first;
                var possibleNullObject = second;
                if (ReferenceEquals(first, null))
                {
                    nonNullObject = second;
                    possibleNullObject = first;
                }

                return !nonNullObject.Equals(possibleNullObject);
            }

            private readonly HistoryId m_Id;

            public MockHistoryObject(long id)
            {
                m_Id = new HistoryId(id);
            }

            public MockHistoryObject(HistoryId id)
            {
                m_Id = id;
            }

            public HistoryId HistoryId
            {
                get
                {
                    return m_Id;
                }
            }

            public void Dispose()
            {
                // don't really care about this.
            }

            /// <summary>
            /// Determines whether the specified <see cref=""/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref=""/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref=""/> is equal to this instance;
            ///     otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Equals(MockHistoryObject other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                // Check if other is a null reference by using ReferenceEquals because
                // we overload the == operator. If other isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                return !ReferenceEquals(other, null) && HistoryId.Equals(other.HistoryId);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
            ///     otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public sealed override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                var historyObj = obj as MockHistoryObject;
                return Equals(historyObj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                // As obtained from the Jon Skeet answer to:
                // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
                // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
                //
                // Overflow is fine, just wrap
                unchecked
                {
                    // Pick a random prime number
                    int hash = 17;

                    // Mash the hash together with yet another random prime number
                    return (hash * 23) ^ HistoryId.GetHashCode();
                }
            }
        }

        [Test]
        public void Add()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int key = 5;
            var value = new MockHistoryObject(10);
            objects.Add(value.HistoryId, value);
            storage.Add(key, value);

            Assert.IsTrue(storage.ContainsKey(key));
            Assert.AreEqual(1, storage.Count);
            Assert.AreSame(value, storage[key]);
        }

        [Test]
        public void AddWithNullKey()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<string, MockHistoryObject>(lookupFunc);
            Assert.Throws<ArgumentNullException>(() => storage.Add(null, new MockHistoryObject(0)));
        }

        [Test]
        public void AddWithDuplicateKey()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int key = 5;
            var value = new MockHistoryObject(10);
            objects.Add(value.HistoryId, value);
            storage.Add(key, value);

            Assert.Throws<ArgumentException>(() => storage.Add(key, value));
        }

        [Test]
        public void Remove()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int key = 5;
            var value = new MockHistoryObject(10);
            objects.Add(value.HistoryId, value);
            storage.Add(key, value);

            var result = storage.Remove(key);
            Assert.IsTrue(result);
            Assert.AreEqual(0, storage.Count);
            Assert.IsFalse(storage.ContainsKey(key));
        }

        [Test]
        public void RemoveWithNullKey()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<string, MockHistoryObject>(lookupFunc);

            bool result = true;
            Assert.DoesNotThrow(
                () =>
                {
                    result = storage.Remove(null);
                });
            Assert.IsFalse(result);
        }

        [Test]
        public void RemoveWithUnknownKey()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int key = 5;
            var value = new MockHistoryObject(10);
            objects.Add(value.HistoryId, value);
            storage.Add(key, value);

            Assert.IsFalse(storage.Remove(6));
        }

        [Test]
        public void Clear()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int key = 5;
            var value = new MockHistoryObject(10);
            objects.Add(value.HistoryId, value);

            storage.Add(key, value);
            storage.Clear();

            Assert.AreEqual(0, storage.Count);
            Assert.IsFalse(storage.ContainsKey(key));
        }

        [Test]
        public void TryGetValueWithExistingValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int key = 5;
            var value = new MockHistoryObject(10);
            objects.Add(value.HistoryId, value);
            storage.Add(key, value);

            MockHistoryObject result;
            var success = storage.TryGetValue(key, out result);

            Assert.IsTrue(success);
            Assert.AreSame(value, result);
        }

        [Test]
        public void TryGetValueWithNullKey()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<string, MockHistoryObject>(lookupFunc);

            MockHistoryObject result;
            var success = storage.TryGetValue(null, out result);

            Assert.IsFalse(success);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void TryGetValueWithUnknownValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int key = 5;
            var value = new MockHistoryObject(10);
            objects.Add(value.HistoryId, value);
            storage.Add(key, value);

            MockHistoryObject result;
            var success = storage.TryGetValue(6, out result);

            Assert.IsFalse(success);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void RollBackToBeforeLastSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(25));

            Assert.AreEqual(25, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackToLastSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The first snapshot is at 1, the next one is 20 further so it is at 21
            storage.RollBackTo(new TimeMarker(21));
            Assert.AreEqual(21, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackPastLastSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(15));
            Assert.AreEqual(15, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackToCurrentValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker((ulong)maximumValue));
            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackToFirstValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(1, storage.Count);
            Assert.IsTrue(storage.ContainsKey(0));
        }

        [Test]
        public void RollBackPastFirstValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(0));
            Assert.AreEqual(0, storage.Count);
        }

        [Test]
        public void RollBackWithNoValues()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);
            Assert.DoesNotThrow(() => storage.RollBackTo(new TimeMarker(1)));
        }

        [Test]
        public void RollBackToStartWithNoValues()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);
            Assert.DoesNotThrow(() => storage.RollBackToStart());
        }

        [Test]
        public void RollBackToStart()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackToStart();
            Assert.AreEqual(0, storage.Count);
        }

        [Test]
        public void RollBackMultipleTimes()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            for (int i = maximumValue - 1; i >= 0; i--)
            {
                storage.RollBackTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.ContainsKey(j - 1));
                }
            }
        }

        [Test]
        public void RollBackThroughClear()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Clear();
            var otherObj = new MockHistoryObject(maximumValue + 1);
            objects.Add(otherObj.HistoryId, otherObj);
            storage.Add(maximumValue + 1, otherObj);

            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackThroughRemove()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackThroughUpdate()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var otherObj = new MockHistoryObject(maximumValue);
            objects.Add(otherObj.HistoryId, otherObj);

            storage[5] = otherObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToPriorToNextSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(20));

            Assert.AreEqual(20, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToNextSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(21));

            Assert.AreEqual(21, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToPastNextSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(25));

            Assert.AreEqual(25, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToCurrentValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker(5));

            Assert.AreEqual(5, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToLastValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)maximumValue));

            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardPastLastValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)(maximumValue + 1)));

            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreSame(objects[new HistoryId(index)], pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardMultipleTimes()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackToStart();
            for (int i = 1; i < maximumValue; i++)
            {
                storage.RollForwardTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.ContainsKey(j - 1));
                }
            }
        }

        [Test]
        public void RollForwardThroughClear()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Clear();

            var otherObj = new MockHistoryObject(maximumValue + 1);
            objects.Add(otherObj.HistoryId, otherObj);
            storage.Add(maximumValue + 1, otherObj);

            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));

            Assert.AreEqual(1, storage.Count);
            Assert.IsTrue(storage.ContainsKey(maximumValue + 1));
        }

        [Test]
        public void RollForwardThroughRemove()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));

            Assert.AreEqual(maximumValue - 1, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                if (index < 5)
                {
                    Assert.AreEqual(index, pair.Key);
                    Assert.AreSame(objects[new HistoryId(index)], pair.Value);
                }
                else
                {
                    Assert.AreEqual(index + 1, pair.Key);
                    Assert.AreSame(objects[new HistoryId(index + 1)], pair.Value);
                }

                index++;
            }
        }

        [Test]
        public void RollForwardThroughUpdate()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var otherObj = new MockHistoryObject(maximumValue);
            objects.Add(otherObj.HistoryId, otherObj);
            storage[5] = otherObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));

            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                if (index != 5)
                {
                    Assert.AreSame(objects[new HistoryId(index)], pair.Value);
                }
                else
                {
                    Assert.AreSame(objects[new HistoryId(maximumValue)], pair.Value);
                }

                index++;
            }
        }

        [Test]
        public void AddVoidsForwardStack()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var otherObj = new MockHistoryObject(maximumValue);
            objects.Add(otherObj.HistoryId, otherObj);
            storage[5] = otherObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            var yetAnotherObj = new MockHistoryObject(maximumValue + 1);
            objects.Add(yetAnotherObj.HistoryId, yetAnotherObj);
            storage.Add(11, yetAnotherObj);
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));

            Assert.AreEqual(maximumValue + 1, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                if (index < maximumValue)
                {
                    Assert.AreEqual(index, pair.Key);
                    Assert.AreSame(objects[new HistoryId(index)], pair.Value);
                }
                else
                {
                    Assert.AreEqual(index + 1, pair.Key);
                    Assert.AreSame(objects[new HistoryId(index + 1)], pair.Value);
                }

                index++;
            }
        }

        [Test]
        public void ClearVoidsForwardStack()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var otherObj = new MockHistoryObject(maximumValue);
            objects.Add(otherObj.HistoryId, otherObj);
            storage[5] = otherObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            storage.Clear();

            var yetAnotherObj = new MockHistoryObject(maximumValue + 1);
            objects.Add(yetAnotherObj.HistoryId, yetAnotherObj);
            storage.Add(maximumValue + 1, yetAnotherObj);
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));

            Assert.AreEqual(1, storage.Count);
            Assert.IsTrue(storage.ContainsKey(maximumValue + 1));
        }

        [Test]
        public void RemoveVoidsForwardStack()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var otherObj = new MockHistoryObject(maximumValue + 1);
            objects.Add(otherObj.HistoryId, otherObj);
            storage[5] = otherObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(maximumValue - 1, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                if (index < 5)
                {
                    Assert.AreEqual(index, pair.Key);
                    Assert.AreSame(objects[new HistoryId(index)], pair.Value);
                }
                else
                {
                    Assert.AreEqual(index + 1, pair.Key);
                    Assert.AreSame(objects[new HistoryId(index + 1)], pair.Value);
                }

                index++;
            }
        }

        [Test]
        public void UpdateClearsForwardStack()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectDictionaryTimelineStorage<int, MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(i, obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var otherObj = new MockHistoryObject(maximumValue + 1);
            objects.Add(otherObj.HistoryId, otherObj);
            storage[5] = otherObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            storage[9] = otherObj;
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));

            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                if (index != 9)
                {
                    Assert.AreSame(objects[new HistoryId(index)], pair.Value);
                }
                else
                {
                    Assert.AreSame(objects[new HistoryId(maximumValue + 1)], pair.Value);
                }

                index++;
            }
        }
    }
}
