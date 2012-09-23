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
    public sealed class SerializedScheduleActionDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedScheduleActionDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedScheduleActionDefinition> 
                    {
                        new SerializedScheduleActionDefinition(typeof(string).GetMethod("Contains")),
                        new SerializedScheduleActionDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        new SerializedScheduleActionDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        new SerializedScheduleActionDefinition(typeof(IComparable).GetMethod("CompareTo")),
                        new SerializedScheduleActionDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedScheduleActionDefinition(typeof(string).GetMethod("Contains")),
                    new SerializedScheduleActionDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    new SerializedScheduleActionDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    new SerializedScheduleActionDefinition(typeof(IComparable).GetMethod("CompareTo")),
                    new SerializedScheduleActionDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedScheduleActionDefinition(GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleActionDefinition first = null;
            var second = new SerializedScheduleActionDefinition(GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            SerializedScheduleActionDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            var second = new SerializedScheduleActionDefinition(GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            var second = new SerializedScheduleActionDefinition(GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleActionDefinition first = null;
            var second = new SerializedScheduleActionDefinition(GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            SerializedScheduleActionDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            var second = new SerializedScheduleActionDefinition(GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            var second = new SerializedScheduleActionDefinition(GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = new SerializedScheduleActionDefinition(GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual(new SerializedMethodDefinition(GetMethodForInt()), obj.Method);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            object second = new SerializedScheduleActionDefinition(GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            object second = new SerializedScheduleActionDefinition(GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedScheduleActionDefinition(GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
