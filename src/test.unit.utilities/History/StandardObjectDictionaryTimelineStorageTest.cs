//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class StandardObjectDictionaryTimelineStorageTest
    {
        [Test]
        public void Add()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int key = 5;
            int value = 10;
            storage.Add(key, value);

            Assert.IsTrue(storage.ContainsKey(key));
            Assert.AreEqual(1, storage.Count);
            Assert.AreEqual(value, storage[key]);
        }

        [Test]
        public void AddWithNullKey()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<string, string>();
            Assert.Throws<ArgumentNullException>(() => storage.Add(null, "a"));
        }

        [Test]
        public void AddWithDuplicateKey()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int key = 5;
            int value = 10;
            storage.Add(key, value);
            Assert.Throws<ArgumentException>(() => storage.Add(key, value));
        }

        [Test]
        public void Remove()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int key = 5;
            int value = 10;
            storage.Add(key, value);

            var result = storage.Remove(key);
            Assert.IsTrue(result);
            Assert.AreEqual(0, storage.Count);
            Assert.IsFalse(storage.ContainsKey(key));
        }

        [Test]
        public void RemoveWithNullKey()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<string, string>();

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
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int key = 5;
            int value = 10;
            storage.Add(key, value);

            Assert.IsFalse(storage.Remove(value));
        }

        [Test]
        public void Clear()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int key = 5;
            int value = 10;
            storage.Add(key, value);
            storage.Clear();

            Assert.AreEqual(0, storage.Count);
            Assert.IsFalse(storage.ContainsKey(key));
        }

        [Test]
        public void TryGetValueWithExistingValue()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int key = 5;
            int value = 10;
            storage.Add(key, value);

            int result;
            var success = storage.TryGetValue(key, out result);

            Assert.IsTrue(success);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void TryGetValueWithNullKey()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<string, string>();

            string result;
            var success = storage.TryGetValue(null, out result);

            Assert.IsFalse(success);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void TryGetValueWithUnknownValue()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int key = 5;
            int value = 10;
            storage.Add(key, value);

            int result;
            var success = storage.TryGetValue(value, out result);

            Assert.IsFalse(success);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void RollBackToBeforeLastSnapshot()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(25));

            Assert.AreEqual(26, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackToLastSnapshot()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The first snapshot is at 0, the next one is 20 further so it is at 20
            storage.RollBackTo(new TimeMarker(20));
            Assert.AreEqual(21, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackPastLastSnapshot()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(15));
            Assert.AreEqual(16, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackToCurrentValue()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker((ulong)(maximumValue - 1)));
            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackToFirstValue()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(0));
            Assert.AreEqual(1, storage.Count);
            Assert.IsTrue(storage.ContainsKey(0));
        }

        [Test]
        public void RollBackPastFirstValue()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(0));
            Assert.AreEqual(1, storage.Count);
            Assert.IsTrue(storage.ContainsKey(1));
        }

        [Test]
        public void RollBackWithNoValues()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();
            Assert.DoesNotThrow(() => storage.RollBackTo(new TimeMarker(0)));
        }

        [Test]
        public void RollBackToStartWithNoValues()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();
            Assert.DoesNotThrow(() => storage.RollBackToStart());
        }

        [Test]
        public void RollBackToStart()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, 1);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackToStart();
            Assert.AreEqual(1, storage.Count);
            Assert.IsTrue(storage.ContainsKey(0));
        }

        [Test]
        public void RollBackThroughClear()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));

            storage.Clear();
            storage.Add(maximumValue + 1, maximumValue + 1);

            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();
            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackThroughRemove()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();
            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollBackThroughUpdate()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));

            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();
            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToPriorToNextSnapshot()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(19));

            Assert.AreEqual(20, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToNextSnapshot()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(20));

            Assert.AreEqual(21, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToPastNextSnapshot()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(25));

            Assert.AreEqual(26, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToCurrentValue()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker(5));

            Assert.AreEqual(6, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardToLastValue()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)(maximumValue - 1)));

            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardPastLastValue()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)maximumValue));

            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                Assert.AreEqual(index, pair.Value);

                index++;
            }
        }

        [Test]
        public void RollForwardThroughClear()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));

            storage.Clear();
            storage.Add(maximumValue + 1, maximumValue + 1);

            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(1));

            Assert.AreEqual(1, storage.Count);
            Assert.IsTrue(storage.ContainsKey(maximumValue + 1));
        }

        [Test]
        public void RollForwardThroughRemove()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(1));

            Assert.AreEqual(maximumValue - 1, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                if (index < 5)
                {
                    Assert.AreEqual(index, pair.Key);
                    Assert.AreEqual(index, pair.Value);
                }
                else 
                {
                    Assert.AreEqual(index + 1, pair.Key);
                    Assert.AreEqual(index + 1, pair.Value);
                }

                index++;
            }
        }

        [Test]
        public void RollForwardThroughUpdate()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));

            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(1));

            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                if (index != 5)
                {
                    Assert.AreEqual(index, pair.Value);
                }
                else
                {
                    Assert.AreEqual(maximumValue, pair.Value);
                }

                index++;
            }
        }

        [Test]
        public void AddVoidsForwardStack()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));
            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();

            storage.Add(11, 11);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollForwardTo(new TimeMarker(1));

            Assert.AreEqual(maximumValue + 1, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                if (index < maximumValue)
                {
                    Assert.AreEqual(index, pair.Key);
                    Assert.AreEqual(index, pair.Value);
                }
                else
                {
                    Assert.AreEqual(index + 1, pair.Key);
                    Assert.AreEqual(index + 1, pair.Value);
                }

                index++;
            }
        }

        [Test]
        public void ClearVoidsForwardStack()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));
            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();

            storage.Clear();
            storage.Add(maximumValue + 1, maximumValue + 1);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollForwardTo(new TimeMarker(1));

            Assert.AreEqual(1, storage.Count);
            Assert.IsTrue(storage.ContainsKey(maximumValue + 1));
        }

        [Test]
        public void RemoveVoidsForwardStack()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));
            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollForwardTo(new TimeMarker(1));
            Assert.AreEqual(maximumValue - 1, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                if (index < 5)
                {
                    Assert.AreEqual(index, pair.Key);
                    Assert.AreEqual(index, pair.Value);
                }
                else
                {
                    Assert.AreEqual(index + 1, pair.Key);
                    Assert.AreEqual(index + 1, pair.Value);
                }

                index++;
            }
        }

        [Test]
        public void UpdateClearsForwardStack()
        {
            var storage = new StandardObjectDictionaryTimelineStorage<int, int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i, i);
            }

            storage.StoreCurrent(new TimeMarker(0));

            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(1));

            storage.RollBackToStart();
            storage[9] = maximumValue;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollForwardTo(new TimeMarker(1));

            Assert.AreEqual(maximumValue, storage.Count);

            int index = 0;
            foreach (var pair in storage)
            {
                Assert.AreEqual(index, pair.Key);
                if (index != 9)
                {
                    Assert.AreEqual(index, pair.Value);
                }
                else
                {
                    Assert.AreEqual(maximumValue, pair.Value);
                }

                index++;
            }
        }
    }
}
