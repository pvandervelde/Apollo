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
    public sealed class SerializedGroupImportDefinitionTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedGroupImportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedGroupImportDefinition> 
                    {
                        SerializedGroupImportDefinition.CreateDefinition(
                            "a", 
                            new GroupRegistrationId("b"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        SerializedGroupImportDefinition.CreateDefinition(
                            "c", 
                            new GroupRegistrationId("d"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        SerializedGroupImportDefinition.CreateDefinition(
                            "e", 
                            new GroupRegistrationId("f"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        SerializedGroupImportDefinition.CreateDefinition(
                            "g", 
                            new GroupRegistrationId("h"), 
                            new EditableInsertVertex(0, 1), 
                            Enumerable.Empty<ImportRegistrationId>()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedGroupImportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedGroupImportDefinition.CreateDefinition(
                        "a", 
                        new GroupRegistrationId("b"), 
                        null, 
                        Enumerable.Empty<ImportRegistrationId>()),
                    SerializedGroupImportDefinition.CreateDefinition(
                        "c", 
                        new GroupRegistrationId("d"), 
                        null, 
                        Enumerable.Empty<ImportRegistrationId>()),
                    SerializedGroupImportDefinition.CreateDefinition(
                        "e", 
                        new GroupRegistrationId("f"), 
                        null, 
                        Enumerable.Empty<ImportRegistrationId>()),
                    SerializedGroupImportDefinition.CreateDefinition(
                        "g", 
                        new GroupRegistrationId("h"), 
                        new EditableInsertVertex(0, 1), 
                        Enumerable.Empty<ImportRegistrationId>()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                new List<ImportRegistrationId> { new ImportRegistrationId(typeof(string), 0, "a") });
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedGroupImportDefinition first = null;
            var second = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            SerializedGroupImportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = SerializedGroupImportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedGroupImportDefinition first = null;
            var second = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            SerializedGroupImportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = SerializedGroupImportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var groupId = new GroupRegistrationId("a");
            var contractName = "b";
            var vertex = new EditableInsertVertex(0, 1);
            var imports = new List<ImportRegistrationId> { new ImportRegistrationId(typeof(string), 0, "a") };
            var obj = SerializedGroupImportDefinition.CreateDefinition(contractName, groupId, vertex, imports);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(contractName, obj.ContractName);
            Assert.AreEqual(vertex, obj.ScheduleInsertPosition);
            Assert.AreElementsEqual(imports, obj.ImportsToMatch);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            object second = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            object second = SerializedGroupImportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedGroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
