//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messages;
using MbUnit.Framework;
using MbUnit.Framework.Reflection;

namespace Apollo.Core.Test.Unit.Messages
{
    [TestFixture]
    [Description("Tests the MessageId class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MessageIdTest
    {
        private MessageId Create(Guid id)
        { 
            var assembly = typeof(MessageId).Assembly.GetName().FullName;
            return (MessageId)Reflector.CreateInstance(assembly, typeof(MessageId).FullName, id);
        }

        [Test]
        [Description("Checks that the == operator returns false if the first object is null.")]
        public void EqualsOperatorWithFirstObjectNull()
        {
            MessageId first = null;
            MessageId second = MessageId.Next();

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if the second object is null.")]
        public void EqualsOperatorWithSecondObjectNull()
        {
            MessageId first = MessageId.Next();
            MessageId second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            MessageId first = MessageId.Next();
            MessageId second = first.Clone();

            Assert.IsTrue(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            MessageId first = MessageId.Next();
            MessageId second = MessageId.Next();

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the first object is null.")]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            MessageId first = null;
            MessageId second = MessageId.Next();

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the second object is null.")]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            MessageId first = MessageId.Next();
            MessageId second = null;

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            MessageId first = MessageId.Next();
            MessageId second = first.Clone();

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            MessageId first = MessageId.Next();
            MessageId second = MessageId.Next();

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is null.")]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            MessageId first = null;
            MessageId second = MessageId.Next();

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the second object is null.")]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            MessageId first = MessageId.Next();
            MessageId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are null.")]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            MessageId first = null;
            MessageId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if both objects are equal.")]
        public void LargerThanOperatorWithEqualObjects()
        {
            MessageId first = MessageId.Next();
            MessageId second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns true if the first object is larger than the second.")]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            Guid firstGuid = Guid.NewGuid();
            Guid secondGuid = Guid.NewGuid();

            MessageId first = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);
            MessageId second = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsTrue(first > second);
        }

        [Test]
        [Description("Checks that the > operator returns false if the first object is smaller than the second.")]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            Guid firstGuid = Guid.NewGuid();
            Guid secondGuid = Guid.NewGuid();

            MessageId first = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);
            MessageId second = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsFalse(first > second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is null.")]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            MessageId first = null;
            MessageId second = MessageId.Next();

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the second object is null.")]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            MessageId first = MessageId.Next();
            MessageId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are null.")]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            MessageId first = null;
            MessageId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if both objects are equal.")]
        public void SmallerThanOperatorWithEqualObjects()
        {
            MessageId first = MessageId.Next();
            MessageId second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns false if the first object is larger than the second.")]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            Guid firstGuid = Guid.NewGuid();
            Guid secondGuid = Guid.NewGuid();

            MessageId first = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);
            MessageId second = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsFalse(first < second);
        }

        [Test]
        [Description("Checks that the < operator returns true if the first object is smaller than the second.")]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            Guid firstGuid = Guid.NewGuid();
            Guid secondGuid = Guid.NewGuid();

            MessageId first = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);
            MessageId second = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsTrue(first < second);
        }

        [Test]
        [Description("Checks that the Clone method returns an exact copy of the original object.")]
        public void Clone()
        {
            MessageId first = MessageId.Next();
            MessageId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is null.")]
        public void EqualsWithNullObject()
        {
            MessageId first = MessageId.Next();
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns true if the second object is equal to the first.")]
        public void EqualsWithEqualObjects()
        {
            MessageId first = MessageId.Next();
            object second = first.Clone();

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is not equal to the first.")]
        public void EqualsWithUnequalObjects()
        {
            MessageId first = MessageId.Next();
            object second = MessageId.Next();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects type is not equal to the first.")]
        public void EqualsWithUnequalObjectTypes()
        {
            MessageId first = MessageId.Next();
            object second = new object();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 1 if the second objects is null.")]
        public void CompareToWithNullObject()
        {
            MessageId first = MessageId.Next();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns 0 if the second object is equal to the first.")]
        public void CompareToOperatorWithEqualObjects()
        {
            MessageId first = MessageId.Next();
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        [Description("Checks that the CompareTo method returns a postive number if the first objects is larger than the second.")]
        public void CompareToWithLargerFirstObject()
        {
            Guid firstGuid = Guid.NewGuid();
            Guid secondGuid = Guid.NewGuid();

            MessageId first = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);
            MessageId second = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        [Description("Checks that the CompareTo method returns a negative number if the first objects is larger than the second.")]
        public void CompareToWithSmallerFirstObject()
        {
            Guid firstGuid = Guid.NewGuid();
            Guid secondGuid = Guid.NewGuid();

            MessageId first = (firstGuid.CompareTo(secondGuid) < 0) ? Create(firstGuid) : Create(secondGuid);
            MessageId second = (firstGuid.CompareTo(secondGuid) > 0) ? Create(firstGuid) : Create(secondGuid);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        [Description("Checks that the CompareTo method throws an exception if the second objects type is not equal to the first.")]
        public void CompareToWithUnequalObjectTypes()
        {
            MessageId first = MessageId.Next();
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
