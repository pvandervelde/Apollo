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
    public sealed class ObjectRegistrationIdTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ObjectRegistrationId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ObjectRegistrationId> 
                        {
                            new ObjectRegistrationId(typeof(string).FullName, 0),
                            new ObjectRegistrationId(typeof(int).FullName, 0),
                            new ObjectRegistrationId(typeof(string).FullName, 1),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ObjectRegistrationId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new ObjectRegistrationId(typeof(string).FullName, 0),
                        new ObjectRegistrationId(typeof(int).FullName, 0),
                        new ObjectRegistrationId(typeof(string).FullName, 1),
                    },
        };

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            ObjectRegistrationId first = null;
            ObjectRegistrationId second = new ObjectRegistrationId(typeof(string).FullName, 0);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ObjectRegistrationId first = new ObjectRegistrationId(typeof(string).FullName, 0);
            ObjectRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ObjectRegistrationId first = null;
            ObjectRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 0);
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 1);
            var second = new ObjectRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 0);
            var second = new ObjectRegistrationId(typeof(string).FullName, 1);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ObjectRegistrationId first = null;
            ObjectRegistrationId second = new ObjectRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ObjectRegistrationId first = new ObjectRegistrationId(typeof(string).FullName, 0);
            ObjectRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ObjectRegistrationId first = null;
            ObjectRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 0);
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 1);
            var second = new ObjectRegistrationId(typeof(string).FullName, 0);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 0);
            var second = new ObjectRegistrationId(typeof(string).FullName, 1);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ObjectRegistrationId first = new ObjectRegistrationId(typeof(string).FullName, 0);
            ObjectRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ObjectRegistrationId first = new ObjectRegistrationId(typeof(string).FullName, 0);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 0);
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 1);
            var second = new ObjectRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ObjectRegistrationId(typeof(string).FullName, 0);
            var second = new ObjectRegistrationId(typeof(string).FullName, 1);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ObjectRegistrationId first = new ObjectRegistrationId(typeof(string).FullName, 0);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
