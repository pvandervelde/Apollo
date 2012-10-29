//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class GroupExportDefinitionTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<GroupExportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<GroupExportDefinition> 
                    {
                        GroupExportDefinition.CreateDefinition(
                            "a", 
                            new GroupRegistrationId("b"), 
                            null, 
                            Enumerable.Empty<ExportRegistrationId>()),
                        GroupExportDefinition.CreateDefinition(
                            "c", 
                            new GroupRegistrationId("d"), 
                            null, 
                            Enumerable.Empty<ExportRegistrationId>()),
                        GroupExportDefinition.CreateDefinition(
                            "e", 
                            new GroupRegistrationId("f"), 
                            null, 
                            Enumerable.Empty<ExportRegistrationId>()),
                        GroupExportDefinition.CreateDefinition(
                            "g", 
                            new GroupRegistrationId("h"), 
                            new ScheduleId(), 
                            Enumerable.Empty<ExportRegistrationId>()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<GroupExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    GroupExportDefinition.CreateDefinition(
                        "a", 
                        new GroupRegistrationId("b"), 
                        null, 
                        Enumerable.Empty<ExportRegistrationId>()),
                    GroupExportDefinition.CreateDefinition(
                        "c", 
                        new GroupRegistrationId("d"), 
                        null, 
                        Enumerable.Empty<ExportRegistrationId>()),
                    GroupExportDefinition.CreateDefinition(
                        "e", 
                        new GroupRegistrationId("f"), 
                        null, 
                        Enumerable.Empty<ExportRegistrationId>()),
                    GroupExportDefinition.CreateDefinition(
                        "g", 
                        new GroupRegistrationId("h"), 
                        new ScheduleId(), 
                        Enumerable.Empty<ExportRegistrationId>()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                new List<ExportRegistrationId> { new ExportRegistrationId(typeof(string), 1, "a") });
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            GroupExportDefinition first = null;
            var second = GroupExportDefinition.CreateDefinition(
                "b", 
                new GroupRegistrationId("a"), 
                new ScheduleId(), 
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b", 
                new GroupRegistrationId("a"), 
                new ScheduleId(), 
                Enumerable.Empty<ExportRegistrationId>());
            GroupExportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = GroupExportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            GroupExportDefinition first = null;
            var second = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            GroupExportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = GroupExportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var groupId = new GroupRegistrationId("a");
            var contractName = "b";
            var schedule = new ScheduleId();
            var imports = new List<ExportRegistrationId> { new ExportRegistrationId(typeof(string), 0, "a") };
            var obj = GroupExportDefinition.CreateDefinition(contractName, groupId, schedule, imports);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(contractName, obj.ContractName);
            Assert.AreEqual(schedule, obj.ScheduleToExport);
            Assert.AreElementsEqual(imports, obj.ProvidedExports);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            object second = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            object second = GroupExportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
