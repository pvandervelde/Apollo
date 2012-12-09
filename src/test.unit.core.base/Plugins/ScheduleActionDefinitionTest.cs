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

namespace Apollo.Core.Base.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleActionDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ScheduleActionDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ScheduleActionDefinition> 
                    {
                        ScheduleActionDefinition.CreateDefinition("a", typeof(string).GetMethod("Contains")),
                        ScheduleActionDefinition.CreateDefinition("b", typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        ScheduleActionDefinition.CreateDefinition("c", typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        ScheduleActionDefinition.CreateDefinition("d", typeof(IComparable).GetMethod("CompareTo")),
                        ScheduleActionDefinition.CreateDefinition("e", typeof(IComparable<>).GetMethod("CompareTo")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializableExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    ScheduleActionDefinition.CreateDefinition("a", typeof(string).GetMethod("Contains")),
                    ScheduleActionDefinition.CreateDefinition("b", typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                    ScheduleActionDefinition.CreateDefinition("c", typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                    ScheduleActionDefinition.CreateDefinition("d", typeof(IComparable).GetMethod("CompareTo")),
                    ScheduleActionDefinition.CreateDefinition("e", typeof(IComparable<>).GetMethod("CompareTo")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            ScheduleActionDefinition first = null;
            var second = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            ScheduleActionDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            var second = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            var second = ScheduleActionDefinition.CreateDefinition("b", GetMethodForDouble());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            ScheduleActionDefinition first = null;
            var second = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            ScheduleActionDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            var second = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            var second = ScheduleActionDefinition.CreateDefinition("b", GetMethodForDouble());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual("a", obj.ContractName);
            Assert.AreEqual(MethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            object second = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            object second = ScheduleActionDefinition.CreateDefinition("b", GetMethodForDouble());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
