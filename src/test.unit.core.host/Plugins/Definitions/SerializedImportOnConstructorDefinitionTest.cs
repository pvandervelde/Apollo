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
    public sealed class SerializedImportOnConstructorDefinitionTest
    {
        private static ConstructorInfo GetConstructorForString()
        {
            return typeof(string).GetConstructor(new[] { typeof(char[]) });
        }

        private static ConstructorInfo GetConstructorForUri()
        {
            return typeof(Uri).GetConstructor(new[] { typeof(string) });
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedImportOnConstructorDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedImportOnConstructorDefinition> 
                    {
                        new SerializedImportOnConstructorDefinition(
                            "A",
                            typeof(bool),
                            typeof(string).GetConstructor(new[] 
                                { 
                                    typeof(char[])
                                }).GetParameters().First()),
                        new SerializedImportOnConstructorDefinition(
                            "B",
                            typeof(long),
                            typeof(Uri).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                        new SerializedImportOnConstructorDefinition(
                            "C",
                            typeof(string),
                            typeof(Version).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                        new SerializedImportOnConstructorDefinition(
                            "D",
                            typeof(string),
                            typeof(NotImplementedException).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedImportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedImportOnConstructorDefinition(
                            "A",
                            typeof(bool),
                            typeof(string).GetConstructor(new[] 
                                { 
                                    typeof(char[])
                                }).GetParameters().First()),
                        new SerializedImportOnConstructorDefinition(
                            "B",
                            typeof(long),
                            typeof(Uri).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                        new SerializedImportOnConstructorDefinition(
                            "C",
                            typeof(string),
                            typeof(Version).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                        new SerializedImportOnConstructorDefinition(
                            "D",
                            typeof(string),
                            typeof(NotImplementedException).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedImportOnConstructorDefinition first = null;
            var second = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            SerializedImportOnConstructorDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            var second = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            var second = new SerializedImportOnConstructorDefinition("B", typeof(short), GetConstructorForUri().GetParameters().First());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedImportOnConstructorDefinition first = null;
            var second = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            SerializedImportOnConstructorDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            var second = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            var second = new SerializedImportOnConstructorDefinition("B", typeof(short), GetConstructorForUri().GetParameters().First());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            var constructor = GetConstructorForString();
            var parameter = constructor.GetParameters().First();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(new SerializedTypeIdentity(typeof(long)), obj.ContractType);
            Assert.AreEqual(new SerializedConstructorDefinition(constructor), obj.Constructor);
            Assert.AreEqual(new SerializedTypeIdentity(typeof(string)), obj.DeclaringType);
            Assert.AreEqual(new SerializedParameterDefinition(parameter), obj.Parameter);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            object second = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            object second = new SerializedImportOnConstructorDefinition("B", typeof(short), GetConstructorForUri().GetParameters().First());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedImportOnConstructorDefinition("A", typeof(long), GetConstructorForString().GetParameters().First());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
