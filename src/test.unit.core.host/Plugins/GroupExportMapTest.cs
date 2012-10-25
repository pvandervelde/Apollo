//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Host.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class GroupExportMapTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<GroupExportMap>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<GroupExportMap> 
                    {
                        new GroupExportMap("a"),
                        new GroupExportMap("b"),
                        new GroupExportMap("c"),
                        new GroupExportMap("d"),
                        new GroupExportMap("e"),
                        new GroupExportMap("f"),
                        new GroupExportMap("g"),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<GroupExportMap>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new GroupExportMap("a"),
                    new GroupExportMap("b"),
                    new GroupExportMap("c"),
                    new GroupExportMap("d"),
                    new GroupExportMap("e"),
                    new GroupExportMap("f"),
                    new GroupExportMap("g"),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new GroupExportMap("a");
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            GroupExportMap first = null;
            var second = new GroupExportMap("a");

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new GroupExportMap("a");
            GroupExportMap second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new GroupExportMap("a");
            var second = new GroupExportMap("a");

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new GroupExportMap("a");
            var second = new GroupExportMap("b");

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            GroupExportMap first = null;
            var second = new GroupExportMap("a");

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new GroupExportMap("a");
            GroupExportMap second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new GroupExportMap("a");
            var second = new GroupExportMap("a");

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new GroupExportMap("a");
            var second = new GroupExportMap("b");

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = new GroupExportMap("a");

            Assert.AreEqual("a", obj.ContractName);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new GroupExportMap("a");
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new GroupExportMap("a");
            object second = new GroupExportMap("a");

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new GroupExportMap("a");
            object second = new GroupExportMap("b");

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new GroupExportMap("a");
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
