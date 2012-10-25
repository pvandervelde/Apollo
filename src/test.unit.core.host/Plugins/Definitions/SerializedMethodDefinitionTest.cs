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
                        SerializedMethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains")),
                        SerializedMethodDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        SerializedMethodDefinition.CreateDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        SerializedMethodDefinition.CreateDefinition(typeof(IComparable).GetMethod("CompareTo")),
                        SerializedMethodDefinition.CreateDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedMethodDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedMethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains")),
                    SerializedMethodDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    SerializedMethodDefinition.CreateDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    SerializedMethodDefinition.CreateDefinition(typeof(IComparable).GetMethod("CompareTo")),
                    SerializedMethodDefinition.CreateDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedMethodDefinition first = null;
            var second = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            SerializedMethodDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            var second = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            var second = SerializedMethodDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedMethodDefinition first = null;
            var second = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            SerializedMethodDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            var second = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            var second = SerializedMethodDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual(method.Name, obj.MethodName);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(method.ReturnType), obj.ReturnType);
            Assert.AreElementsEqualIgnoringOrder(
                method.GetParameters().Select(p => SerializedParameterDefinition.CreateDefinition(p)), 
                obj.Parameters);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(method.DeclaringType), obj.DeclaringType);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            object second = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            object second = SerializedMethodDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedMethodDefinition.CreateDefinition(GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
