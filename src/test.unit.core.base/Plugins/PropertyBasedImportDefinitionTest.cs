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
    public sealed class PropertyBasedImportDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<PropertyBasedImportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<PropertyBasedImportDefinition> 
                    {
                        PropertyBasedImportDefinition.CreateDefinition("A", typeof(string).GetProperty("Length")),
                        PropertyBasedImportDefinition.CreateDefinition("B", typeof(Version).GetProperty("Build")),
                        PropertyBasedImportDefinition.CreateDefinition("C", typeof(List<int>).GetProperty("Count")),
                        PropertyBasedImportDefinition.CreateDefinition("D", typeof(TimeZone).GetProperty("StandardName")),
                        PropertyBasedImportDefinition.CreateDefinition("E", typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializableImportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    PropertyBasedImportDefinition.CreateDefinition("A", typeof(string).GetProperty("Length")),
                    PropertyBasedImportDefinition.CreateDefinition("B", typeof(Version).GetProperty("Build")),
                    PropertyBasedImportDefinition.CreateDefinition("C", typeof(List<int>).GetProperty("Count")),
                    PropertyBasedImportDefinition.CreateDefinition("D", typeof(TimeZone).GetProperty("StandardName")),
                    PropertyBasedImportDefinition.CreateDefinition("E", typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            PropertyBasedImportDefinition first = null;
            var second = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            PropertyBasedImportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = PropertyBasedImportDefinition.CreateDefinition("B", GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            PropertyBasedImportDefinition first = null;
            var second = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            PropertyBasedImportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = PropertyBasedImportDefinition.CreateDefinition("B", GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(property), obj.Property);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            object second = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            object second = PropertyBasedImportDefinition.CreateDefinition("B", GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = PropertyBasedImportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
