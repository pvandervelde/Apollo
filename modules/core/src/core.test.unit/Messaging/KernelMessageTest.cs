//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Messaging
{
    [TestFixture]
    [Description("Tests the KernelMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class KernelMessageTest
    {
        [Test]
        [Description("Checks that a message cannot be created without a header.")]
        public void CreateWithoutHeader()
        {
            Assert.Throws<ArgumentNullException>(() => new KernelMessage(null, new Mock<MessageBody>(false).Object));
        }

        [Test]
        [Description("Checks that a message cannot be created without a body.")]
        public void CreateWithoutBody()
        {
            Assert.Throws<ArgumentNullException>(() => new KernelMessage(new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")), null));
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                new Mock<MessageBody>(false).Object);

            Assert.IsFalse(message.Equals((object)null));
        }

        [Test]
        [Description("Checks that a message is not equal to an object of a different type.")]
        public void EqualsWithIncorrectType()
        {
            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                new Mock<MessageBody>(false).Object);

            Assert.IsFalse(message.Equals(new object()));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal message.")]
        public void EqualsWithNonEqualObject()
        {
            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                new Mock<MessageBody>(false).Object);
            var otherMessage = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("otherName"), new DnsName("name")),
                new Mock<MessageBody>(false).Object);

            Assert.IsFalse(message.Equals((object)otherMessage));
        }

        [Test]
        [Description("Checks that a message is equal to an equal message.")]
        public void EqualsWithEqualObject()
        {
            var bodyMock = new Mock<MessageBody>(false);
            {
                bodyMock.Setup(body => body.Copy())
                    .Returns(bodyMock.Object);
            }

            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                bodyMock.Object);

            var otherMessage = new KernelMessage(message);

            Assert.IsTrue(message.Equals((object)otherMessage));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameObject()
        {
            var bodyMock = new Mock<MessageBody>(false);
            {
                bodyMock.Setup(body => body.Copy())
                    .Returns(bodyMock.Object);
            }

            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                bodyMock.Object);

            Assert.IsTrue(message.Equals((object)message));
        }

        [Test]
        [Description("Checks that a message is not equal to a null reference.")]
        public void EqualsWithNullMessage()
        {
            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                new Mock<MessageBody>(false).Object);

            Assert.IsFalse(message.Equals(null));
        }

        [Test]
        [Description("Checks that a message is not equal to a non-equal message.")]
        public void EqualsWithNonEqualMessage()
        {
            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                new Mock<MessageBody>(false).Object);
            var otherMessage = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("otherName"), new DnsName("name")),
                new Mock<MessageBody>(false).Object);

            Assert.IsFalse(message.Equals(otherMessage));
        }

        [Test]
        [Description("Checks that a message is equal to an equal message.")]
        public void EqualsWithEqualMessage()
        {
            var bodyMock = new Mock<MessageBody>(false);
            {
                bodyMock.Setup(body => body.Copy())
                    .Returns(bodyMock.Object);
            }

            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                bodyMock.Object);

            var otherMessage = new KernelMessage(message);

            Assert.IsTrue(message.Equals(otherMessage));
        }

        [Test]
        [Description("Checks that a message is equal to itself.")]
        public void EqualsWithSameMessage()
        {
            var bodyMock = new Mock<MessageBody>(false);
            {
                bodyMock.Setup(body => body.Copy())
                    .Returns(bodyMock.Object);
            }

            var message = new KernelMessage(
                new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")),
                bodyMock.Object);

            Assert.IsTrue(message.Equals(message));
        }
    }
}
