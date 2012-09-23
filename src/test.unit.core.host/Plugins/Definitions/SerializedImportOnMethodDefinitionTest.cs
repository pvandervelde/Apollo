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
    public sealed class SerializedImportOnMethodDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedImportOnMethodDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedImportOnMethodDefinition> 
                    {
                        new SerializedImportOnMethodDefinition(
                            "A",
                            typeof(long),
                            typeof(string).GetMethod("Contains").GetParameters().First()),
                        new SerializedImportOnMethodDefinition(
                            "B",
                            typeof(short),
                            typeof(int).GetMethod(
                                "CompareTo", 
                                new[] 
                                { 
                                    typeof(int) 
                                }).GetParameters().First()),
                        new SerializedImportOnMethodDefinition(
                            "C",
                            typeof(object),
                            typeof(double).GetMethod(
                                "CompareTo", 
                                new[] 
                                { 
                                    typeof(double) 
                                }).GetParameters().First()),
                        new SerializedImportOnMethodDefinition(
                            "D",
                            typeof(byte),
                            typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedImportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                     new SerializedImportOnMethodDefinition(
                            "A",
                            typeof(long),
                            typeof(string).GetMethod("Contains").GetParameters().First()),
                        new SerializedImportOnMethodDefinition(
                            "B",
                            typeof(short),
                            typeof(int).GetMethod(
                                "CompareTo", 
                                new[] 
                                { 
                                    typeof(int) 
                                }).GetParameters().First()),
                        new SerializedImportOnMethodDefinition(
                            "C",
                            typeof(object),
                            typeof(double).GetMethod(
                                "CompareTo", 
                                new[] 
                                { 
                                    typeof(double) 
                                }).GetParameters().First()),
                        new SerializedImportOnMethodDefinition(
                            "D",
                            typeof(byte),
                            typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedImportOnMethodDefinition first = null;
            var second = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            SerializedImportOnMethodDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            var second = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            var second = new SerializedImportOnMethodDefinition("B", typeof(float), GetMethodForDouble().GetParameters().First());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedImportOnMethodDefinition first = null;
            var second = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            SerializedImportOnMethodDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            var second = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            var second = new SerializedImportOnMethodDefinition("B", typeof(float), GetMethodForDouble().GetParameters().First());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            var method = GetMethodForInt();
            var parameter = method.GetParameters().First();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(new SerializedTypeIdentity(typeof(long)), obj.ContractType);
            Assert.AreEqual(new SerializedTypeIdentity(typeof(int)), obj.DeclaringType);
            Assert.AreEqual(new SerializedMethodDefinition(method), obj.Method);
            Assert.AreEqual(new SerializedParameterDefinition(parameter), obj.Parameter);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            object second = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            object second = new SerializedImportOnMethodDefinition("B", typeof(float), GetMethodForDouble().GetParameters().First());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedImportOnMethodDefinition("A", typeof(long), GetMethodForInt().GetParameters().First());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
