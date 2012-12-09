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
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Dataset.Scheduling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleConditionStorageTest
    {
        [Test]
        public void AddWithNullCondition()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(null, "a", "b"));
        }

        [Test]
        public void Add()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b");
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(condition.Object, collection.Condition(info.Id));
        }

        [Test]
        public void AddWithDuplicate()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b");
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(condition.Object, collection.Condition(info.Id));

            var otherInfo = collection.Add(condition.Object, "d", "e");
            Assert.AreSame(otherInfo, collection.Information(otherInfo.Id));
            Assert.AreSame(condition.Object, collection.Condition(otherInfo.Id));
        }

        [Test]
        public void UpdateWithNullId()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b");
            var otherCondition = new Mock<IScheduleCondition>();
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, otherCondition.Object));
        }

        [Test]
        public void UpdateWithUnknownId()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b");
            var otherCondition = new Mock<IScheduleCondition>();
            Assert.Throws<UnknownScheduleConditionException>(() => collection.Update(new ScheduleElementId(), otherCondition.Object));
        }

        [Test]
        public void UpdateWithNullAction()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b");
            Assert.Throws<ArgumentNullException>(() => collection.Update(info.Id, null));
        }

        [Test]
        public void Update()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b");
            Assert.AreSame(condition.Object, collection.Condition(info.Id));

            var otherCondition = new Mock<IScheduleCondition>();
            collection.Update(info.Id, otherCondition.Object);
            var otherInfo = collection.Information(info.Id);
            Assert.AreEqual(info.Id, otherInfo.Id);
            Assert.AreEqual(info.Name, otherInfo.Name);
            Assert.AreEqual(info.Description, otherInfo.Description);

            Assert.AreSame(otherCondition.Object, collection.Condition(info.Id));
        }

        [Test]
        public void RemoveWithNullId()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            Assert.DoesNotThrow(() => collection.Remove(null));
        }

        [Test]
        public void Remove()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b");
            Assert.IsTrue(collection.Contains(info.Id));

            collection.Remove(info.Id);
            Assert.IsFalse(collection.Contains(info.Id));
        }

        [Test]
        public void Contains()
        {
            var collection = ScheduleConditionStorage.CreateInstanceWithoutTimeline();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b");
            Assert.IsTrue(collection.Contains(info.Id));
            Assert.IsFalse(collection.Contains(new ScheduleElementId()));
            Assert.IsFalse(collection.Contains(null));
        }
    }
}
