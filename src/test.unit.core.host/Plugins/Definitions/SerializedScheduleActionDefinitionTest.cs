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
                        SerializedScheduleActionDefinition.CreateDefinition(typeof(string).GetMethod("Contains")),
                        SerializedScheduleActionDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        SerializedScheduleActionDefinition.CreateDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        SerializedScheduleActionDefinition.CreateDefinition(typeof(IComparable).GetMethod("CompareTo")),
                        SerializedScheduleActionDefinition.CreateDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedScheduleActionDefinition.CreateDefinition(typeof(string).GetMethod("Contains")),
                    SerializedScheduleActionDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    SerializedScheduleActionDefinition.CreateDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    SerializedScheduleActionDefinition.CreateDefinition(typeof(IComparable).GetMethod("CompareTo")),
                    SerializedScheduleActionDefinition.CreateDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleActionDefinition first = null;
            var second = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            SerializedScheduleActionDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            var second = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            var second = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleActionDefinition first = null;
            var second = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            SerializedScheduleActionDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            var second = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            var second = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual(SerializedMethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            object second = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            object second = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedScheduleActionDefinition.CreateDefinition(GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
