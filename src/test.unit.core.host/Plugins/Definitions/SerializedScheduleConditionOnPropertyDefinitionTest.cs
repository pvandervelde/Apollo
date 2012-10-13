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
    public sealed class SerializedScheduleConditionOnPropertyDefinitionTest
    {
        private static PropertyInfo GetPropertyForString()
        {
            return typeof(string).GetProperty("Length");
        }

        private static PropertyInfo GetPropertyForVersion()
        {
            return typeof(Version).GetProperty("Build");
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedScheduleConditionOnPropertyDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedScheduleConditionOnPropertyDefinition> 
                    {
                        SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", typeof(string).GetProperty("Length")),
                        SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("b", typeof(Version).GetProperty("Build")),
                        SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("c", typeof(List<int>).GetProperty("Count")),
                        SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("d", typeof(TimeZone).GetProperty("StandardName")),
                        SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("e", typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedScheduleConditionDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", typeof(string).GetProperty("Length")),
                    SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("b", typeof(Version).GetProperty("Build")),
                    SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("c", typeof(List<int>).GetProperty("Count")),
                    SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("d", typeof(TimeZone).GetProperty("StandardName")),
                    SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("e", typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleConditionOnPropertyDefinition first = null;
            var second = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            SerializedScheduleConditionOnPropertyDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            var second = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            var second = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("b", GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleConditionOnPropertyDefinition first = null;
            var second = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            SerializedScheduleConditionOnPropertyDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            var second = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            var second = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("b", GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("a", obj.ContractName);
            Assert.AreEqual(SerializedPropertyDefinition.CreateDefinition(property), obj.Property);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            object second = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            object second = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("b", GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition("a", GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
