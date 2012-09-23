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
                        new SerializedExportOnPropertyDefinition("A", typeof(bool), typeof(string).GetProperty("Length")),
                        new SerializedExportOnPropertyDefinition("B", typeof(long), typeof(Version).GetProperty("Build")),
                        new SerializedExportOnPropertyDefinition("C", typeof(float), typeof(List<int>).GetProperty("Count")),
                        new SerializedExportOnPropertyDefinition("D", typeof(TimeSpan), typeof(TimeZone).GetProperty("StandardName")),
                        new SerializedExportOnPropertyDefinition("E", typeof(short), typeof(TimeZoneInfo).GetProperty("StandardName")),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new SerializedExportOnPropertyDefinition("A", typeof(bool), typeof(string).GetProperty("Length")),
                    new SerializedExportOnPropertyDefinition("B", typeof(long), typeof(Version).GetProperty("Build")),
                    new SerializedExportOnPropertyDefinition("C", typeof(float), typeof(List<int>).GetProperty("Count")),
                    new SerializedExportOnPropertyDefinition("D", typeof(TimeSpan), typeof(TimeZone).GetProperty("StandardName")),
                    new SerializedExportOnPropertyDefinition("E", typeof(short), typeof(TimeZoneInfo).GetProperty("StandardName")),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedExportOnPropertyDefinition first = null;
            var second = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            SerializedExportOnPropertyDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            var second = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            var second = new SerializedExportOnPropertyDefinition("B", typeof(long), GetPropertyForVersion());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedExportOnPropertyDefinition first = null;
            var second = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            SerializedExportOnPropertyDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            var second = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            var second = new SerializedExportOnPropertyDefinition("B", typeof(long), GetPropertyForVersion());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(new SerializedTypeIdentity(typeof(bool)), obj.ContractType);
            Assert.AreEqual(new SerializedTypeIdentity(property.DeclaringType), obj.DeclaringType);
            Assert.AreEqual(new SerializedPropertyDefinition(GetPropertyForString()), obj.Property);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            object second = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            object second = new SerializedExportOnPropertyDefinition("B", typeof(long), GetPropertyForVersion());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new SerializedExportOnPropertyDefinition("A", typeof(bool), GetPropertyForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
