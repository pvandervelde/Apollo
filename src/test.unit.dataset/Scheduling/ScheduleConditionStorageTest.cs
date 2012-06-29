﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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
            var collection = new ScheduleConditionStorage();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(null, "a", "b", "c", new List<IScheduleDependency>()));
        }

        [Test]
        public void AddWithNullDependencies()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            Assert.Throws<ArgumentNullException>(
                () => collection.Add(condition.Object, "a", "b", "c", null));
        }

        [Test]
        public void Add()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b", "c", new List<IScheduleDependency>());
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(condition.Object, collection.Condition(info.Id));
        }

        [Test]
        public void AddWithDuplicate()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b", "c", new List<IScheduleDependency>());
            Assert.AreSame(info, collection.Information(info.Id));
            Assert.AreSame(condition.Object, collection.Condition(info.Id));

            var otherInfo = collection.Add(condition.Object, "d", "e", "f", new List<IScheduleDependency>());
            Assert.AreSame(otherInfo, collection.Information(otherInfo.Id));
            Assert.AreSame(condition.Object, collection.Condition(otherInfo.Id));
        }

        [Test]
        public void UpdateWithNullId()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b", "c", new List<IScheduleDependency>());
            var otherCondition = new Mock<IScheduleCondition>();
            Assert.Throws<ArgumentNullException>(() => collection.Update(null, otherCondition.Object));
        }

        [Test]
        public void UpdateWithUnknownId()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b", "c", new List<IScheduleDependency>());
            var otherCondition = new Mock<IScheduleCondition>();
            Assert.Throws<UnknownScheduleActionException>(() => collection.Update(new ScheduleElementId(), otherCondition.Object));
        }

        [Test]
        public void UpdateWithNullAction()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b", "c", new List<IScheduleDependency>());
            Assert.Throws<ArgumentNullException>(() => collection.Update(info.Id, null));
        }

        [Test]
        public void Update()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b", "c", new List<IScheduleDependency>());
            Assert.AreSame(condition.Object, collection.Condition(info.Id));

            var otherCondition = new Mock<IScheduleCondition>();
            collection.Update(info.Id, otherCondition.Object);
            var otherInfo = collection.Information(info.Id);
            Assert.AreEqual(info.Id, otherInfo.Id);
            Assert.AreEqual(info.Name, otherInfo.Name);
            Assert.AreEqual(info.Summary, otherInfo.Summary);
            Assert.AreEqual(info.Description, otherInfo.Description);
            Assert.AreElementsEqual(info.DependsOn(), otherInfo.DependsOn());

            Assert.AreSame(otherCondition.Object, collection.Condition(info.Id));
        }

        [Test]
        public void RemoveWithNullId()
        {
            var collection = new ScheduleConditionStorage();
            Assert.DoesNotThrow(() => collection.Remove(null));
        }

        [Test]
        public void Remove()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b", "c", new List<IScheduleDependency>());
            Assert.IsTrue(collection.Contains(info.Id));

            collection.Remove(info.Id);
            Assert.IsFalse(collection.Contains(info.Id));
        }

        [Test]
        public void Contains()
        {
            var collection = new ScheduleConditionStorage();
            var condition = new Mock<IScheduleCondition>();

            var info = collection.Add(condition.Object, "a", "b", "c", new List<IScheduleDependency>());
            Assert.IsTrue(collection.Contains(info.Id));
            Assert.IsFalse(collection.Contains(new ScheduleElementId()));
            Assert.IsFalse(collection.Contains(null));
        }
    }
}
