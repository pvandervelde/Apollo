//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Projects;
using MbUnit.Framework;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the DatasetId class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetIdTest
    {
        private static DatasetId Create(int id)
        {
            return (DatasetId)Mirror.ForType<DatasetId>().Constructor.Invoke(id);
        }

        [Test]
        [Description("Checks that the == operator returns false if the first object is null.")]
        public void EqualsOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            var second = new DatasetId();

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if the second object is null.")]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new DatasetId();
            DatasetId second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.IsTrue(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new DatasetId();
            var second = new DatasetId();

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the first object is null.")]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            var second = new DatasetId();

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the second object is null.")]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new DatasetId();
            DatasetId second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new DatasetId();
            var second = new DatasetId();

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is null.")]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            var second = new DatasetId();

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the second object is null.")]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new DatasetId();
            DatasetId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are null.")]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            DatasetId first = null;
            DatasetId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are equal.")]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the first object is larger than the second.")]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = Create(2);
            var second = Create(1);

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is smaller than the second.")]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = Create(1);
            var second = Create(2);

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is null.")]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            DatasetId first = null;
            var second = new DatasetId();

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the second object is null.")]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new DatasetId();
            DatasetId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are null.")]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            DatasetId first = null;
            DatasetId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are equal.")]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is larger than the second.")]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = Create(2);
            var second = Create(1);

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns true if the first object is smaller than the second.")]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = Create(1);
            var second = Create(2);

            Assert.IsTrue(first < second);
        }

        [Test]
        [Description("Checks that the IsValid method returns the correct value.")]
        public void IsValid()
        {
            Assert.IsTrue(new DatasetId().IsValid());

            var id = new DatasetId();
            Assert.IsTrue(id.IsValid());

            id.Invalidate();
            Assert.IsFalse(id.IsValid());
        }

        [Test]
        [Description("Checks that the Clone method returns an exact copy of the original object.")]
        public void Clone()
        {
            var first = new DatasetId();
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        [Description("Checks that the Clone method returns an exact copy of the original object.")]
        public void CloneInvalidId()
        {
            var first = new DatasetId();
            first.Invalidate();

            var second = first.Clone();
            Assert.AreEqual(first, second);
            Assert.IsFalse(second.IsValid());
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is null.")]
        public void EqualsWithNullObject()
        {
            var first = new DatasetId();
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns true if the second object is equal to the first.")]
        public void EqualsWithEqualObjects()
        {
            var first = new DatasetId();
            object second = first.Clone();

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is not equal to the first.")]
        public void EqualsWithUnequalObjects()
        {
            var first = new DatasetId();
            object second = new DatasetId();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects type is not equal to the first.")]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new DatasetId();
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 1 if the second objects is null.")]
        public void CompareToWithNullObject()
        {
            var first = new DatasetId();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 0 if the second object is equal to the first.")]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new DatasetId();
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns a postive number if the first objects is larger than the second.")]
        public void CompareToWithLargerFirstObject()
        {
            var first = Create(2);
            var second = Create(1);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        [Description("Checks that the CompareTo method returns a negative number if the first objects is larger than the second.")]
        public void CompareToWithSmallerFirstObject()
        {
            var first = Create(1);
            var second = Create(2);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        [Description("Checks that the CompareTo method throws an exception if the second objects type is not equal to the first.")]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new DatasetId();
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
