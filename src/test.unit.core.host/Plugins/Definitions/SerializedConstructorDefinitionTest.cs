//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Host.Plugins.Definitions
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedConstructorDefinitionTest
    {
        private static ConstructorInfo GetConstructorForString()
        {
            return typeof(string).GetConstructor(new[] { typeof(char[]) });
        }

        private static ConstructorInfo GetConstructorForObject()
        {
            return typeof(object).GetConstructor(new Type[0]);
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedConstructorDefinition>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedConstructorDefinition> 
                    {
                        SerializedConstructorDefinition.CreateDefinition(
                            typeof(string).GetConstructor(new[] 
                                { 
                                    typeof(char[])
                                })),
                        SerializedConstructorDefinition.CreateDefinition(typeof(object).GetConstructor(new Type[0])),
                        SerializedConstructorDefinition.CreateDefinition(typeof(List<int>).GetConstructor(new Type[0])),
                        SerializedConstructorDefinition.CreateDefinition(
                            typeof(Uri).GetConstructor(new[] 
                            {
                                typeof(string)
                            })),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<SerializedConstructorDefinition>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    SerializedConstructorDefinition.CreateDefinition(
                        typeof(string).GetConstructor(new[] 
                        { 
                            typeof(char[])
                        })),
                    SerializedConstructorDefinition.CreateDefinition(typeof(object).GetConstructor(new Type[0])),
                    SerializedConstructorDefinition.CreateDefinition(typeof(List<int>).GetConstructor(new Type[0])),
                    SerializedConstructorDefinition.CreateDefinition(
                            typeof(Uri).GetConstructor(new[] 
                            {
                                typeof(string)
                            })),
                },
        };

        [Test]
        public void RoundTripSerialise()
        {
            var original = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            var copy = Assert.BinarySerializeThenDeserialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            SerializedConstructorDefinition first = null;
            var second = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            SerializedConstructorDefinition second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            var second = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            var second = SerializedConstructorDefinition.CreateDefinition(GetConstructorForObject());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            SerializedConstructorDefinition first = null;
            var second = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            SerializedConstructorDefinition second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            var second = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            var second = SerializedConstructorDefinition.CreateDefinition(GetConstructorForObject());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            var constructor = GetConstructorForString();

            Assert.AreElementsEqualIgnoringOrder(
                constructor.GetParameters().Select(p => SerializedParameterDefinition.CreateDefinition(p)), 
                obj.Parameters);
            Assert.AreEqual(SerializedTypeIdentity.CreateDefinition(constructor.DeclaringType), obj.DeclaringType);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            object second = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            object second = SerializedConstructorDefinition.CreateDefinition(GetConstructorForObject());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = SerializedConstructorDefinition.CreateDefinition(GetConstructorForString());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
