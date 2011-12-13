//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class StandardObjectListTimelineStorageTest
    {
        [VerifyContract]
        public readonly IContract ListTests = new ListContract<StandardObjectListTimelineStorage<string>, string>
        {
            AcceptEqualItems = true,
            AcceptNullReference = true,
            DefaultInstance = () => new StandardObjectListTimelineStorage<string>(),
            DistinctInstances = new DistinctInstanceCollection<string> 
                {
                    "a",
                    "b",
                    "c",
                    "d",
                    "e",
                    "f",
                }
        };

        [Test]
        public void RollBackToBeforeLastSnapshot()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(26));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 },
                storage);
        }

        [Test]
        public void RollBackToLastSnapshot()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The first snapshot is at 1, the next one is 20 further so it is at 20
            storage.RollBackTo(new TimeMarker(21));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
                storage);
        }

        [Test]
        public void RollBackPastLastSnapshot()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(16));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
                storage);
        }

        [Test]
        public void RollBackToCurrentValue()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker((ulong)maximumValue));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void RollBackToFirstValue()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreElementsEqual(
                new int[] { 0 },
                storage);
        }

        [Test]
        public void RollBackPastFirstValue()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(0));
            Assert.AreElementsEqual(
                new int[] { },
                storage);
        }

        [Test]
        public void RollBackMultipleTimes()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            for (int i = maximumValue - 1; i >= 0; i--)
            {
                storage.RollBackTo(new TimeMarker((ulong)i));
                Assert.AreEqual(i, storage.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(storage.Contains(j));
                }
            }
        }

        [Test]
        public void RollBackWithNoValues()
        {
            var storage = new StandardObjectListTimelineStorage<int>();
            Assert.DoesNotThrow(() => storage.RollBackTo(new TimeMarker(0)));
        }

        [Test]
        public void RollBackToStartWithNoValues()
        {
            var storage = new StandardObjectListTimelineStorage<int>();
            Assert.DoesNotThrow(() => storage.RollBackToStart());
        }

        [Test]
        public void RollBackToStart()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackToStart();
            Assert.AreElementsEqual(
                new int[0],
                storage);
        }

        [Test]
        public void RollBackThroughClear()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

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
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void RollBackThroughRemove()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage.Remove(5);
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void RollBackThroughUpdate()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
            }

            storage.StoreCurrent(new TimeMarker(1));

            storage[5] = maximumValue;
            storage.StoreCurrent(new TimeMarker(2));

            storage.RollBackTo(new TimeMarker(1));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void RollForwardToPriorToNextSnapshot()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(16));
            storage.RollForwardTo(new TimeMarker(20));

            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 },
                storage);
        }

        [Test]
        public void RollForwardToNextSnapshot()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(16));
            storage.RollForwardTo(new TimeMarker(21));

            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
                storage);
        }

        [Test]
        public void RollForwardToPastNextSnapshot()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 30;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            // The snapshot is at 21
            storage.RollBackTo(new TimeMarker(16));
            storage.RollForwardTo(new TimeMarker(26));

            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 },
                storage);
        }

        [Test]
        public void RollForwardToCurrentValue()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker(6));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5 },
                storage);
        }

        [Test]
        public void RollForwardToLastValue()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker((ulong)maximumValue));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void RollForwardPastLastValue()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Add(i);
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker((ulong)(maximumValue + 1)));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void RollForwardMultipleTimes()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

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
            var storage = new StandardObjectListTimelineStorage<int>();

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
            var storage = new StandardObjectListTimelineStorage<int>();

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
            Assert.AreElementsEqual(
                new int[] { maximumValue + 1 },
                storage);
        }

        [Test]
        public void RollForwardThroughRemove()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

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
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void RollForwardThroughUpdate()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

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
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, maximumValue, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void AddVoidsForwardStack()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

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
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11 },
                storage);
        }

        [Test]
        public void ClearVoidsForwardStack()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

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
            Assert.AreElementsEqual(
                new int[] { maximumValue + 1 },
                storage);
        }

        [Test]
        public void RemoveVoidsForwardStack()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

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
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 9 },
                storage);
        }

        [Test]
        public void UpdateClearsForwardStack()
        {
            var storage = new StandardObjectListTimelineStorage<int>();

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

            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, maximumValue },
                storage);
        }
    }
}
