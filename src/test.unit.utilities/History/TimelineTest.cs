//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class TimelineTest
    {
        private sealed class MockHistoryObject : IAmHistoryEnabled, IEquatable<MockHistoryObject>
        {
            public const byte SomeValueIndex = 0;
            public const byte LotsOfValuesIndex = 1;

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

            [FieldIndexForHistoryTracking(SomeValueIndex)]
            private IVariableTimeline<int> m_SomeValue;

            [FieldIndexForHistoryTracking(LotsOfValuesIndex)]
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

            /// <summary>
            /// Determines whether the specified <see cref="MockHistoryObject"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="MockHistoryObject"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="MockHistoryObject"/> is equal to this instance;
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

        private static MockHistoryObject BuildObject(
            HistoryId id, 
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members, 
            params object[] constructorArguments)
        {
            IVariableTimeline<int> someValue = null;
            IListTimelineStorage<int> lotsOfValues = null;

            foreach (var pair in members)
            {
                if (pair.Item1 == MockHistoryObject.SomeValueIndex)
                {
                    someValue = pair.Item2 as IVariableTimeline<int>;
                    continue;
                }

                if (pair.Item1 == MockHistoryObject.LotsOfValuesIndex)
                {
                    lotsOfValues = pair.Item2 as IListTimelineStorage<int>;
                    continue;
                }
            }

            return new MockHistoryObject(id, someValue, lotsOfValues);
        }

        [Test]
        public void AddToTimelineWithoutMark()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            Assert.IsNotNull(obj);
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsTrue(timeline.DoesObjectExistCurrently(obj.HistoryId));
        }

        [Test]
        public void AddToTimelineWithMark()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();
            
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsTrue(timeline.DoesObjectExistCurrently(obj.HistoryId));
        }

        [Test]
        public void RemoveFromTimelineWithMark()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            timeline.RemoveFromTimeline(obj.HistoryId);
            timeline.Mark();

            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));
        }

        [Test]
        public void RemoveFromTimelineBeforeMark()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            timeline.RemoveFromTimeline(obj.HistoryId);
            
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));
        }

        [Test]
        public void RemoveFromTimelineWhileDeletedBeforeMark()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            timeline.RemoveFromTimeline(obj.HistoryId);
            Assert.DoesNotThrow(() => timeline.RemoveFromTimeline(obj.HistoryId));
        }

        [Test]
        public void RemoveFromTimelineWhileDeletedAfterMark()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            timeline.RemoveFromTimeline(obj.HistoryId);
            timeline.Mark();

            Assert.DoesNotThrow(() => timeline.RemoveFromTimeline(obj.HistoryId));
        }

        [Test]
        public void RemoveFromTimelineWithUnknownId()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            Assert.DoesNotThrow(() => timeline.RemoveFromTimeline(new HistoryId()));
        }

        [Test]
        public void DeleteFromTimelineAtSameTimeAsCreation()
        { 
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline<MockHistoryObject>(BuildObject);
            timeline.RemoveFromTimeline(obj.HistoryId);

            Assert.IsFalse(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));
        }

        [Test]
        public void RollBackToPreviousStep()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            var objectChangeMarkers = new List<TimeMarker>();
            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                obj.SomeValue = i;
                obj.LotsOfValues.Add(i);
                var marker = timeline.Mark();
                objectChangeMarkers.Add(marker);
            }

            for (int i = maximumValue - 1; i >= 0; i--)
            {
                timeline.RollBackTo(objectChangeMarkers[i]);
                Assert.AreEqual(i, obj.SomeValue);

                Assert.AreEqual(i + 1, obj.LotsOfValues.Count);
                
                var expected = Enumerable.Range(0, i + 1);
                Assert.AreElementsEqual(expected, obj.LotsOfValues);
            }
        }

        [Test]
        public void RollBackToBeginning()
        {
            var timeline = new Timeline(BuildStorageOfType);
            var firstMark = timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            var objectChangeMarkers = new List<TimeMarker>();
            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                obj.SomeValue = i;
                obj.LotsOfValues.Add(i);
                var marker = timeline.Mark();
                objectChangeMarkers.Add(marker);
            }

            timeline.RollBackTo(firstMark);
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));

            // Remember that the obj has been cut from the timeline but because
            // we have a reference to it, it's not dead yet. However the storage
            // should have been rolled back
            Assert.AreEqual(0, obj.SomeValue);
            Assert.AreEqual(0, obj.LotsOfValues.Count);
        }

        [Test]
        public void RollBackToBeforeBeginning()
        { 
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            var objectChangeMarkers = new List<TimeMarker>();
            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                obj.SomeValue = i;
                obj.LotsOfValues.Add(i);
                var marker = timeline.Mark();
                objectChangeMarkers.Add(marker);
            }

            timeline.RollBackTo(TimeMarker.TheBeginOfTime);
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));

            // Remember that the obj has been cut from the timeline but because
            // we have a reference to it, it's not dead yet. However the storage
            // should have been rolled back
            Assert.AreEqual(0, obj.SomeValue);
            Assert.AreEqual(0, obj.LotsOfValues.Count);
        }

        [Test]
        public void RollBackFromDeath()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            int maximumValue = 10;
            obj.SomeValue = maximumValue;
            for (int i = 0; i < maximumValue; i++)
            {
                obj.LotsOfValues.Add(i);
            }

            var changeMarker = timeline.Mark();

            timeline.RemoveFromTimeline(obj.HistoryId);
            timeline.Mark();

            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));

            timeline.RollBackTo(changeMarker);
            var restoredObj = timeline.IdToObject<MockHistoryObject>(obj.HistoryId);

            Assert.IsNotNull(restoredObj);
            Assert.AreNotSame(obj, restoredObj);
            Assert.AreEqual(obj.HistoryId, restoredObj.HistoryId);
            Assert.AreEqual(maximumValue, restoredObj.SomeValue);
            Assert.AreEqual(maximumValue, restoredObj.LotsOfValues.Count);
        }

        [Test]
        public void RollForwardToNextValue()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            var creationTime = timeline.Mark();

            var objectChangeMarkers = new List<TimeMarker>();
            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                obj.SomeValue = i;
                obj.LotsOfValues.Add(i);
                var marker = timeline.Mark();
                objectChangeMarkers.Add(marker);
            }

            timeline.RollBackTo(creationTime);

            for (int i = 0; i < maximumValue; i++)
            {
                timeline.RollForwardTo(objectChangeMarkers[i]);
                Assert.AreEqual(i, obj.SomeValue);

                var expected = Enumerable.Range(0, i + 1);
                Assert.AreElementsEqual(expected, obj.LotsOfValues);
            }
        }

        [Test]
        public void RollForwardToTheEnd()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            var creationTime = timeline.Mark();

            var objectChangeMarkers = new List<TimeMarker>();
            int maximumValue = 10;
            for (int i = 0; i < maximumValue; i++)
            {
                obj.SomeValue = i;
                obj.LotsOfValues.Add(i);
                var marker = timeline.Mark();
                objectChangeMarkers.Add(marker);
            }

            timeline.RollBackTo(creationTime);
            timeline.RollForwardTo(new TimeMarker(100));

            Assert.AreEqual(maximumValue - 1, obj.SomeValue);

            var expected = Enumerable.Range(0, maximumValue);
            Assert.AreElementsEqual(expected, obj.LotsOfValues);
        }

        [Test]
        public void RollForwardPastDeath()
        {
            var timeline = new Timeline(BuildStorageOfType);
            var firstMark = timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            var secondMark = timeline.Mark();

            timeline.RemoveFromTimeline(obj.HistoryId);
            var thirdMark = timeline.Mark();

            timeline.RollBackTo(firstMark);
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));

            timeline.RollForwardTo(secondMark);
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsTrue(timeline.DoesObjectExistCurrently(obj.HistoryId));

            timeline.RollForwardTo(thirdMark);
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));
        }

        [Test]
        public void RollForwardWithLocalChange()
        {
            var timeline = new Timeline(BuildStorageOfType);
            timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            obj.SomeValue = 1;
            obj.LotsOfValues.Add(2);
            var firstChange = timeline.Mark();

            obj.SomeValue = 2;
            obj.LotsOfValues.Add(4);
            var secondChange = timeline.Mark();

            timeline.RollBackTo(firstChange);
            obj.SomeValue = 3;
            obj.LotsOfValues.Add(8);
            var otherObj = timeline.AddToTimeline(BuildObject);

            timeline.RollForwardTo(secondChange);
            Assert.AreEqual(2, obj.SomeValue);
            Assert.AreElementsEqual(new[] { 2, 4 }, obj.LotsOfValues);
            Assert.IsFalse(timeline.DoesObjectExistCurrently(otherObj.HistoryId));
            Assert.IsFalse(timeline.HasObjectEverExisted(otherObj.HistoryId));
        }

        [Test]
        public void MarkWithNoObjects()
        {
            var timeline = new Timeline(BuildStorageOfType);
            var firstMark = timeline.Mark();
            Assert.GreaterThan(firstMark, TimeMarker.TheBeginOfTime);
            Assert.AreEqual(TimeMarker.TheBeginOfTime.Next(), firstMark);
        }

        [Test]
        public void MarkClearsForwardHistory()
        {
            var timeline = new Timeline(BuildStorageOfType);
            var firstMark = timeline.Mark();

            var obj = timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            timeline.RemoveFromTimeline(obj.HistoryId);
            var thirdMark = timeline.Mark();

            timeline.RollBackTo(firstMark);
            Assert.IsTrue(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));

            timeline.AddToTimeline(BuildObject);
            timeline.Mark();

            Assert.IsFalse(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));

            timeline.RollForwardTo(thirdMark);
            Assert.IsFalse(timeline.HasObjectEverExisted(obj.HistoryId));
            Assert.IsFalse(timeline.DoesObjectExistCurrently(obj.HistoryId));
        }
    }
}
