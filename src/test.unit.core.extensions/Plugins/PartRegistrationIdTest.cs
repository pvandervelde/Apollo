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

namespace Apollo.Core.Extensions.Plugins
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class PartRegistrationIdTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<PartRegistrationId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<PartRegistrationId> 
                        {
                            new PartRegistrationId(typeof(string).FullName, 0),
                            new PartRegistrationId(typeof(int).FullName, 0),
                            new PartRegistrationId(typeof(string).FullName, 1),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<PartRegistrationId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new PartRegistrationId(typeof(string).FullName, 0),
                        new PartRegistrationId(typeof(int).FullName, 0),
                        new PartRegistrationId(typeof(string).FullName, 1),
                    },
        };

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            PartRegistrationId first = null;
            PartRegistrationId second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            PartRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            PartRegistrationId first = null;
            PartRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 1);
            var second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = new PartRegistrationId(typeof(string).FullName, 1);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            PartRegistrationId first = null;
            PartRegistrationId second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            PartRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            PartRegistrationId first = null;
            PartRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 1);
            var second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = new PartRegistrationId(typeof(string).FullName, 1);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            PartRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 1);
            var second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = new PartRegistrationId(typeof(string).FullName, 1);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
