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
    public sealed class ObjectTimelineTest
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

            private IVariableTimeline<int> m_SomeValue;

            private IListTimelineStorage<int> m_LotsOfValues;

            public MockHistoryObject(HistoryId id, IVariableTimeline<int> valueStorage, IListTimelineStorage<int> collectionStorage)
            {
                m_Id = id;
                m_SomeValue = valueStorage;
                m_LotsOfValues = collectionStorage;
            }

            public HistoryId HistoryId
            {
                get
                {
                    return m_Id;
                }
            }

            public int SomeValue
            {
                get
                {
                    return m_SomeValue.Current;
                }

                set
                {
                    m_SomeValue.Current = value;
                }
            }

            public IList<int> LotsOfValues
            {
                get
                {
                    return m_LotsOfValues;
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

        private static IStoreTimelineValues BuildStorageOfType(Type storageType)
        {
            if (typeof(IVariableTimeline<int>).IsAssignableFrom(storageType))
            {
                return new ValueHistory<int>();
            }

            if (typeof(IListTimelineStorage<int>).IsAssignableFrom(storageType))
            {
                return new ListHistory<int>();
            }

            // Dunno what that is ...
            throw new ArgumentException();
        }

        private static MockHistoryObject BuildObject(HistoryId id, IEnumerable<Tuple<string, IStoreTimelineValues>> members)
        {
            IVariableTimeline<int> someValue = null;
            IListTimelineStorage<int> lotsOfValues = null;

            foreach (var pair in members)
            {
                if (string.Equals(pair.Item1, "m_SomeValue"))
                {
                    someValue = pair.Item2 as IVariableTimeline<int>;
                    continue;
                }

                if (string.Equals(pair.Item1, "m_LotsOfValues"))
                {
                    lotsOfValues = pair.Item2 as IListTimelineStorage<int>;
                    continue;
                }
            }

            return new MockHistoryObject(id, someValue, lotsOfValues);
        }

        [Test]
        public void AddToTimeline()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            Assert.IsFalse(timeline.IsAlive());

            timeline.AddToTimeline();
            var marker = new TimeMarker(1);
            timeline.Mark(marker);

            Assert.IsTrue(timeline.IsAlive());
            Assert.AreEqual(marker, timeline.CreationTime);
            Assert.IsNull(timeline.DeletionTime);

            var obj = timeline.Object;
            Assert.AreEqual(0, obj.SomeValue);
            Assert.AreEqual(0, obj.LotsOfValues.Count);
        }

        [Test]
        public void AddToTimelineWithExistingObject()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            timeline.AddToTimeline();
            var marker = new TimeMarker(1);
            timeline.Mark(marker);

            Assert.Throws<ObjectHasAlreadyBeenAddedToTheTimelineException>(() => timeline.AddToTimeline());
        }

        [Test]
        public void DeleteFromTimeline()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            timeline.AddToTimeline();
            var creationTime = new TimeMarker(1);
            timeline.Mark(creationTime);

            Assert.IsTrue(timeline.IsAlive());
            Assert.AreEqual(creationTime, timeline.CreationTime);
            Assert.IsNull(timeline.DeletionTime);
            Assert.IsNotNull(timeline.Object);

            timeline.DeleteFromTimeline();
            var deletionTime = new TimeMarker(2);
            timeline.Mark(deletionTime);

            Assert.IsFalse(timeline.IsAlive());
            Assert.AreEqual(creationTime, timeline.CreationTime);
            Assert.AreEqual(deletionTime, timeline.DeletionTime);
            Assert.IsNull(timeline.Object);
        }

        [Test]
        public void DeleteFromTimelineWhileDeleted()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            timeline.AddToTimeline();
            var creationTime = new TimeMarker(1);
            timeline.Mark(creationTime);

            timeline.DeleteFromTimeline();
            var deletionTime = new TimeMarker(2);
            timeline.Mark(deletionTime);

            Assert.Throws<CannotRemoveNonLivingObjectException>(() => timeline.DeleteFromTimeline());
        }

        [Test]
        public void DeleteFromTimelineWhileNotCreated()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            Assert.Throws<ObjectHasNotBeenCreatedYetException>(() => timeline.DeleteFromTimeline());
        }

        [Test]
        public void RollBackToPreviousValue()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            int maximumValue = 10;
            for (int i = 1; i < maximumValue; i++)
            {
                timeline.Object.SomeValue = i;
                timeline.Object.LotsOfValues.Add(i);
                timeline.Mark(new TimeMarker((ulong)(i + 1)));
            }

            for (int i = maximumValue - 1; i > 0; i--)
            {
                timeline.RollBackTo(new TimeMarker((ulong)(i + 1)));
                Assert.AreEqual(i, timeline.Object.SomeValue);

                Assert.AreEqual(i, timeline.Object.LotsOfValues.Count);
                for (int j = 1; j <= i; j++)
                {
                    Assert.IsTrue(timeline.Object.LotsOfValues.Contains(j));
                }
            }
        }

        [Test]
        public void RollBackToBeginning()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);
            
            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                timeline.Object.SomeValue = i;
                timeline.Object.LotsOfValues.Add(i);
                timeline.Mark(new TimeMarker((ulong)(i + 2)));
            }

            timeline.RollBackTo(new TimeMarker(1));
            Assert.IsTrue(timeline.IsAlive());
            Assert.IsNotNull(timeline.Object);
            Assert.AreEqual(0, timeline.Object.SomeValue);
            Assert.AreEqual(0, timeline.Object.LotsOfValues.Count);
        }

        [Test]
        public void RollBackToBeforeBeginning()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                timeline.Object.SomeValue = i;
                timeline.Object.LotsOfValues.Add(i);
                timeline.Mark(new TimeMarker((ulong)(i + 2)));
            }

            timeline.RollBackTo(new TimeMarker(0));
            Assert.IsFalse(timeline.IsAlive());
            Assert.IsNull(timeline.Object);
        }

        [Test]
        public void RollBackWhileNotExisting()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            Assert.DoesNotThrow(() => timeline.RollBackTo(new TimeMarker(0)));
        }

        [Test]
        public void RollBackToAfterDeath()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            var deletionTime = new TimeMarker(2);
            timeline.DeleteFromTimeline();
            timeline.Mark(deletionTime);

            Assert.DoesNotThrow(() => timeline.RollBackTo(new TimeMarker(3)));
            Assert.IsFalse(timeline.IsAlive());
            Assert.IsNull(timeline.Object);
        }

        [Test]
        public void RollBackFromDeathToLife()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            timeline.Object.SomeValue = 1;
            timeline.Object.LotsOfValues.Add(2);
            timeline.Mark(new TimeMarker(2));

            var deletionTime = new TimeMarker(3);
            timeline.DeleteFromTimeline();
            timeline.Mark(deletionTime);

            Assert.DoesNotThrow(() => timeline.RollBackTo(new TimeMarker(2)));
            Assert.IsTrue(timeline.IsAlive());
            Assert.IsNotNull(timeline.Object);
            Assert.AreEqual(1, timeline.Object.SomeValue);
            Assert.AreElementsEqual(new int[] { 2 }, timeline.Object.LotsOfValues);
        }

        [Test]
        public void RollBackFromDeathPastCreation()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            var deletionTime = new TimeMarker(2);
            timeline.DeleteFromTimeline();
            timeline.Mark(deletionTime);

            Assert.DoesNotThrow(() => timeline.RollBackTo(new TimeMarker(0)));
            Assert.IsFalse(timeline.IsAlive());
            Assert.IsNull(timeline.Object);
        }

        [Test]
        public void RollForwardToNextValue()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            timeline.Object.SomeValue = 1;
            timeline.Object.LotsOfValues.Add(2);
            timeline.Mark(new TimeMarker(2));

            timeline.RollBackTo(new TimeMarker(1));
            Assert.AreEqual(0, timeline.Object.SomeValue);
            Assert.AreEqual(0, timeline.Object.LotsOfValues.Count);

            timeline.RollForwardTo(new TimeMarker(2));
            Assert.AreEqual(1, timeline.Object.SomeValue);
            Assert.AreElementsEqual(new int[] { 2 }, timeline.Object.LotsOfValues);
        }

        [Test]
        public void RollForwardPastDeath()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            timeline.Object.SomeValue = 1;
            timeline.Object.LotsOfValues.Add(2);
            timeline.Mark(new TimeMarker(2));

            var deletionTime = new TimeMarker(3);
            timeline.DeleteFromTimeline();
            timeline.Mark(deletionTime);

            timeline.RollBackTo(new TimeMarker(1));
            Assert.IsTrue(timeline.IsAlive());

            timeline.RollForwardTo(new TimeMarker(4));
            Assert.IsFalse(timeline.IsAlive());
            Assert.IsNull(timeline.Object);
        }

        [Test]
        public void RollForwardWhileDead()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            var deletionTime = new TimeMarker(2);
            timeline.DeleteFromTimeline();
            timeline.Mark(deletionTime);

            Assert.DoesNotThrow(() => timeline.RollForwardTo(new TimeMarker(3)));
            Assert.IsFalse(timeline.IsAlive());
            Assert.IsNull(timeline.Object);
        }

        [Test]
        public void RollForwardToAfterCreation()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(2);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            timeline.Object.SomeValue = 1;
            timeline.Object.LotsOfValues.Add(2);
            timeline.Mark(new TimeMarker(3));

            timeline.RollBackTo(new TimeMarker(1));
            Assert.IsFalse(timeline.IsAlive());

            timeline.RollForwardTo(new TimeMarker(3));
            Assert.AreEqual(1, timeline.Object.SomeValue);
            Assert.AreElementsEqual(new int[] { 2 }, timeline.Object.LotsOfValues);
        }

        [Test]
        public void RollForwardFromBeforeCreationToPastDeath()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(2);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            var deletionTime = new TimeMarker(3);
            timeline.DeleteFromTimeline();
            timeline.Mark(deletionTime);

            timeline.RollBackTo(new TimeMarker(1));
            Assert.IsFalse(timeline.IsAlive());
            Assert.IsNull(timeline.Object);

            timeline.RollForwardTo(new TimeMarker(4));
            Assert.IsFalse(timeline.IsAlive());
            Assert.IsNull(timeline.Object);
        }

        [Test]
        public void RollForwardWithLocalChange()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            var creationTime = new TimeMarker(1);
            timeline.AddToTimeline();
            timeline.Mark(creationTime);

            timeline.Object.SomeValue = 1;
            timeline.Object.LotsOfValues.Add(2);
            timeline.Mark(new TimeMarker(2));

            timeline.Object.SomeValue = 2;
            timeline.Object.LotsOfValues.Add(4);
            timeline.Mark(new TimeMarker(3));

            timeline.RollBackTo(new TimeMarker(2));
            timeline.Object.SomeValue = 3;
            timeline.Object.LotsOfValues.Add(8);

            timeline.RollForwardTo(new TimeMarker(3));
            Assert.AreEqual(2, timeline.Object.SomeValue);
            Assert.AreElementsEqual(new int[] { 2, 4 }, timeline.Object.LotsOfValues);
        }

        [Test]
        public void CreateClearsForwardHistory()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            timeline.AddToTimeline();
            timeline.Mark(new TimeMarker(2));

            timeline.Object.SomeValue = 1;
            timeline.Object.LotsOfValues.Add(2);
            timeline.Mark(new TimeMarker(3));

            timeline.DeleteFromTimeline();
            timeline.Mark(new TimeMarker(4));

            timeline.RollBackTo(new TimeMarker(0));
            timeline.AddToTimeline();
            timeline.Mark(new TimeMarker(1));
            timeline.RollForwardTo(new TimeMarker(10));

            Assert.IsTrue(timeline.IsAlive());
            Assert.AreEqual(new TimeMarker(1), timeline.CreationTime);
            Assert.IsNull(timeline.DeletionTime);

            Assert.AreEqual(0, timeline.Object.SomeValue);
            Assert.AreEqual(0, timeline.Object.LotsOfValues.Count);
        }

        [Test]
        public void DeleteClearsForwardHistory()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            timeline.AddToTimeline();
            timeline.Mark(new TimeMarker(2));

            timeline.Object.SomeValue = 1;
            timeline.Object.LotsOfValues.Add(2);
            timeline.Mark(new TimeMarker(3));

            timeline.DeleteFromTimeline();
            timeline.Mark(new TimeMarker(4));

            timeline.RollBackTo(new TimeMarker(2));
            timeline.DeleteFromTimeline();
            timeline.Mark(new TimeMarker(3));

            timeline.RollBackTo(new TimeMarker(2));
            Assert.IsTrue(timeline.IsAlive());
            Assert.AreEqual(new TimeMarker(2), timeline.CreationTime);
            Assert.AreEqual(new TimeMarker(3), timeline.DeletionTime);

            Assert.AreEqual(0, timeline.Object.SomeValue);
            Assert.AreEqual(0, timeline.Object.LotsOfValues.Count);
        }

        [Test]
        public void MarkClearsForwardHistory()
        {
            var id = new HistoryId();
            var timeline = new ObjectTimeline<MockHistoryObject>(id, BuildStorageOfType, BuildObject);

            timeline.AddToTimeline();
            timeline.Mark(new TimeMarker(2));

            timeline.Object.SomeValue = 1;
            timeline.Object.LotsOfValues.Add(2);
            timeline.Mark(new TimeMarker(3));

            timeline.DeleteFromTimeline();
            timeline.Mark(new TimeMarker(4));

            timeline.RollBackTo(new TimeMarker(2));

            timeline.Object.SomeValue = 2;
            timeline.Object.LotsOfValues.Add(3);
            timeline.Mark(new TimeMarker(3));

            timeline.RollForwardTo(new TimeMarker(10));
            Assert.AreEqual(2, timeline.Object.SomeValue);
            Assert.AreElementsEqual(new int[] { 3 }, timeline.Object.LotsOfValues);
        }
    }
}
