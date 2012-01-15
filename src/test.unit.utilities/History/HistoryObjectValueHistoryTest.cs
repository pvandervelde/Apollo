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
    public sealed class HistoryObjectValueHistoryTest
    {
        private sealed class MockHistoryObject : IAmHistoryEnabled
        {
            private readonly HistoryId m_Id;

            public MockHistoryObject(long id)
            {
                m_Id = new HistoryId(id);
            }

            public HistoryId HistoryId
            {
                get 
                {
                    return m_Id;
                }
            }
        }

        [Test]
        public void RollBackToPreviousValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => objects[id];
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            for (int i = maximumValue - 1; i >= 0; i--)
            {
                storage.RollBackTo(new TimeMarker((ulong)(i + 1)));
                MockHistoryObject storedValue = storage;
                Assert.AreSame(objects[new HistoryId(i)], storedValue);
            }
        }

        [Test]
        public void RollBackToToCurrentValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => objects[id];
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker((ulong)(maximumValue + 1)));
            MockHistoryObject storedValue = storage;
            Assert.AreSame(objects[new HistoryId(maximumValue - 1)], storedValue);
        }

        [Test]
        public void RollBackToFirstValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => objects[id];
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(1));
            MockHistoryObject storedValue = storage;
            Assert.AreSame(objects[new HistoryId(0)], storedValue);
        }

        [Test]
        public void RollBackPastFirstValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => (id != null) ? objects[id] : null;
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(0));
            MockHistoryObject storedValue = storage;
            Assert.AreEqual(null, storedValue);
        }

        [Test]
        public void RollBackToStartWithNoValues()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => objects[id];
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            Assert.DoesNotThrow(() => storage.RollBackToStart());
        }

        [Test]
        public void RollBackToStart()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => (id != null) ? objects[id] : null;
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackToStart();
            MockHistoryObject storedValue = storage;
            Assert.AreEqual(null, storedValue);
        }

        [Test]
        public void RollForwardToNextValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => objects[id];
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker(7));
            MockHistoryObject storedValue = storage;
            Assert.AreSame(objects[new HistoryId(6)], storedValue);
        }

        [Test]
        public void RollForwardToCurrentValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => objects[id];
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker(6));
            MockHistoryObject storedValue = storage;
            Assert.AreSame(objects[new HistoryId(5)], storedValue);
        }

        [Test]
        public void RollForwardToLastValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => objects[id];
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker((ulong)maximumValue));
            MockHistoryObject storedValue = storage;
            Assert.AreSame(objects[new HistoryId(maximumValue - 1)], storedValue);
        }

        [Test]
        public void RollForwardPastLastValue()
        {
            var objects = new Dictionary<HistoryId, MockHistoryObject>();
            Func<HistoryId, MockHistoryObject> lookupFunc =
                id => objects[id];
            var storage = new HistoryObjectValueHistory<MockHistoryObject>(lookupFunc);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                var obj = new MockHistoryObject(i);
                objects.Add(obj.HistoryId, obj);
                storage.Current = obj;
                storage.StoreCurrent(new TimeMarker((ulong)(i + 1)));
            }

            storage.RollBackTo(new TimeMarker(6));
            storage.RollForwardTo(new TimeMarker((ulong)(maximumValue + 1)));
            MockHistoryObject storedValue = storage;
            Assert.AreSame(objects[new HistoryId(maximumValue - 1)], storedValue);
        }
    }
}
