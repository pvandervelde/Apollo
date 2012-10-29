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

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MethodDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<MethodDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<MethodDefinition> 
                    {
                        MethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains")),
                        MethodDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        MethodDefinition.CreateDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        MethodDefinition.CreateDefinition(typeof(IComparable).GetMethod("CompareTo")),
                        MethodDefinition.CreateDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<MethodDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    MethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains")),
                    MethodDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    MethodDefinition.CreateDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    MethodDefinition.CreateDefinition(typeof(IComparable).GetMethod("CompareTo")),
                    MethodDefinition.CreateDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = MethodDefinition.CreateDefinition(GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            MethodDefinition first = null;
            var second = MethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            MethodDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            var second = MethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            var second = MethodDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            MethodDefinition first = null;
            var second = MethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            MethodDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            var second = MethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            var second = MethodDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = MethodDefinition.CreateDefinition(GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual(method.Name, obj.MethodName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(method.ReturnType), obj.ReturnType);
            Assert.AreElementsEqualIgnoringOrder(
                method.GetParameters().Select(p => ParameterDefinition.CreateDefinition(p)), 
                obj.Parameters);
            Assert.AreEqual(TypeIdentity.CreateDefinition(method.DeclaringType), obj.DeclaringType);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            object second = MethodDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            object second = MethodDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = MethodDefinition.CreateDefinition(GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
