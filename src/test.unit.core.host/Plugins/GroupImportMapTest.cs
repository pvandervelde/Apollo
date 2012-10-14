//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Plugins;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Host.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class GroupImportMapTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<GroupImportMap>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<GroupImportMap> 
                    {
                        new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a")),
                        new GroupImportMap(new ImportRegistrationId(typeof(int), 0, "a")),
                        new GroupImportMap(new ImportRegistrationId(typeof(string), 1, "a")),
                        new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "b")),
                        new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 0, "a")),
                        new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(int), 0, "a")),
                        new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 1, "a")),
                        new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 0, "b")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<GroupImportMap>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a")),
                    new GroupImportMap(new ImportRegistrationId(typeof(int), 0, "a")),
                    new GroupImportMap(new ImportRegistrationId(typeof(string), 1, "a")),
                    new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "b")),
                    new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 0, "a")),
                    new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(int), 0, "a")),
                    new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 1, "a")),
                    new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 0, "b")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            GroupImportMap first = null;
            var second = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            GroupImportMap second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            var second = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            var second = new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 0, "a"));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            GroupImportMap first = null;
            var second = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            GroupImportMap second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            var second = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            var second = new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 0, "a"));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var group = new GroupRegistrationId("a");
            var export = new ImportRegistrationId(typeof(string), 0, "b");
            var obj = new GroupImportMap(group, export);

            Assert.AreEqual(group, obj.Group);
            Assert.AreEqual(export, obj.Import);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            object second = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            object second = new GroupImportMap(new GroupRegistrationId("a"), new ImportRegistrationId(typeof(string), 0, "a"));

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new GroupImportMap(new ImportRegistrationId(typeof(string), 0, "a"));
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
