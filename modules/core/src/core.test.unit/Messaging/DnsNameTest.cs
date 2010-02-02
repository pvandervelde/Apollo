//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using MbUnit.Framework;

namespace Apollo.Core.Test.Unit.Messaging
{
    [TestFixture]
    [Description("Tests the DnsName class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DnsNameTest
    {
        [Test]
        [Description("Checks that the == operator returns false if the first object is null.")]
        public void EqualsOperatorWithFirstObjectNull()
        {
            DnsName first = null;
            var second = new DnsName("name");

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if the second object is null.")]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new DnsName("name");
            DnsName second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            var first = new DnsName("name");
            var second = new DnsName("name");

            Assert.IsTrue(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new DnsName("name");
            var second = new DnsName("otherName");

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the first object is null.")]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            DnsName first = null;
            var second = new DnsName("name");

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the second object is null.")]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new DnsName("name");
            DnsName second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            var first = new DnsName("name");
            var second = new DnsName("name");

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new DnsName("name");
            var second = new DnsName("otherName");

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is null.")]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            DnsName first = null;
            var second = new DnsName("name");

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the second object is null.")]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            var first = new DnsName("name");
            DnsName second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are null.")]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            DnsName first = null;
            DnsName second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are equal.")]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new DnsName("name");
            var second = new DnsName("name");

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the first object is larger than the second.")]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new DnsName("b");
            var second = new DnsName("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is smaller than the second.")]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new DnsName("a");
            var second = new DnsName("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is null.")]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            DnsName first = null;
            var second = new DnsName("name");

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the second object is null.")]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            var first = new DnsName("name");
            DnsName second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are null.")]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            DnsName first = null;
            DnsName second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are equal.")]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new DnsName("name");
            var second = new DnsName("name");

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is larger than the second.")]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new DnsName("b");
            var second = new DnsName("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns true if the first object is smaller than the second.")]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new DnsName("a");
            var second = new DnsName("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        [Description("Checks that the Clone method returns an exact copy of the original object.")]
        public void Clone()
        {
            var first = new DnsName("name");
            var second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is null.")]
        public void EqualsWithNullObject()
        {
            var first = new DnsName("name");
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns true if the second object is equal to the first.")]
        public void EqualsWithEqualObjects()
        {
            var first = new DnsName("name");
            object second = new DnsName("name");

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is not equal to the first.")]
        public void EqualsWithUnequalObjects()
        {
            var first = new DnsName("name");
            object second = new DnsName("otherName");

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects type is not equal to the first.")]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new DnsName("name");
            object second = new object();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 1 if the second objects is null.")]
        public void CompareToWithNullObject()
        {
            IComparable first = new DnsName("name");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 0 if the second object is equal to the first.")]
        public void CompareToOperatorWithEqualObjects()
        {
            IComparable first = new DnsName("name");
            object second = new DnsName("name");

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns a postive number if the first objects is larger than the second.")]
        public void CompareToWithLargerFirstObject()
        {
            IComparable first = new DnsName("b");
            object second = new DnsName("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        [Description("Checks that the CompareTo method returns a negative number if the first objects is larger than the second.")]
        public void CompareToWithSmallerFirstObject()
        {
            IComparable first = new DnsName("a");
            object second = new DnsName("b");

            Assert.IsTrue("a".CompareTo("b") < 0);
            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        [Description("Checks that the CompareTo method throws an exception if the second objects type is not equal to the first.")]
        public void CompareToWithUnequalObjectTypes()
        {
            IComparable first = new DnsName("name");
            var second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
