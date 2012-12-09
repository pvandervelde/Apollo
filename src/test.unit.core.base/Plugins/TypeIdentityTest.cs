//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TypeIdentityTest
    {
        public sealed class Nested<TKey, TValue>
        {
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<TypeIdentity>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<TypeIdentity> 
                    {
                        TypeIdentity.CreateDefinition(typeof(string)),
                        TypeIdentity.CreateDefinition(typeof(object)),
                        TypeIdentity.CreateDefinition(typeof(int)),
                        TypeIdentity.CreateDefinition(typeof(IComparable)),
                        TypeIdentity.CreateDefinition(typeof(IComparable<>)),
                        TypeIdentity.CreateDefinition(typeof(List<int>)),
                        TypeIdentity.CreateDefinition(typeof(double)),
                        TypeIdentity.CreateDefinition(typeof(void)),
                        TypeIdentity.CreateDefinition(typeof(IEnumerable<>).GetGenericArguments().First()),
                        TypeIdentity.CreateDefinition(typeof(IComparable<>).GetGenericArguments().First()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<TypeIdentity>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    TypeIdentity.CreateDefinition(typeof(string)),
                    TypeIdentity.CreateDefinition(typeof(object)),
                    TypeIdentity.CreateDefinition(typeof(int)),
                    TypeIdentity.CreateDefinition(typeof(IComparable)),
                    TypeIdentity.CreateDefinition(typeof(IComparable<>)),
                    TypeIdentity.CreateDefinition(typeof(List<int>)),
                    TypeIdentity.CreateDefinition(typeof(double)),
                    TypeIdentity.CreateDefinition(typeof(void)),
                    TypeIdentity.CreateDefinition(typeof(IEnumerable<>).GetGenericArguments().First()),
                    TypeIdentity.CreateDefinition(typeof(IComparable<>).GetGenericArguments().First()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = TypeIdentity.CreateDefinition(typeof(string));
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            TypeIdentity first = null;
            var second = TypeIdentity.CreateDefinition(typeof(string));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            TypeIdentity second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            var second = TypeIdentity.CreateDefinition(typeof(string));

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            var second = TypeIdentity.CreateDefinition(typeof(object));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            TypeIdentity first = null;
            var second = TypeIdentity.CreateDefinition(typeof(string));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            TypeIdentity second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            var second = TypeIdentity.CreateDefinition(typeof(string));

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            var second = TypeIdentity.CreateDefinition(typeof(object));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = TypeIdentity.CreateDefinition(typeof(List<int>));

            Assert.AreEqual(typeof(List<int>).Name, obj.Name);
            Assert.AreEqual(typeof(List<int>).Namespace, obj.Namespace);
            Assert.AreEqual(typeof(List<int>).FullName, obj.FullName);
            Assert.AreEqual(typeof(List<int>).Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = TypeIdentity.CreateDefinition(type);

            Assert.AreEqual(type.Name, obj.Name);
            Assert.AreEqual(type.Namespace, obj.Namespace);
            Assert.AreEqual(type.FullName, obj.FullName);
            Assert.AreEqual(type.Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = TypeIdentity.CreateDefinition(typeof(IEnumerable<>));

            Assert.AreEqual(typeof(IEnumerable<>).Name, obj.Name);
            Assert.AreEqual(typeof(IEnumerable<>).Namespace, obj.Namespace);
            Assert.AreEqual(typeof(IEnumerable<>).FullName, obj.FullName);
            Assert.AreEqual(typeof(IEnumerable<>).Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            object second = TypeIdentity.CreateDefinition(typeof(string));

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            object second = TypeIdentity.CreateDefinition(typeof(object));

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(object));
            var second = typeof(object);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualGenericType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<int>));
            var second = typeof(IEnumerable<int>);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualOpenGenericType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<>));
            var second = typeof(IEnumerable<>);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualGenericTypeParameter()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<>).GetGenericArguments().First());
            var second = typeof(IEnumerable<>).GetGenericArguments().First();

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(object));
            var second = typeof(string);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalGenericType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<int>));
            var second = typeof(IEnumerable<float>);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalOpenGenericType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<>));
            var second = typeof(IComparable<>);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalGenericTypeParameter()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<>).GetGenericArguments().First());
            var second = typeof(IComparable<>).GetGenericArguments().First();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
