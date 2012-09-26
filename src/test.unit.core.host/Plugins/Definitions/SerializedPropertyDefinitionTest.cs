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
    public sealed class SerializedPropertyDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedPropertyDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedPropertyDefinition> 
                    {
                        SerializedPropertyDefinition.CreateDefinition(typeof(string).GetProperty("Length")),
                        SerializedPropertyDefinition.CreateDefinition(typeof(Version).GetProperty("Build")),
                        SerializedPropertyDefinition.CreateDefinition(typeof(List<int>).GetProperty("Count")),
                        SerializedPropertyDefinition.CreateDefinition(typeof(TimeZone).GetProperty("StandardName")),
                        SerializedPropertyDefinition.CreateDefinition(typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedPropertyDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedPropertyDefinition.CreateDefinition(typeof(string).GetProperty("Length")),
                    SerializedPropertyDefinition.CreateDefinition(typeof(Version).GetProperty("Build")),
                    SerializedPropertyDefinition.CreateDefinition(typeof(List<int>).GetProperty("Count")),
                    SerializedPropertyDefinition.CreateDefinition(typeof(TimeZone).GetProperty("StandardName")),
                    SerializedPropertyDefinition.CreateDefinition(typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedPropertyDefinition first = null;
            var second = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            SerializedPropertyDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = SerializedPropertyDefinition.CreateDefinition(GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedPropertyDefinition first = null;
            var second = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            SerializedPropertyDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = SerializedPropertyDefinition.CreateDefinition(GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual(property.Name, obj.PropertyName);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(property.PropertyType), obj.PropertyType);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            object second = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            object second = SerializedPropertyDefinition.CreateDefinition(GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedPropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
