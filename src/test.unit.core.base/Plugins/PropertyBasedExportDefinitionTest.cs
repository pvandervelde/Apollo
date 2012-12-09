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
    public sealed class PropertyBasedExportDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<PropertyBasedExportDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<PropertyBasedExportDefinition> 
                    {
                        PropertyBasedExportDefinition.CreateDefinition("A", typeof(string).GetProperty("Length")),
                        PropertyBasedExportDefinition.CreateDefinition("B", typeof(Version).GetProperty("Build")),
                        PropertyBasedExportDefinition.CreateDefinition("C", typeof(List<int>).GetProperty("Count")),
                        PropertyBasedExportDefinition.CreateDefinition("D", typeof(TimeZone).GetProperty("StandardName")),
                        PropertyBasedExportDefinition.CreateDefinition("E", typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializableExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    PropertyBasedExportDefinition.CreateDefinition("A", typeof(string).GetProperty("Length")),
                    PropertyBasedExportDefinition.CreateDefinition("B", typeof(Version).GetProperty("Build")),
                    PropertyBasedExportDefinition.CreateDefinition("C", typeof(List<int>).GetProperty("Count")),
                    PropertyBasedExportDefinition.CreateDefinition("D", typeof(TimeZone).GetProperty("StandardName")),
                    PropertyBasedExportDefinition.CreateDefinition("E", typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            PropertyBasedExportDefinition first = null;
            var second = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            PropertyBasedExportDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = PropertyBasedExportDefinition.CreateDefinition("B", GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            PropertyBasedExportDefinition first = null;
            var second = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            PropertyBasedExportDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = PropertyBasedExportDefinition.CreateDefinition("B", GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(GetPropertyForString()), obj.Property);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            object second = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            object second = PropertyBasedExportDefinition.CreateDefinition("B", GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
