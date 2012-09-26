//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedAssemblyDefinitionTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedAssemblyDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedAssemblyDefinition> 
                    {
                        SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly),
                        SerializedAssemblyDefinition.CreateDefinition(typeof(ExportAttribute).Assembly),
                        SerializedAssemblyDefinition.CreateDefinition(typeof(TestFixtureAttribute).Assembly),
                        SerializedAssemblyDefinition.CreateDefinition(typeof(BigInteger).Assembly),
                        SerializedAssemblyDefinition.CreateDefinition(typeof(SerializedAssemblyDefinition).Assembly),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedAssemblyDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly),
                    SerializedAssemblyDefinition.CreateDefinition(typeof(ExportAttribute).Assembly),
                    SerializedAssemblyDefinition.CreateDefinition(typeof(TestFixtureAttribute).Assembly),
                    SerializedAssemblyDefinition.CreateDefinition(typeof(BigInteger).Assembly),
                    SerializedAssemblyDefinition.CreateDefinition(typeof(SerializedAssemblyDefinition).Assembly),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedAssemblyDefinition first = null;
            var second = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            SerializedAssemblyDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            var second = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            var second = SerializedAssemblyDefinition.CreateDefinition(typeof(ExportAttribute).Assembly);

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedAssemblyDefinition first = null;
            var second = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            SerializedAssemblyDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            var second = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            var second = SerializedAssemblyDefinition.CreateDefinition(typeof(ExportAttribute).Assembly);

            Assert.IsTrue(first != second);
        }

        [Test]
        public void Create()
        {
            var obj = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);

            Assert.AreEqual(typeof(string).Assembly.GetName().Name, obj.Name);
            Assert.AreEqual(typeof(string).Assembly.GetName().Version, obj.Version);
            Assert.AreEqual(typeof(string).Assembly.GetName().CultureInfo, obj.Culture);

            var bits = typeof(string).Assembly.GetName().GetPublicKeyToken();
            var token = new StringBuilder();
            foreach (var bit in bits)
            {
                token.Append(bit.ToString("x2"));
            }

            Assert.AreEqual(token.ToString(), obj.PublicKeyToken);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            object second = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            object second = SerializedAssemblyDefinition.CreateDefinition(typeof(ExportAttribute).Assembly);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedAssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
