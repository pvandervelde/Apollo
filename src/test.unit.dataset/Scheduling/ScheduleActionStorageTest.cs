//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleActionStorageTest
    {
        [Test]
        public void AddWithNullAction()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(null, "a", "b"));
        }

        [Test]
        public void Add()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var action = new Mock<IScheduleAction>(); 

            var info = collection.Add(action.Object, "a", "b");
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(action.Object, collection.Action(info.Id));
        }

        [Test]
        public void AddWithDuplicate()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            var info = collection.Add(action.Object, "a", "b");
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(action.Object, collection.Action(info.Id));

            var otherInfo = collection.Add(action.Object, "d", "e");
            Assert.AreSame(otherInfo, collection.Information(otherInfo.Id));
            Assert.AreSame(action.Object, collection.Action(otherInfo.Id));
        }

        [Test]
        public void UpdateWithNullId()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            var info = collection.Add(action.Object, "a", "b");
            var otherAction = new Mock<IScheduleAction>();
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, otherAction.Object));
        }

        [Test]
        public void UpdateWithUnknownId()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            var info = collection.Add(action.Object, "a", "b");
            var otherAction = new Mock<IScheduleAction>();
            Assert.Throws<UnknownScheduleActionException>(() => collection.Update(new ScheduleElementId(), otherAction.Object));
        }

        [Test]
        public void UpdateWithNullAction()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            var info = collection.Add(action.Object, "a", "b");
            Assert.Throws<ArgumentNullException>(() => collection.Update(info.Id, null));
        }

        [Test]
        public void Update()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            var info = collection.Add(action.Object, "a", "b");
            Assert.AreSame(action.Object, collection.Action(info.Id));

            var otherAction = new Mock<IScheduleAction>();
            collection.Update(info.Id, otherAction.Object);
            var otherInfo = collection.Information(info.Id);
            Assert.AreEqual(info.Id, otherInfo.Id);
            Assert.AreEqual(info.Name, otherInfo.Name);
            Assert.AreEqual(info.Description, otherInfo.Description);

            Assert.AreSame(otherAction.Object, collection.Action(info.Id));
        }

        [Test]
        public void RemoveWithNullId()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            Assert.DoesNotThrow(() => collection.Remove(null));
        }

        [Test]
        public void Remove()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            var info = collection.Add(action.Object, "a", "b");
            Assert.IsTrue(collection.Contains(info.Id));
            
            collection.Remove(info.Id);
            Assert.IsFalse(collection.Contains(info.Id));
        }

        [Test]
        public void Contains()
        {
            var collection = ScheduleActionStorage.CreateInstanceWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            var info = collection.Add(action.Object, "a", "b");
            Assert.IsTrue(collection.Contains(info.Id));
            Assert.IsFalse(collection.Contains(new ScheduleElementId()));
            Assert.IsFalse(collection.Contains(null));
        }
    }
}
