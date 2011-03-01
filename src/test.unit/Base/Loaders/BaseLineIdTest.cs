//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Loaders;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Base.Loaders
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [Description("Tests the BaseLineId class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class BaseLineIdTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<BaseLineId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<BaseLineId> 
                        {
                            new BaseLineId(0),
                            new BaseLineId(1),
                            new BaseLineId(2),
                            new BaseLineId(3),
                            new BaseLineId(4),
                            new BaseLineId(5),
                            new BaseLineId(6),
                            new BaseLineId(7),
                            new BaseLineId(8),
                            new BaseLineId(9),
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<BaseLineId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new BaseLineId(0),
                        new BaseLineId(1),
                        new BaseLineId(2),
                        new BaseLineId(3),
                        new BaseLineId(4),
                        new BaseLineId(5),
                        new BaseLineId(6),
                        new BaseLineId(7),
                        new BaseLineId(8),
                        new BaseLineId(9),
                    },
        };

        [Test]
        [Description("Checks that the > operator returns false if the first object is null.")]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            BaseLineId first = null;
            BaseLineId second = new BaseLineId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the second object is null.")]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            BaseLineId first = new BaseLineId(10);
            BaseLineId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are null.")]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            BaseLineId first = null;
            BaseLineId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are equal.")]
        public void LargerThanOperatorWithEqualObjects()
        {
            BaseLineId first = new BaseLineId(10);
            BaseLineId second = new BaseLineId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the first object is larger than the second.")]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            BaseLineId first = new BaseLineId(11);
            BaseLineId second = new BaseLineId(10);

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is smaller than the second.")]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            BaseLineId first = new BaseLineId(9);
            BaseLineId second = new BaseLineId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is null.")]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            BaseLineId first = null;
            BaseLineId second = new BaseLineId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the second object is null.")]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            BaseLineId first = new BaseLineId(10);
            BaseLineId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are null.")]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            BaseLineId first = null;
            BaseLineId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are equal.")]
        public void SmallerThanOperatorWithEqualObjects()
        {
            BaseLineId first = new BaseLineId(10);
            BaseLineId second = new BaseLineId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is larger than the second.")]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            BaseLineId first = new BaseLineId(11);
            BaseLineId second = new BaseLineId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns true if the first object is smaller than the second.")]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            BaseLineId first = new BaseLineId(9);
            BaseLineId second = new BaseLineId(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        [Description("Checks that the Clone method returns an exact copy of the original object.")]
        public void Clone()
        {
            BaseLineId first = new BaseLineId(10);
            BaseLineId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        [Description("Checks that the CompareTo method returns 1 if the second objects is null.")]
        public void CompareToWithNullObject()
        {
            BaseLineId first = new BaseLineId(10);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 0 if the second object is equal to the first.")]
        public void CompareToOperatorWithEqualObjects()
        {
            BaseLineId first = new BaseLineId(10);
            object second = new BaseLineId(10);

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns a postive number if the first objects is larger than the second.")]
        public void CompareToWithLargerFirstObject()
        {
            BaseLineId first = new BaseLineId(11);
            object second = new BaseLineId(10);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        [Description("Checks that the CompareTo method returns a negative number if the first objects is larger than the second.")]
        public void CompareToWithSmallerFirstObject()
        {
            BaseLineId first = new BaseLineId(10);
            object second = new BaseLineId(11);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        [Description("Checks that the CompareTo method throws an exception if the second objects type is not equal to the first.")]
        public void CompareToWithUnequalObjectTypes()
        {
            BaseLineId first = new BaseLineId(10);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
