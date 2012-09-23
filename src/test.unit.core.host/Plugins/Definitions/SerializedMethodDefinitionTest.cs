//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedMethodDefinitionTest
    {
        private static MethodInfo GetMethodForInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
        }

        private static MethodInfo GetMethodForDouble()
        {
            return typeof(double).GetMethod("CompareTo", new[] { typeof(double) });
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedMethodDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedMethodDefinition> 
                    {
                        new SerializedMethodDefinition(typeof(string).GetMethod("Contains")),
                        new SerializedMethodDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        new SerializedMethodDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        new SerializedMethodDefinition(typeof(IComparable).GetMethod("CompareTo")),
                        new SerializedMethodDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedMethodDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedMethodDefinition(typeof(string).GetMethod("Contains")),
                    new SerializedMethodDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    new SerializedMethodDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    new SerializedMethodDefinition(typeof(IComparable).GetMethod("CompareTo")),
                    new SerializedMethodDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedMethodDefinition(GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedMethodDefinition first = null;
            var second = new SerializedMethodDefinition(GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            SerializedMethodDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            var second = new SerializedMethodDefinition(GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            var second = new SerializedMethodDefinition(GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedMethodDefinition first = null;
            var second = new SerializedMethodDefinition(GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            SerializedMethodDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            var second = new SerializedMethodDefinition(GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            var second = new SerializedMethodDefinition(GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = new SerializedMethodDefinition(GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual(method.Name, obj.MethodName);
            Assert.AreEqual(new SerializedTypeIdentity(method.ReturnType), obj.ReturnType);
            Assert.AreElementsEqualIgnoringOrder(method.GetParameters().Select(p => new SerializedParameterDefinition(p)), obj.Parameters);
            Assert.AreEqual(new SerializedTypeIdentity(method.DeclaringType), obj.DeclaringType);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            object second = new SerializedMethodDefinition(GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            object second = new SerializedMethodDefinition(GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedMethodDefinition(GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
