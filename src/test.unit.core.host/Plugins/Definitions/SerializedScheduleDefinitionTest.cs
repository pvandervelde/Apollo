//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using QuickGraph;

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedScheduleDefinitionTest
    {
        private static IEditableSchedule BuildSchedule()
        {
            var graph = new BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge>();

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);
            graph.AddEdge(new EditableScheduleEdge(start, end));

            return new EditableSchedule(graph, start, end);
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedScheduleDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedScheduleDefinition> 
                    {
                        SerializedScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("a"), 
                            new ScheduleId(), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        SerializedScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("b"), 
                            new ScheduleId(), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        SerializedScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("c"), 
                            new ScheduleId(), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        SerializedScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("d"), 
                            new ScheduleId(), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedScheduleDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("a"), 
                        new ScheduleId(), 
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                    SerializedScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("b"), 
                        new ScheduleId(), 
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                    SerializedScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("c"), 
                        new ScheduleId(), 
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                    SerializedScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("d"), 
                        new ScheduleId(), 
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleDefinition first = null;
            var second = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            SerializedScheduleDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var id = new ScheduleId();
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                id,
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                id,
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("b"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleDefinition first = null;
            var second = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            SerializedScheduleDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var id = new ScheduleId();
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                id,
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                id,
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("b"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var groupId = new GroupRegistrationId("a");
            var scheduleId = new ScheduleId();
            var schedule = BuildSchedule();
            var actions = new Dictionary<ScheduleElementId, ScheduleActionRegistrationId> 
                {
                    { new ScheduleElementId(), new ScheduleActionRegistrationId(typeof(string), 0, "a") }
                };
            var conditions = new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId> 
                { 
                    { new ScheduleElementId(), new ScheduleConditionRegistrationId(typeof(string), 0, "a") }
                };
            var obj = SerializedScheduleDefinition.CreateDefinition(
                groupId,
                scheduleId,
                schedule,
                actions,
                conditions);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(scheduleId, obj.ScheduleId);
            Assert.AreEqual(schedule, obj.Schedule);
            Assert.AreElementsEqual(actions, obj.Actions);
            Assert.AreElementsEqual(conditions, obj.Conditions);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var id = new ScheduleId();
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                id,
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            object second = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                id,
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            object second = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("b"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                new ScheduleId(),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
