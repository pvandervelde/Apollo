//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TypeDefinitionTest
    {
        public sealed class Nested<TKey, TValue>
        { 
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<TypeDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<TypeDefinition> 
                    {
                        TypeDefinition.CreateDefinition(typeof(string)),
                        TypeDefinition.CreateDefinition(typeof(object)),
                        TypeDefinition.CreateDefinition(typeof(int)),
                        TypeDefinition.CreateDefinition(typeof(IComparable)),
                        TypeDefinition.CreateDefinition(typeof(IComparable<>)),
                        TypeDefinition.CreateDefinition(typeof(List<int>)),
                        TypeDefinition.CreateDefinition(typeof(double)),
                        TypeDefinition.CreateDefinition(typeof(void)),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<TypeDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    TypeDefinition.CreateDefinition(typeof(string)),
                    TypeDefinition.CreateDefinition(typeof(object)),
                    TypeDefinition.CreateDefinition(typeof(int)),
                    TypeDefinition.CreateDefinition(typeof(IComparable)),
                    TypeDefinition.CreateDefinition(typeof(IComparable<>)),
                    TypeDefinition.CreateDefinition(typeof(List<int>)),
                    TypeDefinition.CreateDefinition(typeof(double)),
                    TypeDefinition.CreateDefinition(typeof(void)),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = TypeDefinition.CreateDefinition(typeof(string));
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            TypeDefinition first = null;
            var second = TypeDefinition.CreateDefinition(typeof(string));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            TypeDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            var second = TypeDefinition.CreateDefinition(typeof(string));

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            var second = TypeDefinition.CreateDefinition(typeof(object));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            TypeDefinition first = null;
            var second = TypeDefinition.CreateDefinition(typeof(string));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            TypeDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            var second = TypeDefinition.CreateDefinition(typeof(string));

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            var second = TypeDefinition.CreateDefinition(typeof(object));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = TypeDefinition.CreateDefinition(typeof(List<int>));

            Assert.AreEqual(typeof(List<int>).FullName, obj.Identity.FullName);
            Assert.AreEqual(typeof(List<int>).IsClass, obj.IsClass);
            Assert.AreEqual(typeof(List<int>).IsInterface, obj.IsInterface);

            Assert.AreEqual(typeof(List<int>).BaseType.FullName, obj.BaseType.FullName);

            var interfaces = new[]
                {
                    TypeIdentity.CreateDefinition(typeof(IList<int>)),
                    TypeIdentity.CreateDefinition(typeof(ICollection<int>)),
                    TypeIdentity.CreateDefinition(typeof(IEnumerable<int>)),
                    TypeIdentity.CreateDefinition(typeof(IReadOnlyCollection<int>)),
                    TypeIdentity.CreateDefinition(typeof(IReadOnlyList<int>)),
                    TypeIdentity.CreateDefinition(typeof(IList)),
                    TypeIdentity.CreateDefinition(typeof(ICollection)),
                    TypeIdentity.CreateDefinition(typeof(IEnumerable)),
                };
            Assert.AreElementsEqualIgnoringOrder(interfaces, obj.BaseInterfaces);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = TypeDefinition.CreateDefinition(type);

            Assert.AreEqual(type.FullName, obj.Identity.FullName);
            Assert.AreEqual(type.IsClass, obj.IsClass);
            Assert.AreEqual(type.IsInterface, obj.IsInterface);

            Assert.AreEqual(type.BaseType.FullName, obj.BaseType.FullName);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = TypeDefinition.CreateDefinition(typeof(IEnumerable<>));

            Assert.AreEqual(typeof(IEnumerable<>).FullName, obj.Identity.FullName);
            Assert.AreEqual(typeof(IEnumerable<>).IsClass, obj.IsClass);
            Assert.AreEqual(typeof(IEnumerable<>).IsInterface, obj.IsInterface);
            Assert.IsNull(obj.BaseType);
            Assert.AreElementsEqual(new[] { TypeIdentity.CreateDefinition(typeof(IEnumerable)) }, obj.BaseInterfaces);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            object second = TypeDefinition.CreateDefinition(typeof(string));

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            object second = TypeDefinition.CreateDefinition(typeof(object));

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = TypeDefinition.CreateDefinition(typeof(string));
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
