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

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedGroupExportDefinitionTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedGroupExportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedGroupExportDefinition> 
                    {
                        SerializedGroupExportDefinition.CreateDefinition(
                            "a", 
                            new GroupRegistrationId("b"), 
                            null, 
                            Enumerable.Empty<ExportRegistrationId>()),
                        SerializedGroupExportDefinition.CreateDefinition(
                            "c", 
                            new GroupRegistrationId("d"), 
                            null, 
                            Enumerable.Empty<ExportRegistrationId>()),
                        SerializedGroupExportDefinition.CreateDefinition(
                            "e", 
                            new GroupRegistrationId("f"), 
                            null, 
                            Enumerable.Empty<ExportRegistrationId>()),
                        SerializedGroupExportDefinition.CreateDefinition(
                            "g", 
                            new GroupRegistrationId("h"), 
                            new ScheduleId(), 
                            Enumerable.Empty<ExportRegistrationId>()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedGroupExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedGroupExportDefinition.CreateDefinition(
                        "a", 
                        new GroupRegistrationId("b"), 
                        null, 
                        Enumerable.Empty<ExportRegistrationId>()),
                    SerializedGroupExportDefinition.CreateDefinition(
                        "c", 
                        new GroupRegistrationId("d"), 
                        null, 
                        Enumerable.Empty<ExportRegistrationId>()),
                    SerializedGroupExportDefinition.CreateDefinition(
                        "e", 
                        new GroupRegistrationId("f"), 
                        null, 
                        Enumerable.Empty<ExportRegistrationId>()),
                    SerializedGroupExportDefinition.CreateDefinition(
                        "g", 
                        new GroupRegistrationId("h"), 
                        new ScheduleId(), 
                        Enumerable.Empty<ExportRegistrationId>()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedGroupExportDefinition.CreateDefinition(
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
            SerializedGroupExportDefinition first = null;
            var second = SerializedGroupExportDefinition.CreateDefinition(
                "b", 
                new GroupRegistrationId("a"), 
                new ScheduleId(), 
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b", 
                new GroupRegistrationId("a"), 
                new ScheduleId(), 
                Enumerable.Empty<ExportRegistrationId>());
            SerializedGroupExportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = SerializedGroupExportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedGroupExportDefinition first = null;
            var second = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            SerializedGroupExportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = SerializedGroupExportDefinition.CreateDefinition(
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
            var obj = SerializedGroupExportDefinition.CreateDefinition(contractName, groupId, schedule, imports);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(contractName, obj.ContractName);
            Assert.AreEqual(schedule, obj.ScheduleToExport);
            Assert.AreElementsEqual(imports, obj.ProvidedExports);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
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
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            object second = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            object second = SerializedGroupExportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedGroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new ScheduleId(),
                Enumerable.Empty<ExportRegistrationId>());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
