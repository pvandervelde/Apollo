//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class StandardObjectTimelineStorageTest
    {
        [Test]
        public void RollBackToPreviousValue()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            for (int i = maximumValue; i > 0; i--)
            {
                storage.RollBackTo(new TimeMarker((ulong)i));
                int storedValue = storage;
                Assert.AreEqual(i - 1, storedValue);
            }
        }

        [Test]
        public void RollBackToToCurrentValue()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker((ulong)maximumValue));
            int storedValue = storage;
            Assert.AreEqual(maximumValue - 1, storedValue);
        }

        [Test]
        public void RollBackToFirstValue()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(1));
            int storedValue = storage;
            Assert.AreEqual(0, storedValue);
        }

        [Test]
        public void RollBackPastFirstValue()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(0));
            int storedValue = storage;
            Assert.AreEqual(0, storedValue);
        }

        [Test]
        public void RollBackToStartWithNoValues()
        {
            var storage = new StandardObjectTimelineStorage<int>();
            Assert.DoesNotThrow(() => storage.RollBackToStart());
        }

        [Test]
        public void RollBackToStart()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackToStart();
            int storedValue = storage;
            Assert.AreEqual(0, storedValue);
        }

        [Test]
        public void RollForwardToNextValue()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker(6));

            int storedValue = storage;
            Assert.AreEqual(5, storedValue);
        }

        [Test]
        public void RollForwardToCurrentValue()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker(5));

            int storedValue = storage;
            Assert.AreEqual(4, storedValue);
        }

        [Test]
        public void RollForwardToLastValue()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)maximumValue));

            int storedValue = storage;
            Assert.AreEqual(maximumValue - 1, storedValue);
        }

        [Test]
        public void RollForwardPastLastValue()
        {
            var storage = new StandardObjectTimelineStorage<int>();

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                storage.Current = i;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(5));
            storage.RollForwardTo(new TimeMarker((ulong)(maximumValue + 1)));

            int storedValue = storage;
            Assert.AreEqual(maximumValue - 1, storedValue);
        }
    }
}
