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

namespace Apollo.Core.Base.Communication
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointIdTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<EndpointId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<EndpointId> 
                        {
                            new EndpointId("a"),
                            new EndpointId("b"),
                            new EndpointId("c"),
                            new EndpointId("d"),
                            new EndpointId("e"),
                            new EndpointId("f"),
                            new EndpointId("g"),
                            new EndpointId("h"),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<EndpointId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new EndpointId("a"),
                        new EndpointId("b"),
                        new EndpointId("c"),
                        new EndpointId("d"),
                        new EndpointId("e"),
                        new EndpointId("f"),
                        new EndpointId("g"),
                        new EndpointId("h"),
                    },
        };

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            EndpointId first = null;
            EndpointId second = new EndpointId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            EndpointId first = new EndpointId("a");
            EndpointId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            EndpointId first = null;
            EndpointId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            EndpointId first = new EndpointId("a");
            EndpointId second = new EndpointId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            EndpointId first = new EndpointId("b");
            EndpointId second = new EndpointId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            EndpointId first = new EndpointId("a");
            EndpointId second = new EndpointId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            EndpointId first = null;
            EndpointId second = new EndpointId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            EndpointId first = new EndpointId("a");
            EndpointId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            EndpointId first = null;
            EndpointId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            EndpointId first = new EndpointId("a");
            EndpointId second = new EndpointId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            EndpointId first = new EndpointId("b");
            EndpointId second = new EndpointId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            EndpointId first = new EndpointId("a");
            EndpointId second = new EndpointId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            EndpointId first = new EndpointId("a");
            EndpointId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            EndpointId first = new EndpointId("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            EndpointId first = new EndpointId("a");
            object second = new EndpointId("a");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            EndpointId first = new EndpointId("b");
            object second = new EndpointId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            EndpointId first = new EndpointId("a");
            object second = new EndpointId("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            EndpointId first = new EndpointId("a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
