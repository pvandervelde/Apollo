//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utils
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [Description("Tests the Id class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class IdTest
    {
        #region Internal test class

        /// <summary>
        /// Defines a mock ID number class, used in testing.
        /// </summary>
        private sealed class MockId : Id<MockId, int>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockId"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public MockId(int value)
                : base(value)
            {
            }

            /// <summary>
            /// Clones the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>A new <see cref="MockId"/> with the given value as internal ID.</returns>
            protected override MockId Clone(int value)
            {
                return new MockId(value);
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "ID: {0}", InternalValue);
            }
        } 

        #endregion

        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<MockId>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<MockId> 
                        {
                            new MockId(0),
                            new MockId(1),
                            new MockId(2),
                            new MockId(3),
                            new MockId(4),
                            new MockId(5),
                            new MockId(6),
                            new MockId(7),
                            new MockId(8),
                            new MockId(9),
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<MockId>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new MockId(0),
                        new MockId(1),
                        new MockId(2),
                        new MockId(3),
                        new MockId(4),
                        new MockId(5),
                        new MockId(6),
                        new MockId(7),
                        new MockId(8),
                        new MockId(9),
                    },
        };

        [Test]
        [Description("Checks that the > operator returns false if the first object is null.")]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            MockId first = null;
            MockId second = new MockId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the second object is null.")]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            MockId first = new MockId(10);
            MockId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are null.")]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            MockId first = null;
            MockId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are equal.")]
        public void LargerThanOperatorWithEqualObjects()
        {
            MockId first = new MockId(10);
            MockId second = new MockId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the first object is larger than the second.")]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            MockId first = new MockId(11);
            MockId second = new MockId(10);

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is smaller than the second.")]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            MockId first = new MockId(9);
            MockId second = new MockId(10);

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is null.")]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            MockId first = null;
            MockId second = new MockId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the second object is null.")]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            MockId first = new MockId(10);
            MockId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are null.")]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            MockId first = null;
            MockId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are equal.")]
        public void SmallerThanOperatorWithEqualObjects()
        {
            MockId first = new MockId(10);
            MockId second = new MockId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is larger than the second.")]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            MockId first = new MockId(11);
            MockId second = new MockId(10);

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns true if the first object is smaller than the second.")]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            MockId first = new MockId(9);
            MockId second = new MockId(10);

            Assert.IsTrue(first < second);
        }

        [Test]
        [Description("Checks that the Clone method returns an exact copy of the original object.")]
        public void Clone()
        {
            MockId first = new MockId(10);
            MockId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        [Description("Checks that the CompareTo method returns 1 if the second objects is null.")]
        public void CompareToWithNullObject()
        {
            MockId first = new MockId(10);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 0 if the second object is equal to the first.")]
        public void CompareToOperatorWithEqualObjects()
        {
            MockId first = new MockId(10);
            object second = new MockId(10);

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns a postive number if the first objects is larger than the second.")]
        public void CompareToWithLargerFirstObject()
        {
            MockId first = new MockId(11);
            object second = new MockId(10);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        [Description("Checks that the CompareTo method returns a negative number if the first objects is larger than the second.")]
        public void CompareToWithSmallerFirstObject()
        {
            MockId first = new MockId(10);
            object second = new MockId(11);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        [Description("Checks that the CompareTo method throws an exception if the second objects type is not equal to the first.")]
        public void CompareToWithUnequalObjectTypes()
        {
            MockId first = new MockId(10);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
