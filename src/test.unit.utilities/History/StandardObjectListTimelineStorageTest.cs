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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(25));
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The first snapshot is at 0, the next one is 20 further so it is at 20
            storage.RollBackTo(new TimeMarker(20));
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(15));
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker((ulong)(maximumValue - 1)));
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(0));
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(0));
            Assert.AreElementsEqual(
                new int[] { 1 },
                storage);
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackToStart();
            Assert.AreElementsEqual(
                new int[] { 0 },
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(19));

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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(20));

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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            // The snapshot is at 20
            storage.RollBackTo(new TimeMarker(15));
            storage.RollForwardTo(new TimeMarker(25));

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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker(5));
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)(maximumValue - 1)));
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
                storage.StoreCurrent(new TimeMarker((ulong)i));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)maximumValue));
            Assert.AreElementsEqual(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                storage);
        }
    }
}
