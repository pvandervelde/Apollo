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
    public sealed class SerializedScheduleConditionOnMethodDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedScheduleConditionOnMethodDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedScheduleConditionOnMethodDefinition> 
                    {
                        SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                            "a", 
                            typeof(string).GetMethod("Contains")),
                        SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                            "b", 
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                            "c", 
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                            "d", 
                            typeof(IComparable).GetMethod("CompareTo")),
                        SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                            "e", 
                            typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedScheduleConditionDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                        "a", 
                        typeof(string).GetMethod("Contains")),
                    SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                        "b", 
                        typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                        "c", 
                        typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                        "d", 
                        typeof(IComparable).GetMethod("CompareTo")),
                    SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                        "e", 
                        typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleConditionOnMethodDefinition first = null;
            var second = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            SerializedScheduleConditionOnMethodDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            var second = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            var second = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("b", GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleConditionOnMethodDefinition first = null;
            var second = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            SerializedScheduleConditionOnMethodDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            var second = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            var second = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("b", GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual("a", obj.ContractName);
            Assert.AreEqual(SerializedMethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            object second = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            object second = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("b", GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedScheduleConditionOnMethodDefinition.CreateDefinition("a", GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
