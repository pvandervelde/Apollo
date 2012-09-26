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

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedExportOnMethodDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedExportOnMethodDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedExportOnMethodDefinition> 
                    {
                        SerializedExportOnMethodDefinition.CreateDefinition(
                            "A", 
                            typeof(long), 
                            typeof(string).GetMethod("Contains")),
                        SerializedExportOnMethodDefinition.CreateDefinition(
                            "B", 
                            typeof(bool), 
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        SerializedExportOnMethodDefinition.CreateDefinition(
                            "C", 
                            typeof(float), 
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        SerializedExportOnMethodDefinition.CreateDefinition(
                            "D", 
                            typeof(IList<>), 
                            typeof(IComparable).GetMethod("CompareTo")),
                        SerializedExportOnMethodDefinition.CreateDefinition(
                            "E", 
                            typeof(TimeSpan), 
                            typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedExportOnMethodDefinition.CreateDefinition(
                        "A", 
                        typeof(long), 
                        typeof(string).GetMethod("Contains")),
                    SerializedExportOnMethodDefinition.CreateDefinition(
                        "B", 
                        typeof(bool), 
                        typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    SerializedExportOnMethodDefinition.CreateDefinition(
                        "C", 
                        typeof(float), 
                        typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    SerializedExportOnMethodDefinition.CreateDefinition(
                        "D", 
                        typeof(IList<>), 
                        typeof(IComparable).GetMethod("CompareTo")),
                    SerializedExportOnMethodDefinition.CreateDefinition(
                        "E", 
                        typeof(TimeSpan), 
                        typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedExportOnMethodDefinition first = null;
            var second = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            SerializedExportOnMethodDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            var second = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            var second = SerializedExportOnMethodDefinition.CreateDefinition("C", typeof(long), GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedExportOnMethodDefinition first = null;
            var second = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            SerializedExportOnMethodDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            var second = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            var second = SerializedExportOnMethodDefinition.CreateDefinition("C", typeof(long), GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual("B", obj.ContractName);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(bool)), obj.ContractType);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(int)), obj.DeclaringType);
            Assert.AreEqual(SerializedMethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            object second = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            object second = SerializedExportOnMethodDefinition.CreateDefinition("C", typeof(long), GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedExportOnMethodDefinition.CreateDefinition("B", typeof(bool), GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
