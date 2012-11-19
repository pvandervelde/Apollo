﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
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
    public sealed class ConstructorBasedImportDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ConstructorBasedImportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ConstructorBasedImportDefinition> 
                    {
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "A",
                            TypeIdentity.CreateDefinition(typeof(char[])),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(string).GetConstructor(
                                new[] 
                                { 
                                    typeof(char[])
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "B",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(Uri).GetConstructor(
                                new[] 
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "C",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(Version).GetConstructor(
                                new[] 
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "D",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(NotImplementedException).GetConstructor(
                                new[] 
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializableImportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    ConstructorBasedImportDefinition.CreateDefinition(
                            "A",
                            TypeIdentity.CreateDefinition(typeof(char[])),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(string).GetConstructor(
                                new[] 
                                { 
                                    typeof(char[])
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "B",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(Uri).GetConstructor(
                                new[] 
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "C",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(Version).GetConstructor(
                                new[] 
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "D",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(NotImplementedException).GetConstructor(
                                new[] 
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            ConstructorBasedImportDefinition first = null;
            var second = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            ConstructorBasedImportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var second = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var second = ConstructorBasedImportDefinition.CreateDefinition(
                "B",
                TypeIdentity.CreateDefinition(typeof(string)),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForUri().GetParameters().First());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            ConstructorBasedImportDefinition first = null;
            var second = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            ConstructorBasedImportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var second = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var second = ConstructorBasedImportDefinition.CreateDefinition(
                "B",
                TypeIdentity.CreateDefinition(typeof(string)),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForUri().GetParameters().First());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var constructor = GetConstructorForString();
            var parameter = constructor.GetParameters().First();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(char[])), obj.RequiredTypeIdentity);
            Assert.AreEqual(ImportCardinality.ExactlyOne, obj.Cardinality);
            Assert.IsFalse(obj.IsRecomposable);
            Assert.IsTrue(obj.IsPrerequisite);
            Assert.AreEqual(ConstructorDefinition.CreateDefinition(constructor), obj.Constructor);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(string)), obj.DeclaringType);
            Assert.AreEqual(ParameterDefinition.CreateDefinition(parameter), obj.Parameter);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            object second = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            object second = ConstructorBasedImportDefinition.CreateDefinition(
                "B",
                TypeIdentity.CreateDefinition(typeof(string)),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForUri().GetParameters().First());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
