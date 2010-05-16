//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the ApplicationShutdownCapabilityResponseMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ApplicationShutdownCapabilityResponseMessageTest
    {
        [Test]
        [Description("Checks that the shutdown state is stored properly.")]
        public void IsShutdownForced()
        {
            var message = new ApplicationShutdownCapabilityResponseMessage(true);
            Assert.IsTrue(message.CanShutdown);
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var message = new ApplicationShutdownCapabilityResponseMessage(false);
            object nullReference = null;

            Assert.IsFalse(message.Equals(nullReference));
        }

        [Test]
        [Description("Checks that a message is not equal to an object of a different type.")]
        public void EqualsWithDifferentType()
        {
            var message = new ApplicationShutdownCapabilityResponseMessage(false);
            object obj = new object();

            Assert.IsFalse(message.Equals(obj));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal object of equal type.")]
        public void EqualsWithNonEqualObjects()
        {
            var message1 = new ApplicationShutdownCapabilityResponseMessage(false);
            var message2 = new ApplicationShutdownCapabilityResponseMessage(true);

            Assert.IsFalse(message1.Equals((object)message2));
            Assert.IsFalse(message2.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is equal to an equal object of equal type.")]
        public void EqualsWithEqualObjects()
        {
            var message1 = new ApplicationShutdownCapabilityResponseMessage(false);
            var message2 = (ApplicationShutdownCapabilityResponseMessage)message1.Copy();

            Assert.IsTrue(message1.Equals((object)message2));
            Assert.IsTrue(message2.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameObject()
        {
            var message = new ApplicationShutdownCapabilityResponseMessage(false);
            Assert.IsTrue(message.Equals((object)message));
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullMessage()
        {
            var message = new ApplicationShutdownCapabilityResponseMessage(false);
            ApplicationShutdownCapabilityResponseMessage nullReference = null;

            Assert.IsFalse(message.Equals(nullReference));
        }

        [Test]
        [Description("Checks that a message is not equal to an message of a different type.")]
        public void EqualsWithDifferentMessageType()
        {
            var message = new ApplicationShutdownCapabilityResponseMessage(false);
            var otherMessage = new ApplicationShutdownCapabilityRequestMessage();

            Assert.IsFalse(message.Equals(otherMessage));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal message of equal type.")]
        public void EqualsWithNonEqualMessages()
        {
            var message1 = new ApplicationShutdownCapabilityResponseMessage(false);
            var message2 = new ApplicationShutdownCapabilityResponseMessage(true);

            Assert.IsFalse(message1.Equals(message2));
            Assert.IsFalse(message2.Equals(message1));
        }

        [Test]
        [Description("Checks that a message is equal to an equal message of equal type.")]
        public void EqualsWithEqualMessages()
        {
            var message1 = new ApplicationShutdownCapabilityResponseMessage(false);
            var message2 = (ApplicationShutdownCapabilityResponseMessage)message1.Copy();

            Assert.IsTrue(message1.Equals(message2));
            Assert.IsTrue(message2.Equals(message1));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameMessage()
        {
            var message = new ApplicationShutdownCapabilityResponseMessage(false);
            Assert.IsTrue(message.Equals(message));
        }
    }
}
