//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TypeBasedExportDefinitionTest
    {
        public sealed class Nested<TKey, TValue>
        {
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<TypeBasedExportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<TypeBasedExportDefinition> 
                    {
                        TypeBasedExportDefinition.CreateDefinition("A", typeof(string)),
                        TypeBasedExportDefinition.CreateDefinition("B", typeof(object)),
                        TypeBasedExportDefinition.CreateDefinition("C", typeof(int)),
                        TypeBasedExportDefinition.CreateDefinition("D", typeof(IComparable)),
                        TypeBasedExportDefinition.CreateDefinition("E", typeof(IComparable<>)),
                        TypeBasedExportDefinition.CreateDefinition("F", typeof(List<int>)),
                        TypeBasedExportDefinition.CreateDefinition("G", typeof(double)),
                        TypeBasedExportDefinition.CreateDefinition("H", typeof(void)),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializableExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    TypeBasedExportDefinition.CreateDefinition("A", typeof(string)),
                    TypeBasedExportDefinition.CreateDefinition("B", typeof(object)),
                    TypeBasedExportDefinition.CreateDefinition("C", typeof(int)),
                    TypeBasedExportDefinition.CreateDefinition("D", typeof(IComparable)),
                    TypeBasedExportDefinition.CreateDefinition("E", typeof(IComparable<>)),
                    TypeBasedExportDefinition.CreateDefinition("F", typeof(List<int>)),
                    TypeBasedExportDefinition.CreateDefinition("G", typeof(double)),
                    TypeBasedExportDefinition.CreateDefinition("H", typeof(void)),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            TypeBasedExportDefinition first = null;
            var second = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            TypeBasedExportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            var second = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            var second = TypeBasedExportDefinition.CreateDefinition("B", typeof(object));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            TypeBasedExportDefinition first = null;
            var second = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            TypeBasedExportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            var second = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            var second = TypeBasedExportDefinition.CreateDefinition("B", typeof(object));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = TypeBasedExportDefinition.CreateDefinition("A", typeof(List<int>));

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(List<int>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = TypeBasedExportDefinition.CreateDefinition("A", type);

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Nested<,>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = TypeBasedExportDefinition.CreateDefinition("A", typeof(IEnumerable<>));

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<>)), obj.DeclaringType);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            object second = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            object second = TypeBasedExportDefinition.CreateDefinition("B", typeof(object));

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
