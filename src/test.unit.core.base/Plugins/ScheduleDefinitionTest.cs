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

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleDefinitionTest
    {
        private static IEditableSchedule BuildSchedule()
        {
            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new EditableStartVertex(1);
            graph.AddVertex(start);

            var end = new EditableEndVertex(2);
            graph.AddVertex(end);
            graph.AddEdge(new ScheduleEdge(start, end));

            return new EditableSchedule(graph, start, end);
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ScheduleDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ScheduleDefinition> 
                    {
                        ScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("a"), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        ScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("b"), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        ScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("c"), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        ScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("d"), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ScheduleDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    ScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("a"), 
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                    ScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("b"), 
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                    ScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("c"), 
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                    ScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("d"), 
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            ScheduleDefinition first = null;
            var second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            ScheduleDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("b"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            ScheduleDefinition first = null;
            var second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            ScheduleDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("b"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var groupId = new GroupRegistrationId("a");
            var schedule = BuildSchedule();
            var actions = new Dictionary<ScheduleElementId, ScheduleActionRegistrationId> 
                {
                    { new ScheduleElementId(), new ScheduleActionRegistrationId(typeof(string), 0, "a") }
                };
            var conditions = new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId> 
                { 
                    { new ScheduleElementId(), new ScheduleConditionRegistrationId(typeof(string), 0, "a") }
                };
            var obj = ScheduleDefinition.CreateDefinition(
                groupId,
                schedule,
                actions,
                conditions);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(schedule, obj.Schedule);
            Assert.AreElementsEqual(actions, obj.Actions);
            Assert.AreElementsEqual(conditions, obj.Conditions);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            object second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            object second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("b"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
