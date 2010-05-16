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
    [Description("Tests the ServiceShutdownCapabilityResponseMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ServiceShutdownCapabilityResponseMessageTest
    {
        [Test]
        [Description("Checks that the shutdown state is stored properly.")]
        public void CanShutdown()
        {
            var message = new ServiceShutdownCapabilityResponseMessage(true);
            Assert.IsTrue(message.CanShutdown);
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var message = new ServiceShutdownCapabilityResponseMessage(false);
            object nullReference = null;

            Assert.IsFalse(message.Equals(nullReference));
        }

        [Test]
        [Description("Checks that a message is not equal to an object of a different type.")]
        public void EqualsWithDifferentType()
        {
            var message = new ServiceShutdownCapabilityResponseMessage(false);
            object obj = new object();

            Assert.IsFalse(message.Equals(obj));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal object of equal type.")]
        public void EqualsWithNonEqualObjects()
        {
            var message1 = new ServiceShutdownCapabilityResponseMessage(false);
            var message2 = new ServiceShutdownCapabilityResponseMessage(true);

            Assert.IsFalse(message1.Equals((object)message2));
            Assert.IsFalse(message2.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is equal to an equal object of equal type.")]
        public void EqualsWithEqualObjects()
        {
            var message1 = new ServiceShutdownCapabilityResponseMessage(false);
            var message2 = (ServiceShutdownCapabilityResponseMessage)message1.Copy();

            Assert.IsTrue(message1.Equals((object)message2));
            Assert.IsTrue(message2.Equals((object)message1));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameObject()
        {
            var message = new ServiceShutdownCapabilityResponseMessage(false);
            Assert.IsTrue(message.Equals((object)message));
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullMessage()
        {
            var message = new ServiceShutdownCapabilityResponseMessage(false);
            ServiceShutdownCapabilityResponseMessage nullReference = null;

            Assert.IsFalse(message.Equals(nullReference));
        }

        [Test]
        [Description("Checks that a message is not equal to an message of a different type.")]
        public void EqualsWithDifferentMessageType()
        {
            var message = new ServiceShutdownCapabilityResponseMessage(false);
            var otherMessage = new ServiceShutdownCapabilityRequestMessage();

            Assert.IsFalse(message.Equals(otherMessage));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal message of equal type.")]
        public void EqualsWithNonEqualMessage()
        {
            var message1 = new ServiceShutdownCapabilityResponseMessage(false);
            var message2 = new ServiceShutdownCapabilityResponseMessage(true);

            Assert.IsFalse(message1.Equals(message2));
            Assert.IsFalse(message2.Equals(message1));
        }

        [Test]
        [Description("Checks that a message is equal to an equal message of equal type.")]
        public void EqualsWithEqualMessages()
        {
            var message1 = new ServiceShutdownCapabilityResponseMessage(false);
            var message2 = (ServiceShutdownCapabilityResponseMessage)message1.Copy();

            Assert.IsTrue(message1.Equals(message2));
            Assert.IsTrue(message2.Equals(message1));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameMessage()
        {
            var message = new ServiceShutdownCapabilityResponseMessage(false);
            Assert.IsTrue(message.Equals(message));
        }
    }
}
