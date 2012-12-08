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
        public void AddWithNullSchedule()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(null, "a", "b"));
        }

        [Test]
        public void Add()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            var schedule = new Mock<ISchedule>();

            var info = collection.Add(schedule.Object, "a", "b");
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(schedule.Object, collection.Schedule(info.Id));
        }

        [Test]
        public void AddWithDuplicate()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            var schedule = new Mock<ISchedule>();

            var info = collection.Add(schedule.Object, "a", "b");
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(schedule.Object, collection.Schedule(info.Id));

            var otherInfo = collection.Add(schedule.Object, "d", "e");
            Assert.AreSame(otherInfo, collection.Information(otherInfo.Id));
            Assert.AreSame(schedule.Object, collection.Schedule(otherInfo.Id));
        }

        [Test]
        public void UpdateWithNullId()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            var schedule = new Mock<ISchedule>();

            var info = collection.Add(schedule.Object, "a", "b");
            var otherSchedule = new Mock<ISchedule>();
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, otherSchedule.Object));
        }

        [Test]
        public void UpdateWithUnknownId()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            var schedule = new Mock<ISchedule>();

            var info = collection.Add(schedule.Object, "a", "b");
            var otherSchedule = new Mock<ISchedule>();
            Assert.Throws<UnknownScheduleException>(() => collection.Update(new ScheduleId(), otherSchedule.Object));
        }

        [Test]
        public void UpdateWithNullSchedule()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            var schedule = new Mock<ISchedule>();

            var info = collection.Add(schedule.Object, "a", "b");
            Assert.Throws<ArgumentNullException>(() => collection.Update(info.Id, null));
        }

        [Test]
        public void Update()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            var schedule = new Mock<ISchedule>();

            var info = collection.Add(schedule.Object, "a", "b");
            Assert.AreSame(schedule.Object, collection.Schedule(info.Id));

            var otherSchedule = new Mock<ISchedule>();
            collection.Update(info.Id, otherSchedule.Object);
            var otherInfo = collection.Information(info.Id);
            Assert.AreEqual(info.Id, otherInfo.Id);
            Assert.AreEqual(info.Name, otherInfo.Name);
            Assert.AreEqual(info.Description, otherInfo.Description);

            Assert.AreSame(otherSchedule.Object, collection.Schedule(info.Id));
        }

        [Test]
        public void RemoveWithNullId()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            Assert.DoesNotThrow(() => collection.Remove(null));
        }

        [Test]
        public void Remove()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            var schedule = new Mock<ISchedule>();

            var info = collection.Add(schedule.Object, "a", "b");
            Assert.IsTrue(collection.Contains(info.Id));

            collection.Remove(info.Id);
            Assert.IsFalse(collection.Contains(info.Id));
        }

        [Test]
        public void Contains()
        {
            var collection = ScheduleStorage.CreateInstanceWithoutTimeline();
            var schedule = new Mock<ISchedule>();

            var info = collection.Add(schedule.Object, "a", "b");
            Assert.IsTrue(collection.Contains(info.Id));
            Assert.IsFalse(collection.Contains(new ScheduleId()));
            Assert.IsFalse(collection.Contains(null));
        }
    }
}
