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
                        new SerializedParameterDefinition(typeof(string).GetMethod("Contains").GetParameters().First()),
                        new SerializedParameterDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First()),
                        new SerializedParameterDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) }).GetParameters().First()),
                        new SerializedParameterDefinition(typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                        new SerializedParameterDefinition(typeof(IComparable<>).GetMethod("CompareTo").GetParameters().First()),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedParameterDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedParameterDefinition(typeof(string).GetMethod("Contains").GetParameters().First()),
                    new SerializedParameterDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First()),
                    new SerializedParameterDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) }).GetParameters().First()),
                    new SerializedParameterDefinition(typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                    new SerializedParameterDefinition(typeof(IComparable<>).GetMethod("CompareTo").GetParameters().First()),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedParameterDefinition(ParameterFromInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedParameterDefinition first = null;
            var second = new SerializedParameterDefinition(ParameterFromInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            SerializedParameterDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            var second = new SerializedParameterDefinition(ParameterFromInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            var second = new SerializedParameterDefinition(ParameterFromDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedParameterDefinition first = null;
            var second = new SerializedParameterDefinition(ParameterFromInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            SerializedParameterDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            var second = new SerializedParameterDefinition(ParameterFromInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            var second = new SerializedParameterDefinition(ParameterFromDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = new SerializedParameterDefinition(ParameterFromInt());
            var parameter = ParameterFromInt();

            Assert.AreEqual(parameter.Name, obj.Name);
            Assert.AreEqual(new SerializedTypeIdentity(parameter.ParameterType), obj.Type);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            object second = new SerializedParameterDefinition(ParameterFromInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            object second = new SerializedParameterDefinition(ParameterFromDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedParameterDefinition(ParameterFromInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
