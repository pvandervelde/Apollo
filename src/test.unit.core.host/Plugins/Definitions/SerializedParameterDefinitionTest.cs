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
    public sealed class SerializedParameterDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedParameterDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedParameterDefinition> 
                    {
                        SerializedParameterDefinition.CreateDefinition(
                            typeof(string).GetMethod("Contains").GetParameters().First()),
                        SerializedParameterDefinition.CreateDefinition(
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First()),
                        SerializedParameterDefinition.CreateDefinition(
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) }).GetParameters().First()),
                        SerializedParameterDefinition.CreateDefinition(
                            typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                        SerializedParameterDefinition.CreateDefinition(
                            typeof(IComparable<>).GetMethod("CompareTo").GetParameters().First()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedParameterDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedParameterDefinition.CreateDefinition(
                        typeof(string).GetMethod("Contains").GetParameters().First()),
                    SerializedParameterDefinition.CreateDefinition(
                        typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First()),
                    SerializedParameterDefinition.CreateDefinition(
                        typeof(double).GetMethod("CompareTo", new[] { typeof(double) }).GetParameters().First()),
                    SerializedParameterDefinition.CreateDefinition(
                        typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                    SerializedParameterDefinition.CreateDefinition(
                        typeof(IComparable<>).GetMethod("CompareTo").GetParameters().First()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedParameterDefinition first = null;
            var second = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            SerializedParameterDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = SerializedParameterDefinition.CreateDefinition(ParameterFromDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedParameterDefinition first = null;
            var second = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            SerializedParameterDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = SerializedParameterDefinition.CreateDefinition(ParameterFromDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            var parameter = ParameterFromInt();

            Assert.AreEqual(parameter.Name, obj.Name);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(parameter.ParameterType), obj.Type);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            object second = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            object second = SerializedParameterDefinition.CreateDefinition(ParameterFromDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedParameterDefinition.CreateDefinition(ParameterFromInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
