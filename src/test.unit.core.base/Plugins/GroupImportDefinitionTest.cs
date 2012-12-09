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
    public sealed class GroupImportDefinitionTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<GroupImportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<GroupImportDefinition> 
                    {
                        GroupImportDefinition.CreateDefinition(
                            "a", 
                            new GroupRegistrationId("b"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        GroupImportDefinition.CreateDefinition(
                            "c", 
                            new GroupRegistrationId("d"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        GroupImportDefinition.CreateDefinition(
                            "e", 
                            new GroupRegistrationId("f"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        GroupImportDefinition.CreateDefinition(
                            "g", 
                            new GroupRegistrationId("h"), 
                            new InsertVertex(0, 1), 
                            Enumerable.Empty<ImportRegistrationId>()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<GroupImportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    GroupImportDefinition.CreateDefinition(
                        "a", 
                        new GroupRegistrationId("b"), 
                        null, 
                        Enumerable.Empty<ImportRegistrationId>()),
                    GroupImportDefinition.CreateDefinition(
                        "c", 
                        new GroupRegistrationId("d"), 
                        null, 
                        Enumerable.Empty<ImportRegistrationId>()),
                    GroupImportDefinition.CreateDefinition(
                        "e", 
                        new GroupRegistrationId("f"), 
                        null, 
                        Enumerable.Empty<ImportRegistrationId>()),
                    GroupImportDefinition.CreateDefinition(
                        "g", 
                        new GroupRegistrationId("h"), 
                        new InsertVertex(0, 1), 
                        Enumerable.Empty<ImportRegistrationId>()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = GroupImportDefinition.CreateDefinition(
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
            GroupImportDefinition first = null;
            var second = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            GroupImportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = GroupImportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            GroupImportDefinition first = null;
            var second = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            GroupImportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = GroupImportDefinition.CreateDefinition(
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
            var vertex = new InsertVertex(0, 1);
            var imports = new List<ImportRegistrationId> { new ImportRegistrationId(typeof(string), 0, "a") };
            var obj = GroupImportDefinition.CreateDefinition(contractName, groupId, vertex, imports);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(contractName, obj.ContractName);
            Assert.AreEqual(vertex, obj.ScheduleInsertPosition);
            Assert.AreElementsEqual(imports, obj.ImportsToMatch);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = GroupImportDefinition.CreateDefinition(
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
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            object second = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            object second = GroupImportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
