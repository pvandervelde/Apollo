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
        public void AddWithNullActionId()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(null, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>()));
        }

        [Test]
        public void AddWithNullAction()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(new ScheduleElementId(), null, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>()));
        }

        [Test]
        public void AddWithNullProductionVariables()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(new ScheduleElementId(), action.Object, "a", "b", "c", null, new List<IScheduleDependency>()));
        }

        [Test]
        public void AddWithNullDependencies()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(new ScheduleElementId(), action.Object, "a", "b", "c", new List<IScheduleVariable>(), null));
        }

        [Test]
        public void Add()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>(); 
            var id = new ScheduleElementId();

            var info = collection.Add(id, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.AreSame(id, info.Id);
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(action.Object, collection.Action(info.Id));
        }

        [Test]
        public void AddWithDuplicate()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();
            var firstId = new ScheduleElementId();
            var secondId = new ScheduleElementId();

            var info = collection.Add(firstId, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.AreSame(firstId, info.Id);
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(action.Object, collection.Action(info.Id));

            var otherInfo = collection.Add(secondId, action.Object, "d", "e", "f", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.AreSame(secondId, otherInfo.Id);
            Assert.AreSame(otherInfo, collection.Information(otherInfo.Id));
            Assert.AreSame(action.Object, collection.Action(otherInfo.Id));
        }

        [Test]
        public void UpdateWithNullId()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();
            var id = new ScheduleElementId();

            var info = collection.Add(id, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            var otherAction = new Mock<IScheduleAction>();
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, otherAction.Object));
        }

        [Test]
        public void UpdateWithUnknownId()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();
            var id = new ScheduleElementId();

            var info = collection.Add(id, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            var otherAction = new Mock<IScheduleAction>();
            Assert.Throws<UnknownScheduleActionException>(() => collection.Update(new ScheduleElementId(), otherAction.Object));
        }

        [Test]
        public void UpdateWithNullAction()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();
            var id = new ScheduleElementId();

            var info = collection.Add(id, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.Throws<ArgumentNullException>(() => collection.Update(info.Id, null));
        }

        [Test]
        public void Update()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();
            var id = new ScheduleElementId();

            var info = collection.Add(id, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.AreSame(action.Object, collection.Action(info.Id));

            var otherAction = new Mock<IScheduleAction>();
            collection.Update(info.Id, otherAction.Object);
            var otherInfo = collection.Information(info.Id);
            Assert.AreEqual(info.Id, otherInfo.Id);
            Assert.AreEqual(info.Name, otherInfo.Name);
            Assert.AreEqual(info.Summary, otherInfo.Summary);
            Assert.AreEqual(info.Description, otherInfo.Description);
            Assert.AreElementsEqual(info.Produces(), otherInfo.Produces());
            Assert.AreElementsEqual(info.DependsOn(), otherInfo.DependsOn());

            Assert.AreSame(otherAction.Object, collection.Action(info.Id));
        }

        [Test]
        public void RemoveWithNullId()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            Assert.DoesNotThrow(() => collection.Remove(null));
        }

        [Test]
        public void Remove()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();
            var id = new ScheduleElementId();

            var info = collection.Add(id, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.IsTrue(collection.Contains(info.Id));
            
            collection.Remove(info.Id);
            Assert.IsFalse(collection.Contains(info.Id));
        }

        [Test]
        public void Contains()
        {
            var collection = ScheduleActionStorage.BuildStorageWithoutTimeline();
            var action = new Mock<IScheduleAction>();
            var id = new ScheduleElementId();

            var info = collection.Add(id, action.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.IsTrue(collection.Contains(info.Id));
            Assert.IsFalse(collection.Contains(new ScheduleElementId()));
            Assert.IsFalse(collection.Contains(null));
        }
    }
}
