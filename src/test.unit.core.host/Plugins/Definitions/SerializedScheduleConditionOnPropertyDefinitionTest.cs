﻿//-----------------------------------------------------------------------
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
                        new SerializedScheduleConditionOnPropertyDefinition(typeof(string).GetProperty("Length")),
                        new SerializedScheduleConditionOnPropertyDefinition(typeof(Version).GetProperty("Build")),
                        new SerializedScheduleConditionOnPropertyDefinition(typeof(List<int>).GetProperty("Count")),
                        new SerializedScheduleConditionOnPropertyDefinition(typeof(TimeZone).GetProperty("StandardName")),
                        new SerializedScheduleConditionOnPropertyDefinition(typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedScheduleConditionDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedScheduleConditionOnPropertyDefinition(typeof(string).GetProperty("Length")),
                    new SerializedScheduleConditionOnPropertyDefinition(typeof(Version).GetProperty("Build")),
                    new SerializedScheduleConditionOnPropertyDefinition(typeof(List<int>).GetProperty("Count")),
                    new SerializedScheduleConditionOnPropertyDefinition(typeof(TimeZone).GetProperty("StandardName")),
                    new SerializedScheduleConditionOnPropertyDefinition(typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleConditionOnPropertyDefinition first = null;
            var second = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            SerializedScheduleConditionOnPropertyDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            var second = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            var second = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedScheduleConditionOnPropertyDefinition first = null;
            var second = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            SerializedScheduleConditionOnPropertyDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            var second = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            var second = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual(new SerializedPropertyDefinition(property), obj.Property);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            object second = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            object second = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedScheduleConditionOnPropertyDefinition(GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
