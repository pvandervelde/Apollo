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
    public sealed class ParameterDefinitionTest
    {
        private static ParameterInfo ParameterFromInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First();
        }

        private static ParameterInfo ParameterFromDouble()
        {
            return typeof(double).GetMethod("CompareTo", new[] { typeof(double) }).GetParameters().First();
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ParameterDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ParameterDefinition> 
                    {
                        ParameterDefinition.CreateDefinition(
                            typeof(string).GetMethod("Contains").GetParameters().First()),
                        ParameterDefinition.CreateDefinition(
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First()),
                        ParameterDefinition.CreateDefinition(
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) }).GetParameters().First()),
                        ParameterDefinition.CreateDefinition(
                            typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                        ParameterDefinition.CreateDefinition(
                            typeof(IComparable<>).GetMethod("CompareTo").GetParameters().First()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ParameterDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    ParameterDefinition.CreateDefinition(
                        typeof(string).GetMethod("Contains").GetParameters().First()),
                    ParameterDefinition.CreateDefinition(
                        typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First()),
                    ParameterDefinition.CreateDefinition(
                        typeof(double).GetMethod("CompareTo", new[] { typeof(double) }).GetParameters().First()),
                    ParameterDefinition.CreateDefinition(
                        typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                    ParameterDefinition.CreateDefinition(
                        typeof(IComparable<>).GetMethod("CompareTo").GetParameters().First()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            ParameterDefinition first = null;
            var second = ParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            ParameterDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = ParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = ParameterDefinition.CreateDefinition(ParameterFromDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            ParameterDefinition first = null;
            var second = ParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            ParameterDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = ParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = ParameterDefinition.CreateDefinition(ParameterFromDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var parameter = ParameterFromInt();

            Assert.AreEqual(parameter.Name, obj.Name);
            Assert.AreEqual(TypeIdentity.CreateDefinition(parameter.ParameterType), obj.Type);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            object second = ParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            object second = ParameterDefinition.CreateDefinition(ParameterFromDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
