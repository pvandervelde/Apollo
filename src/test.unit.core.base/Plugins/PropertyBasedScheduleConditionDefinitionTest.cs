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
    public sealed class PropertyBasedScheduleConditionDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<PropertyBasedScheduleConditionDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<PropertyBasedScheduleConditionDefinition> 
                    {
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("a", typeof(string).GetProperty("Length")),
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("b", typeof(Version).GetProperty("Build")),
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("c", typeof(List<int>).GetProperty("Count")),
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("d", typeof(TimeZone).GetProperty("StandardName")),
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("e", typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ScheduleConditionDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    PropertyBasedScheduleConditionDefinition.CreateDefinition("a", typeof(string).GetProperty("Length")),
                    PropertyBasedScheduleConditionDefinition.CreateDefinition("b", typeof(Version).GetProperty("Build")),
                    PropertyBasedScheduleConditionDefinition.CreateDefinition("c", typeof(List<int>).GetProperty("Count")),
                    PropertyBasedScheduleConditionDefinition.CreateDefinition("d", typeof(TimeZone).GetProperty("StandardName")),
                    PropertyBasedScheduleConditionDefinition.CreateDefinition("e", typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            PropertyBasedScheduleConditionDefinition first = null;
            var second = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            PropertyBasedScheduleConditionDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var second = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var second = PropertyBasedScheduleConditionDefinition.CreateDefinition("b", GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            PropertyBasedScheduleConditionDefinition first = null;
            var second = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            PropertyBasedScheduleConditionDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var second = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var second = PropertyBasedScheduleConditionDefinition.CreateDefinition("b", GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("a", obj.ContractName);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(property), obj.Property);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            object second = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            object second = PropertyBasedScheduleConditionDefinition.CreateDefinition("b", GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
