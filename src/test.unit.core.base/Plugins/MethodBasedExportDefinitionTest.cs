//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MethodBasedExportDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<MethodBasedExportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<MethodBasedExportDefinition> 
                    {
                        MethodBasedExportDefinition.CreateDefinition(
                            "A", 
                            typeof(string).GetMethod("Contains")),
                        MethodBasedExportDefinition.CreateDefinition(
                            "B", 
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        MethodBasedExportDefinition.CreateDefinition(
                            "C", 
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        MethodBasedExportDefinition.CreateDefinition(
                            "D", 
                            typeof(IComparable).GetMethod("CompareTo")),
                        MethodBasedExportDefinition.CreateDefinition(
                            "E", 
                            typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializableExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    MethodBasedExportDefinition.CreateDefinition(
                        "A", 
                        typeof(string).GetMethod("Contains")),
                    MethodBasedExportDefinition.CreateDefinition(
                        "B", 
                        typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    MethodBasedExportDefinition.CreateDefinition(
                        "C", 
                        typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    MethodBasedExportDefinition.CreateDefinition(
                        "D", 
                        typeof(IComparable).GetMethod("CompareTo")),
                    MethodBasedExportDefinition.CreateDefinition(
                        "E", 
                        typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            MethodBasedExportDefinition first = null;
            var second = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            MethodBasedExportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            var second = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            var second = MethodBasedExportDefinition.CreateDefinition("C", GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            MethodBasedExportDefinition first = null;
            var second = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            MethodBasedExportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            var second = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            var second = MethodBasedExportDefinition.CreateDefinition("C", GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual("B", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(int)), obj.DeclaringType);
            Assert.AreEqual(MethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            object second = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            object second = MethodBasedExportDefinition.CreateDefinition("C", GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
