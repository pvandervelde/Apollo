﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the ServiceShutdownCapabilityRequestMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationNameTest
    {
        [Test]
        [Description("Checks that the == operator returns false if the first object is null.")]
        public void EqualsOperatorWithFirstObjectNull()
        {
            NotificationName first = null;
            var second = new NotificationName("name");

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if the second object is null.")]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new NotificationName("name");
            NotificationName second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.IsTrue(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new NotificationName("name1");
            var second = new NotificationName("name2");

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the first object is null.")]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            NotificationName first = null;
            var second = new NotificationName("name");

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the second object is null.")]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new NotificationName("name");
            NotificationName second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new NotificationName("name1");
            var second = new NotificationName("name2");

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is null.")]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            NotificationName first = null;
            var second = new NotificationName("name");

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the second object is null.")]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new NotificationName("name");
            NotificationName second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are null.")]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            NotificationName first = null;
            NotificationName second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are equal.")]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the first object is larger than the second.")]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new NotificationName("b");
            var second = new NotificationName("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is smaller than the second.")]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new NotificationName("a");
            var second = new NotificationName("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is null.")]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            NotificationName first = null;
            var second = new NotificationName("name");

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the second object is null.")]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new NotificationName("name");
            NotificationName second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are null.")]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            NotificationName first = null;
            NotificationName second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are equal.")]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is larger than the second.")]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new NotificationName("b");
            var second = new NotificationName("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns true if the first object is smaller than the second.")]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new NotificationName("a");
            var second = new NotificationName("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        [Description("Checks that the Clone method returns an exact copy of the original object.")]
        public void Clone()
        {
            var first = new NotificationName("name");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is null.")]
        public void EqualsWithNullObject()
        {
            var first = new NotificationName("name");
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns true if the second object is equal to the first.")]
        public void EqualsWithEqualObjects()
        {
            var first = new NotificationName("name");
            object second = first.Clone();

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is not equal to the first.")]
        public void EqualsWithUnequalObjects()
        {
            var first = new NotificationName("name1");
            object second = new NotificationName("name2");

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects type is not equal to the first.")]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new NotificationName("name");
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 1 if the second objects is null.")]
        public void CompareToWithNullObject()
        {
            var first = new NotificationName("name");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 0 if the second object is equal to the first.")]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new NotificationName("name");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns a postive number if the first objects is larger than the second.")]
        public void CompareToWithLargerFirstObject()
        {
            var first = new NotificationName("b");
            var second = new NotificationName("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        [Description("Checks that the CompareTo method returns a negative number if the first objects is larger than the second.")]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new NotificationName("a");
            var second = new NotificationName("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        [Description("Checks that the CompareTo method throws an exception if the second objects type is not equal to the first.")]
        public void CompareToWithUnequalObjectTypes()
        {
            var first = new NotificationName("name");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}