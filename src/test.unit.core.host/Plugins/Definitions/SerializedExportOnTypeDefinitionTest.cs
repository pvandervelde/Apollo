//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedExportOnTypeDefinitionTest
    {
        public sealed class Nested<TKey, TValue>
        {
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedExportOnTypeDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedExportOnTypeDefinition> 
                    {
                        SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string)),
                        SerializedExportOnTypeDefinition.CreateDefinition("B", typeof(float), typeof(object)),
                        SerializedExportOnTypeDefinition.CreateDefinition("C", typeof(short), typeof(int)),
                        SerializedExportOnTypeDefinition.CreateDefinition("D", typeof(byte), typeof(IComparable)),
                        SerializedExportOnTypeDefinition.CreateDefinition("E", typeof(TimeSpan), typeof(IComparable<>)),
                        SerializedExportOnTypeDefinition.CreateDefinition("F", typeof(Version), typeof(List<int>)),
                        SerializedExportOnTypeDefinition.CreateDefinition("G", typeof(List<>), typeof(double)),
                        SerializedExportOnTypeDefinition.CreateDefinition("H", typeof(IEnumerable<>), typeof(void)),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedExportDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string)),
                    SerializedExportOnTypeDefinition.CreateDefinition("B", typeof(float), typeof(object)),
                    SerializedExportOnTypeDefinition.CreateDefinition("C", typeof(short), typeof(int)),
                    SerializedExportOnTypeDefinition.CreateDefinition("D", typeof(byte), typeof(IComparable)),
                    SerializedExportOnTypeDefinition.CreateDefinition("E", typeof(TimeSpan), typeof(IComparable<>)),
                    SerializedExportOnTypeDefinition.CreateDefinition("F", typeof(Version), typeof(List<int>)),
                    SerializedExportOnTypeDefinition.CreateDefinition("G", typeof(List<>), typeof(double)),
                    SerializedExportOnTypeDefinition.CreateDefinition("H", typeof(IEnumerable<>), typeof(void)),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedExportOnTypeDefinition first = null;
            var second = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            SerializedExportOnTypeDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            var second = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            var second = SerializedExportOnTypeDefinition.CreateDefinition("B", typeof(float), typeof(object));

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedExportOnTypeDefinition first = null;
            var second = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            SerializedExportOnTypeDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            var second = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            var second = SerializedExportOnTypeDefinition.CreateDefinition("B", typeof(float), typeof(object));

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(List<int>));

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(long)), obj.ContractType);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(List<int>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), type);

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(long)), obj.ContractType);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(Nested<,>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(IEnumerable<>));

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(long)), obj.ContractType);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(typeof(IEnumerable<>)), obj.DeclaringType);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            object second = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            object second = SerializedExportOnTypeDefinition.CreateDefinition("B", typeof(float), typeof(object));

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedExportOnTypeDefinition.CreateDefinition("A", typeof(long), typeof(string));
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
