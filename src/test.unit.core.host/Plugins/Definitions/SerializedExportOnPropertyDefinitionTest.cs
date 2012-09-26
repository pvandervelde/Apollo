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
    public sealed class SerializedExportOnPropertyDefinitionTest
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
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedExportOnPropertyDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedExportOnPropertyDefinition> 
                    {
                        SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), typeof(string).GetProperty("Length")),
                        SerializedExportOnPropertyDefinition.CreateDefinition("B", typeof(long), typeof(Version).GetProperty("Build")),
                        SerializedExportOnPropertyDefinition.CreateDefinition("C", typeof(float), typeof(List<int>).GetProperty("Count")),
                        SerializedExportOnPropertyDefinition.CreateDefinition("D", typeof(TimeSpan), typeof(TimeZone).GetProperty("StandardName")),
                        SerializedExportOnPropertyDefinition.CreateDefinition("E", typeof(short), typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), typeof(string).GetProperty("Length")),
                    SerializedExportOnPropertyDefinition.CreateDefinition("B", typeof(long), typeof(Version).GetProperty("Build")),
                    SerializedExportOnPropertyDefinition.CreateDefinition("C", typeof(float), typeof(List<int>).GetProperty("Count")),
                    SerializedExportOnPropertyDefinition.CreateDefinition("D", typeof(TimeSpan), typeof(TimeZone).GetProperty("StandardName")),
                    SerializedExportOnPropertyDefinition.CreateDefinition("E", typeof(short), typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedExportOnPropertyDefinition first = null;
            var second = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            SerializedExportOnPropertyDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            var second = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            var second = SerializedExportOnPropertyDefinition.CreateDefinition("B", typeof(long), GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedExportOnPropertyDefinition first = null;
            var second = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            SerializedExportOnPropertyDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            var second = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            var second = SerializedExportOnPropertyDefinition.CreateDefinition("B", typeof(long), GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(bool)), obj.ContractType);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
            Assert.AreEqual(SerializedPropertyDefinition.CreateDefinition(GetPropertyForString()), obj.Property);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            object second = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            object second = SerializedExportOnPropertyDefinition.CreateDefinition("B", typeof(long), GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedExportOnPropertyDefinition.CreateDefinition("A", typeof(bool), GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
