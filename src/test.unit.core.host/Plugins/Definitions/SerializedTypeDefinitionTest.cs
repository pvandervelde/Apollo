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

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedTypeDefinitionTest
    {
        public sealed class Nested<TKey, TValue>
        { 
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedTypeDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedTypeDefinition> 
                    {
                        new SerializedTypeDefinition(typeof(string)),
                        new SerializedTypeDefinition(typeof(object)),
                        new SerializedTypeDefinition(typeof(int)),
                        new SerializedTypeDefinition(typeof(IComparable)),
                        new SerializedTypeDefinition(typeof(IComparable<>)),
                        new SerializedTypeDefinition(typeof(List<int>)),
                        new SerializedTypeDefinition(typeof(double)),
                        new SerializedTypeDefinition(typeof(void)),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedTypeDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedTypeDefinition(typeof(string)),
                    new SerializedTypeDefinition(typeof(object)),
                    new SerializedTypeDefinition(typeof(int)),
                    new SerializedTypeDefinition(typeof(IComparable)),
                    new SerializedTypeDefinition(typeof(IComparable<>)),
                    new SerializedTypeDefinition(typeof(List<int>)),
                    new SerializedTypeDefinition(typeof(double)),
                    new SerializedTypeDefinition(typeof(void)),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedTypeDefinition(typeof(string));
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedTypeDefinition first = null;
            var second = new SerializedTypeDefinition(typeof(string));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            SerializedTypeDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            var second = new SerializedTypeDefinition(typeof(string));

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            var second = new SerializedTypeDefinition(typeof(object));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedTypeDefinition first = null;
            var second = new SerializedTypeDefinition(typeof(string));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            SerializedTypeDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            var second = new SerializedTypeDefinition(typeof(string));

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            var second = new SerializedTypeDefinition(typeof(object));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = new SerializedTypeDefinition(typeof(List<int>));

            Assert.AreEqual(typeof(List<int>).FullName, obj.Identity.FullName);
            Assert.AreEqual(typeof(List<int>).IsClass, obj.IsClass);
            Assert.AreEqual(typeof(List<int>).IsInterface, obj.IsInterface);

            Assert.AreEqual(typeof(List<int>).BaseType.FullName, obj.BaseType.FullName);

            var interfaces = new[]
                {
                    new SerializedTypeIdentity(typeof(IList<int>)),
                    new SerializedTypeIdentity(typeof(ICollection<int>)),
                    new SerializedTypeIdentity(typeof(IEnumerable<int>)),
                    new SerializedTypeIdentity(typeof(IReadOnlyCollection<int>)),
                    new SerializedTypeIdentity(typeof(IReadOnlyList<int>)),
                    new SerializedTypeIdentity(typeof(IList)),
                    new SerializedTypeIdentity(typeof(ICollection)),
                    new SerializedTypeIdentity(typeof(IEnumerable)),
                };
            Assert.AreElementsEqualIgnoringOrder(interfaces, obj.BaseInterfaces);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = new SerializedTypeDefinition(type);

            Assert.AreEqual(type.FullName, obj.Identity.FullName);
            Assert.AreEqual(type.IsClass, obj.IsClass);
            Assert.AreEqual(type.IsInterface, obj.IsInterface);

            Assert.AreEqual(type.BaseType.FullName, obj.BaseType.FullName);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = new SerializedTypeDefinition(typeof(IEnumerable<>));

            Assert.AreEqual(typeof(IEnumerable<>).FullName, obj.Identity.FullName);
            Assert.AreEqual(typeof(IEnumerable<>).IsClass, obj.IsClass);
            Assert.AreEqual(typeof(IEnumerable<>).IsInterface, obj.IsInterface);
            Assert.IsNull(obj.BaseType);
            Assert.AreElementsEqual(new[] { new SerializedTypeIdentity(typeof(IEnumerable)) }, obj.BaseInterfaces);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            object second = new SerializedTypeDefinition(typeof(string));

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            object second = new SerializedTypeDefinition(typeof(object));

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedTypeDefinition(typeof(string));
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
