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
                        SerializedImportOnConstructorDefinition.CreateDefinition(
                            "A",
                            typeof(string).GetConstructor(new[] 
                                { 
                                    typeof(char[])
                                }).GetParameters().First()),
                        SerializedImportOnConstructorDefinition.CreateDefinition(
                            "B",
                            typeof(Uri).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                        SerializedImportOnConstructorDefinition.CreateDefinition(
                            "C",
                            typeof(Version).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                        SerializedImportOnConstructorDefinition.CreateDefinition(
                            "D",
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
                    SerializedImportOnConstructorDefinition.CreateDefinition(
                            "A",
                            typeof(string).GetConstructor(new[] 
                                { 
                                    typeof(char[])
                                }).GetParameters().First()),
                        SerializedImportOnConstructorDefinition.CreateDefinition(
                            "B",
                            typeof(Uri).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                        SerializedImportOnConstructorDefinition.CreateDefinition(
                            "C",
                            typeof(Version).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                        SerializedImportOnConstructorDefinition.CreateDefinition(
                            "D",
                            typeof(NotImplementedException).GetConstructor(new[] 
                            {
                                typeof(string)
                            }).GetParameters().First()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedImportOnConstructorDefinition first = null;
            var second = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            SerializedImportOnConstructorDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            var second = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            var second = SerializedImportOnConstructorDefinition.CreateDefinition(
                "B", 
                GetConstructorForUri().GetParameters().First());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedImportOnConstructorDefinition first = null;
            var second = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            SerializedImportOnConstructorDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            var second = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            var second = SerializedImportOnConstructorDefinition.CreateDefinition(
                "B", 
                GetConstructorForUri().GetParameters().First());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            var constructor = GetConstructorForString();
            var parameter = constructor.GetParameters().First();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(SerializedConstructorDefinition.CreateDefinition(constructor), obj.Constructor);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(string)), obj.DeclaringType);
            Assert.AreEqual(SerializedParameterDefinition.CreateDefinition(parameter), obj.Parameter);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            object second = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            object second = SerializedImportOnConstructorDefinition.CreateDefinition(
                "B", 
                GetConstructorForUri().GetParameters().First());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedImportOnConstructorDefinition.CreateDefinition(
                "A", 
                GetConstructorForString().GetParameters().First());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
