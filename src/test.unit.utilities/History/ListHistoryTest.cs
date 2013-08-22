//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ListHistoryTest
    {
        [Test]
        public void IndexerGetItemsAtInvalidIndex()
        {
            var list = new ListHistory<string>
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e",
                    "f",
                };

            Assert.Throws<IndexOutOfRangeException>(
                () =>
                {
                    var result = list[-1];
                });

            Assert.Throws<IndexOutOfRangeException>(
                () =>
                {
                    var result = list[list.Count];
                });
        }

        [Test]
        public void IndexerGetSetItems()
        {
            var list = new ListHistory<string>
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e",
                    "f",
                };

            Assert.AreEqual("a", list[0]);
            list[0] = "aa";
            Assert.AreEqual("aa", list[0]);

            Assert.AreEqual("c", list[2]);
            list[2] = "cc";
            Assert.AreEqual("cc", list[2]);

            Assert.AreEqual("f", list[5]);
            list[5] = "ff";
            Assert.AreEqual("ff", list[5]);
        }

        [Test]
        public void IndexOfItems()
        {
            var list = new ListHistory<string>
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e",
                    "f",
                };

            Assert.AreEqual(0, list.IndexOf("a"));
            Assert.AreEqual(1, list.IndexOf("b"));
            Assert.AreEqual(2, list.IndexOf("c"));
            Assert.AreEqual(3, list.IndexOf("d"));
            Assert.AreEqual(4, list.IndexOf("e"));
            Assert.AreEqual(5, list.IndexOf("f"));
        }

        [Test]
        public void InsertItemsAtInvalidIndex()
        {
            var list = new ListHistory<string>
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e",
                    "f",
                };

            Assert.Throws<IndexOutOfRangeException>(() => list.Insert(-1, "aa"));
            Assert.Throws<IndexOutOfRangeException>(() => list.Insert(list.Count, "bb"));
        }

        [Test]
        public void RemoveItemsAtInvalidIndex()
        {
            var list = new ListHistory<string>
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e",
                    "f",
                };

            Assert.Throws<IndexOutOfRangeException>(() => list.RemoveAt(-1));
            Assert.Throws<IndexOutOfRangeException>(() => list.RemoveAt(list.Count));
        }

        [Test]
        public void RemoveItemsAt()
        {
            var list = new ListHistory<string>
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e",
                    "f",
                };

            list.RemoveAt(3);
            Assert.IsFalse(list.Contains("d"));

            list.RemoveAt(3);
            Assert.IsFalse(list.Contains("e"));

            list.RemoveAt(3);
            Assert.IsFalse(list.Contains("f"));

            list.RemoveAt(2);
            Assert.IsFalse(list.Contains("c"));

            list.RemoveAt(1);
            Assert.IsFalse(list.Contains("b"));

            list.RemoveAt(0);
            Assert.IsFalse(list.Contains("a"));
        }

        [Test]
        public void RemoveItems()
        {
            var list = new ListHistory<string>
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e",
                    "f",
                };

            list.Remove("a");
            Assert.IsFalse(list.Contains("a"));

            list.Remove("b");
            Assert.IsFalse(list.Contains("b"));

            list.Remove("c");
            Assert.IsFalse(list.Contains("c"));
            
            list.Remove("d");
            Assert.IsFalse(list.Contains("d"));

            list.Remove("e");
            Assert.IsFalse(list.Contains("e"));

            list.Remove("f");
            Assert.IsFalse(list.Contains("f"));
        }

        [Test]
        public void RollBackToBeforeLastSnapshot()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(26));
            Assert.That(
                storage,
                Is.EquivalentTo(
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }));
        }

        [Test]
        public void RollBackToLastSnapshot()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The first snapshot is at 1, the next one is 20 further so it is at 20
            storage.RollBackTo(new TimeMarker(21));
            Assert.That(
                storage,
                Is.EquivalentTo(
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }));
        }

        [Test]
        public void RollBackPastLastSnapshot()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(16));
            Assert.That(
                storage,
                Is.EquivalentTo(
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }));
        }

        [Test]
        public void RollBackToCurrentValue()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker((ulong)maximumValue));
            Assert.That(
                storage,
                Is.EquivalentTo(
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        }

        [Test]
        public void RollBackToFirstValue()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(1));
            Assert.That(
                storage,
                Is.EquivalentTo(
                    new int[] { 0 }));
        }

        [Test]
        public void RollBackPastFirstValue()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(0));
            Assert.That(
                storage,
                Is.EquivalentTo(
                    new int[] { }));
        }

        [Test]
        public void RollBackMultipleTimes()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            for (int i = maximumValue; i > 0; i--)
            {
                storage.RollBackTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.Contains(j - 1));
                }
            }
        }

        [Test]
        public void RollBackWithNoValues()
        {
            var storage = new ListHistory<int>();
            Assert.DoesNotThrow(() => storage.RollBackTo(new TimeMarker(0)));
        }

        [Test]
        public void RollBackToStartWithNoValues()
        {
            var storage = new ListHistory<int>();
            Assert.DoesNotThrow(() => storage.RollBackToStart());
        }

        [Test]
        public void RollBackToStart()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackToStart();
            Assert.That(
                storage,
                Is.EquivalentTo(new int[0]));
        }

        [Test]
        public void RollBackThroughClear()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Clear();
            storage.Add(maximumValue + 1);

            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        }

        [Test]
        public void RollBackThroughRemove()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        }

        [Test]
        public void RollBackThroughUpdate()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        }

        [Test]
        public void RollForwardToPriorToNextSnapshot()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(16));
            storage.RollForwardTo(new TimeMarker(20));

            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }));
        }

        [Test]
        public void RollForwardToNextSnapshot()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(16));
            storage.RollForwardTo(new TimeMarker(21));

            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }));
        }

        [Test]
        public void RollForwardToPastNextSnapshot()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(16));
            storage.RollForwardTo(new TimeMarker(26));

            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }));
        }

        [Test]
        public void RollForwardToCurrentValue()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker(6));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void RollForwardToLastValue()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker((ulong)maximumValue));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        }

        [Test]
        public void RollForwardPastLastValue()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker((ulong)(maximumValue + 1)));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        }

        [Test]
        public void RollForwardMultipleTimes()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackToStart();
            for (int i = 1; i < maximumValue; i++)
            {
                storage.RollForwardTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.Contains(j));
                }
            }
        }

        [Test]
        public void RollForwardWithLocalChange()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackToStart();
            storage.Add(maximumValue + 1);

            for (int i = 1; i < maximumValue; i++)
            {
                storage.RollForwardTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.Contains(j));
                }
            }
        }

        [Test]
        public void RollForwardThroughClear()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Clear();
            storage.Add(maximumValue + 1);

            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { maximumValue + 1 }));
        }

        [Test]
        public void RollForwardThroughRemove()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 9 }));
        }

        [Test]
        public void RollForwardThroughUpdate()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackToStart();
            storage.RollForwardTo(new TimeMarker(2));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, maximumValue, 6, 7, 8, 9 }));
        }

        [Test]
        public void AddVoidsForwardStack()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));
            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            storage.Add(11);
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11 }));
        }

        [Test]
        public void ClearVoidsForwardStack()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));
            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            storage.Clear();
            storage.Add(maximumValue + 1);
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { maximumValue + 1 }));
        }

        [Test]
        public void RemoveVoidsForwardStack()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));
            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));
            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 9 }));
        }

        [Test]
        public void UpdateClearsForwardStack()
        {
            var storage = new ListHistory<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            storage[9] = maximumValue;
            storage.StoreCurrent(new TimeMarker(3));

            storage.RollForwardTo(new TimeMarker(2));

            Assert.That(
                storage,
                Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, maximumValue }));
        }
    }
}
