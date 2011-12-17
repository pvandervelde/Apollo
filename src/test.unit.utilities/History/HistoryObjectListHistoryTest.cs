//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class HistoryObjectListHistoryTest
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

        private static MockHistoryObject FromId(HistoryId id)
        {
            return new MockHistoryObject(id);
        }

        [VerifyContract]
        public readonly IContract ListTests = new ListContract<HistoryObjectListHistory<MockHistoryObject>, MockHistoryObject>
        {
            AcceptEqualItems = true,
            AcceptNullReference = true,
            DefaultInstance = () => new HistoryObjectListHistory<MockHistoryObject>(FromId),
            DistinctInstances = new DistinctInstanceCollection<MockHistoryObject> 
                {
                    new MockHistoryObject(0),
                    new MockHistoryObject(1),
                    new MockHistoryObject(2),
                    new MockHistoryObject(3),
                    new MockHistoryObject(4),
                    new MockHistoryObject(5),
                }
        };

        [Test]
        public void RollBackToBeforeLastSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(26));
            Assert.AreEqual(26, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollBackToLastSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The first snapshot is at 1, the next one is 20 further so it is at 21
            storage.RollBackTo(new TimeMarker(21));
            Assert.AreEqual(21, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollBackPastLastSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(15));
            Assert.AreEqual(15, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollBackToCurrentValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker((ulong)maximumValue));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollBackToFirstValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(1, storage.Count);
            Assert.AreSame(objects[new HistoryId(0)], storage[0]);
        }

        [Test]
        public void RollBackPastFirstValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
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
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);
            Assert.DoesNotThrow(() => storage.RollBackTo(new TimeMarker(1)));
        }

        [Test]
        public void RollBackToStartWithNoValues()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);
            Assert.DoesNotThrow(() => storage.RollBackToStart());
        }

        [Test]
        public void RollBackToStart()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
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
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            for (int i = maximumValue; i > 0; i--)
            {
                storage.RollBackTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.Contains(objects[new HistoryId(j - 1)]));
                }
            }
        }

        [Test]
        public void RollBackThroughClear()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }
            
            storage.StoreCurrent(new TimeMarker(1));

            storage.Clear();
            var newObj = new MockHistoryObject(maximumValue);
            objects.Add(newObj.HistoryId, newObj);
            storage.Add(newObj);

            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollBackThroughRemove()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }
            
            storage.StoreCurrent(new TimeMarker(1));

            storage.Remove(objects[new HistoryId(5)]);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollBackThroughUpdate()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var newObj = new MockHistoryObject(maximumValue);
            objects.Add(newObj.HistoryId, newObj);
            storage[5] = newObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollForwardToPriorToNextSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(20));
            Assert.AreEqual(20, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollForwardToNextSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(21));
            Assert.AreEqual(21, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollForwardToPastNextSnapshot()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(25));
            Assert.AreEqual(25, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollForwardToCurrentValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker(5));
            Assert.AreEqual(5, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollForwardToLastValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)maximumValue));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollForwardPastLastValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)(maximumValue + 1)));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                Assert.AreSame(objects[new HistoryId(i)], storage[i]);
            }
        }

        [Test]
        public void RollForwardMultipleTimes()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackToStart();
            for (int i = 1; i < maximumValue; i++)
            {
                storage.RollForwardTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.Contains(objects[new HistoryId(j - 1)]));
                }
            }
        }

        [Test]
        public void RollForwardWithLocalChange()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackToStart();

            var newObj = new MockHistoryObject(maximumValue + 1);
            objects.Add(newObj.HistoryId, newObj);
            storage.Add(newObj);

            for (int i = 1; i < maximumValue; i++)
            {
                storage.RollForwardTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.Contains(objects[new HistoryId(j - 1)]));
                }
            }
        }

        [Test]
        public void RollForwardThroughClear()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Clear();
            var newObj = new MockHistoryObject(maximumValue);
            objects.Add(newObj.HistoryId, newObj);
            storage.Add(newObj);

            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(1, storage.Count);
            Assert.AreSame(objects[new HistoryId(maximumValue)], storage[0]);
        }

        [Test]
        public void RollForwardThroughRemove()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.RemoveAt(5);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(maximumValue - 1, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                if (i < 5)
                {
                    Assert.AreSame(objects[new HistoryId(i)], storage[i]);
                }
                else
                {
                    Assert.AreSame(objects[new HistoryId(i + 1)], storage[i]);
                }
            }
        }

        [Test]
        public void RollForwardThroughUpdate()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var newObj = new MockHistoryObject(maximumValue);
            objects.Add(newObj.HistoryId, newObj);
            storage[5] = newObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                if (i != 5)
                {
                    Assert.AreSame(objects[new HistoryId(i)], storage[i]);
                }
                else
                {
                    Assert.AreSame(objects[new HistoryId(maximumValue)], storage[i]);
                }
            }
        }

        [Test]
        public void AddVoidsForwardStack()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var newObj = new MockHistoryObject(maximumValue);
            objects.Add(newObj.HistoryId, newObj);
            storage[5] = newObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.Add(newObj);
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollBackTo(new TimeMarker(1));
            storage.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                if (i != 5)
                {
                    Assert.AreSame(objects[new HistoryId(i)], storage[i]);
                }
                else 
                {
                    Assert.AreSame(objects[new HistoryId(maximumValue)], storage[i]);
                }
            }
        }

        [Test]
        public void ClearVoidsForwardStack()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var newObj = new MockHistoryObject(maximumValue);
            objects.Add(newObj.HistoryId, newObj);
            storage[5] = newObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            storage.Clear();
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(0, storage.Count);
        }

        [Test]
        public void RemoveVoidsForwardStack()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var newObj = new MockHistoryObject(maximumValue);
            objects.Add(newObj.HistoryId, newObj);
            storage[5] = newObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            storage.RemoveAt(5);
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(maximumValue - 1, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                if (i < 5)
                {
                    Assert.AreSame(objects[new HistoryId(i)], storage[i]);
                }
                else 
                {
                    Assert.AreSame(objects[new HistoryId(i + 1)], storage[i]);
                }
            }
        }

        [Test]
        public void UpdateClearsForwardStack()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc = id => objects[id];
            var storage = new HistoryObjectListHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Add(obj);
            }

            storage.StoreCurrent(new TimeMarker(1));

            var newObj = new MockHistoryObject(maximumValue);
            objects.Add(newObj.HistoryId, newObj);
            storage[5] = newObj;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            storage[9] = newObj;
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(maximumValue, storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                if (i != 9)
                {
                    Assert.AreSame(objects[new HistoryId(i)], storage[i]);
                }
                else
                {
                    Assert.AreSame(objects[new HistoryId(maximumValue)], storage[i]);
                }
            }
        }
    }
}
