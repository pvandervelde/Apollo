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
    public sealed class PropertyDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<PropertyDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<PropertyDefinition> 
                    {
                        PropertyDefinition.CreateDefinition(typeof(string).GetProperty("Length")),
                        PropertyDefinition.CreateDefinition(typeof(Version).GetProperty("Build")),
                        PropertyDefinition.CreateDefinition(typeof(List<int>).GetProperty("Count")),
                        PropertyDefinition.CreateDefinition(typeof(TimeZone).GetProperty("StandardName")),
                        PropertyDefinition.CreateDefinition(typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<PropertyDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    PropertyDefinition.CreateDefinition(typeof(string).GetProperty("Length")),
                    PropertyDefinition.CreateDefinition(typeof(Version).GetProperty("Build")),
                    PropertyDefinition.CreateDefinition(typeof(List<int>).GetProperty("Count")),
                    PropertyDefinition.CreateDefinition(typeof(TimeZone).GetProperty("StandardName")),
                    PropertyDefinition.CreateDefinition(typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            PropertyDefinition first = null;
            var second = PropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            PropertyDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = PropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = PropertyDefinition.CreateDefinition(GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            PropertyDefinition first = null;
            var second = PropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            PropertyDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = PropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = PropertyDefinition.CreateDefinition(GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual(property.Name, obj.PropertyName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.PropertyType), obj.PropertyType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            object second = PropertyDefinition.CreateDefinition(GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            object second = PropertyDefinition.CreateDefinition(GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
