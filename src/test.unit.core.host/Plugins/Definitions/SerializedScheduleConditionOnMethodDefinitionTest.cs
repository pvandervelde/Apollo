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
                        new SerializedScheduleConditionOnMethodDefinition(typeof(string).GetMethod("Contains")),
                        new SerializedScheduleConditionOnMethodDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        new SerializedScheduleConditionOnMethodDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        new SerializedScheduleConditionOnMethodDefinition(typeof(IComparable).GetMethod("CompareTo")),
                        new SerializedScheduleConditionOnMethodDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedScheduleConditionDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedScheduleConditionOnMethodDefinition(typeof(string).GetMethod("Contains")),
                    new SerializedScheduleConditionOnMethodDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    new SerializedScheduleConditionOnMethodDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    new SerializedScheduleConditionOnMethodDefinition(typeof(IComparable).GetMethod("CompareTo")),
                    new SerializedScheduleConditionOnMethodDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleConditionOnMethodDefinition first = null;
            var second = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            SerializedScheduleConditionOnMethodDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            var second = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            var second = new SerializedScheduleConditionOnMethodDefinition(GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleConditionOnMethodDefinition first = null;
            var second = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            SerializedScheduleConditionOnMethodDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            var second = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            var second = new SerializedScheduleConditionOnMethodDefinition(GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual(new SerializedMethodDefinition(GetMethodForInt()), obj.Method);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            object second = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            object second = new SerializedScheduleConditionOnMethodDefinition(GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedScheduleConditionOnMethodDefinition(GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
