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
    public sealed class ScheduleStorageTest
    {
        [Test]
        public void AddWithNullScheduleId()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(null, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>()));
        }

        [Test]
        public void AddWithNullSchedule()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(new ScheduleId(), null, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>()));
        }

        [Test]
        public void AddWithNullProductionVariables()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(new ScheduleId(), schedule.Object, "a", "b", "c", null, new List<IScheduleDependency>()));
        }

        [Test]
        public void AddWithNullDependencies()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(new ScheduleId(), schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), null));
        }

        [Test]
        public void Add()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var id = new ScheduleId();

            var info = collection.Add(id, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.AreSame(id, info.Id);
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(schedule.Object, collection.Schedule(info.Id));
        }

        [Test]
        public void AddWithDuplicateId()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var id = new ScheduleId();

            collection.Add(id, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.Throws<DuplicateScheduleIdException>(
                () => collection.Add(id, schedule.Object, "d", "e", "f", new List<IScheduleVariable>(), new List<IScheduleDependency>()));
        }

        [Test]
        public void AddWithDuplicate()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var firstId = new ScheduleId();
            var secondId = new ScheduleId();

            var info = collection.Add(firstId, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.AreSame(firstId, info.Id);
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(schedule.Object, collection.Schedule(info.Id));

            var otherInfo = collection.Add(secondId, schedule.Object, "d", "e", "f", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.AreSame(secondId, otherInfo.Id);
            Assert.AreSame(otherInfo, collection.Information(otherInfo.Id));
            Assert.AreSame(schedule.Object, collection.Schedule(otherInfo.Id));
        }

        [Test]
        public void UpdateWithNullId()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var id = new ScheduleId();

            var info = collection.Add(id, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            var otherSchedule = new Mock<IEditableSchedule>();
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, otherSchedule.Object));
        }

        [Test]
        public void UpdateWithUnknownId()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var id = new ScheduleId();

            var info = collection.Add(id, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            var otherSchedule = new Mock<IEditableSchedule>();
            Assert.Throws<UnknownScheduleException>(() => collection.Update(new ScheduleId(), otherSchedule.Object));
        }

        [Test]
        public void UpdateWithNullSchedule()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var id = new ScheduleId();

            var info = collection.Add(id, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.Throws<ArgumentNullException>(() => collection.Update(info.Id, null));
        }

        [Test]
        public void Update()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var id = new ScheduleId();

            var info = collection.Add(id, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.AreSame(schedule.Object, collection.Schedule(info.Id));

            var otherSchedule = new Mock<IEditableSchedule>();
            collection.Update(info.Id, otherSchedule.Object);
            var otherInfo = collection.Information(info.Id);
            Assert.AreEqual(info.Id, otherInfo.Id);
            Assert.AreEqual(info.Name, otherInfo.Name);
            Assert.AreEqual(info.Summary, otherInfo.Summary);
            Assert.AreEqual(info.Description, otherInfo.Description);
            Assert.AreElementsEqual(info.Produces(), otherInfo.Produces());
            Assert.AreElementsEqual(info.DependsOn(), otherInfo.DependsOn());

            Assert.AreSame(otherSchedule.Object, collection.Schedule(info.Id));
        }

        [Test]
        public void RemoveWithNullId()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            Assert.DoesNotThrow(() => collection.Remove(null));
        }

        [Test]
        public void Remove()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var id = new ScheduleId();

            var info = collection.Add(id, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.IsTrue(collection.Contains(info.Id));

            collection.Remove(info.Id);
            Assert.IsFalse(collection.Contains(info.Id));
        }

        [Test]
        public void Contains()
        {
            var collection = ScheduleStorage.BuildStorageWithoutTimeline();
            var schedule = new Mock<IEditableSchedule>();
            var id = new ScheduleId();

            var info = collection.Add(id, schedule.Object, "a", "b", "c", new List<IScheduleVariable>(), new List<IScheduleDependency>());
            Assert.IsTrue(collection.Contains(info.Id));
            Assert.IsFalse(collection.Contains(new ScheduleId()));
            Assert.IsFalse(collection.Contains(null));
        }
    }
}
