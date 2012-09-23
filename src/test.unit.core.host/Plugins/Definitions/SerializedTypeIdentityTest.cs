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

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedTypeIdentityTest
    {
        public sealed class Nested<TKey, TValue>
        {
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedTypeIdentity>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedTypeIdentity> 
                    {
                        new SerializedTypeIdentity(typeof(string)),
                        new SerializedTypeIdentity(typeof(object)),
                        new SerializedTypeIdentity(typeof(int)),
                        new SerializedTypeIdentity(typeof(IComparable)),
                        new SerializedTypeIdentity(typeof(IComparable<>)),
                        new SerializedTypeIdentity(typeof(List<int>)),
                        new SerializedTypeIdentity(typeof(double)),
                        new SerializedTypeIdentity(typeof(void)),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedTypeIdentity>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedTypeIdentity(typeof(string)),
                    new SerializedTypeIdentity(typeof(object)),
                    new SerializedTypeIdentity(typeof(int)),
                    new SerializedTypeIdentity(typeof(IComparable)),
                    new SerializedTypeIdentity(typeof(IComparable<>)),
                    new SerializedTypeIdentity(typeof(List<int>)),
                    new SerializedTypeIdentity(typeof(double)),
                    new SerializedTypeIdentity(typeof(void)),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedTypeIdentity(typeof(string));
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedTypeIdentity first = null;
            var second = new SerializedTypeIdentity(typeof(string));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            SerializedTypeIdentity second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            var second = new SerializedTypeIdentity(typeof(string));

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            var second = new SerializedTypeIdentity(typeof(object));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedTypeIdentity first = null;
            var second = new SerializedTypeIdentity(typeof(string));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            SerializedTypeIdentity second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            var second = new SerializedTypeIdentity(typeof(string));

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            var second = new SerializedTypeIdentity(typeof(object));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = new SerializedTypeIdentity(typeof(List<int>));

            Assert.AreEqual(typeof(List<int>).Name, obj.Name);
            Assert.AreEqual(typeof(List<int>).Namespace, obj.Namespace);
            Assert.AreEqual(typeof(List<int>).FullName, obj.FullName);
            Assert.AreEqual(typeof(List<int>).Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = new SerializedTypeIdentity(type);

            Assert.AreEqual(type.Name, obj.Name);
            Assert.AreEqual(type.Namespace, obj.Namespace);
            Assert.AreEqual(type.FullName, obj.FullName);
            Assert.AreEqual(type.Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = new SerializedTypeIdentity(typeof(IEnumerable<>));

            Assert.AreEqual(typeof(IEnumerable<>).Name, obj.Name);
            Assert.AreEqual(typeof(IEnumerable<>).Namespace, obj.Namespace);
            Assert.AreEqual(typeof(IEnumerable<>).FullName, obj.FullName);
            Assert.AreEqual(typeof(IEnumerable<>).Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            object second = new SerializedTypeIdentity(typeof(string));

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            object second = new SerializedTypeIdentity(typeof(object));

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedTypeIdentity(typeof(string));
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
