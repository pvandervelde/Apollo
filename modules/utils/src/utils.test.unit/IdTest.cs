//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MbUnit.Framework;

namespace Apollo.Utils
{
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

        [Test]
        [Description("Checks that the == operator returns false if the first object is null.")]
        public void EqualsOperatorWithFirstObjectNull()
        {
            MockId first = null;
            MockId second = new MockId(10);

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if the second object is null.")]
        public void EqualsOperatorWithSecondObjectNull()
        {
            MockId first = new MockId(10);
            MockId second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            MockId first = new MockId(10);
            MockId second = new MockId(10);

            Assert.IsTrue(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            MockId first = new MockId(11);
            MockId second = new MockId(10);

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the first object is null.")]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            MockId first = null;
            MockId second = new MockId(10);

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the second object is null.")]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            MockId first = new MockId(10);
            MockId second = null;

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            MockId first = new MockId(10);
            MockId second = new MockId(10);

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            MockId first = new MockId(11);
            MockId second = new MockId(10);

            Assert.IsTrue(first != second);
        }

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

            Assert.IsFalse(first > second);
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
        [Description("Checks that the Equals method returns false if the second objects is null.")]
        public void EqualsWithNullObject()
        {
            MockId first = new MockId(10);
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns true if the second object is equal to the first.")]
        public void EqualsWithEqualObjects()
        {
            MockId first = new MockId(10);
            object second = new MockId(10);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is not equal to the first.")]
        public void EqualsWithUnequalObjects()
        {
            MockId first = new MockId(10);
            object second = new MockId(11);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects type is not equal to the first.")]
        public void EqualsWithUnequalObjectTypes()
        {
            MockId first = new MockId(10);
            object second = new object();

            Assert.IsFalse(first.Equals(second));
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
