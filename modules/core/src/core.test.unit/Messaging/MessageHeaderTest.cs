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
    [Description("Tests the MessageHeader class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MessageHeaderTest
    {
        [Test]
        [Description("Checks that a header can be created with a sender, an ID and a recipient.")]
        public void CreateWithSenderMessageIdAndRecipient()
        {
            var id = MessageId.Next();
            var sender = new DnsName("name");
            var recipient = new DnsName("other");

            var header = new MessageHeader(id, sender, recipient);
            Assert.AreSame(id, header.Id);
            Assert.AreSame(sender, header.Sender);
            Assert.AreEqual(recipient, header.Recipient);
            Assert.AreEqual(MessageId.None, header.InReplyTo);
        }

        [Test]
        [Description("Checks that a header cannot be created with a sender and recipient the same.")]
        public void CreateWithSenderAsRecipient()
        {
            var id = MessageId.Next();
            var sender = new DnsName("name");

            Assert.Throws<ArgumentException>(() => new MessageHeader(id, sender, sender));
        }

        [Test]
        [Description("Checks that a header can be created with a sender, an ID, a recipient and a reply ID.")]
        public void CreateWithSenderMessageIdRecipientAndReplyId()
        { 
            var id = MessageId.Next();
            var sender = new DnsName("name");
            var recipient = new DnsName("other");
            var responseId = MessageId.Next();

            var header = new MessageHeader(id, sender, recipient, responseId);
            Assert.AreSame(id, header.Id);
            Assert.AreSame(sender, header.Sender);
            Assert.AreEqual(recipient, header.Recipient);
            Assert.AreEqual(responseId, header.InReplyTo);
        }

        [Test]
        [Description("Checks that a header cannot be created with a null ID.")]
        public void CreateWithNullId()
        {
            Assert.Throws<ArgumentNullException>(() => new MessageHeader(null, new DnsName("name"), new DnsName("otherName")));
        }

        [Test]
        [Description("Checks that a header cannot be created with a None ID.")]
        public void CreateWithNoneId()
        {
            Assert.Throws<ArgumentException>(() => new MessageHeader(MessageId.None, new DnsName("name"), new DnsName("otherName")));
        }

        [Test]
        [Description("Checks that a header cannot be created with a null sender.")]
        public void CreateWithNullSender()
        {
            Assert.Throws<ArgumentNullException>(() => new MessageHeader(MessageId.Next(), null, new DnsName("otherName")));
        }

        [Test]
        [Description("Checks that a header cannot be created with a sender equal to the Nobody ID.")]
        public void CreateWithNobodySender()
        {
            Assert.Throws<ArgumentException>(() => new MessageHeader(MessageId.Next(), DnsName.Nobody, new DnsName("otherName")));
        }

        [Test]
        [Description("Checks that a header cannot be created with a null recipient.")]
        public void CreateWithNullRecipient()
        {
            Assert.Throws<ArgumentNullException>(() => new MessageHeader(MessageId.Next(), new DnsName("name"), null));
        }

        [Test]
        [Description("Checks that a header cannot be created with a recipient equal to the Nobody ID.")]
        public void CreateWithNobodyRecipient()
        {
            Assert.Throws<ArgumentException>(() => new MessageHeader(MessageId.Next(), new DnsName("name"), DnsName.Nobody));
        }

        [Test]
        [Description("Checks that a header cannot be created with a null response ID.")]
        public void CreateWithNullResponseId()
        {
            Assert.Throws<ArgumentNullException>(() => new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("other"), null));
        }

        [Test]
        [Description("Checks that a MessageHeader is not equal to a null object.")]
        public void EqualsWithNullObject()
        {
            var id = MessageId.Next();
            var sender = new DnsName("name");
            var recipient = new DnsName("otherName");

            var header = new MessageHeader(id, sender, recipient);
            Assert.IsFalse(header.Equals((object)null));
        }

        [Test]
        [Description("Checks that a MessageHeader is not equal to an object of a different type.")]
        public void EqualsWithNonEqualObjectType()
        {
            var id = MessageId.Next();
            var sender = new DnsName("name");
            var recipient = new DnsName("otherName");

            var header = new MessageHeader(id, sender, recipient);
            Assert.IsFalse(header.Equals(new object()));
        }

        [Test]
        [Description("Checks that a MessageHeader is not equal to a different MessageHeader.")]
        public void EqualsWithNonEqualObject()
        {
            var id = MessageId.Next();
            var sender = new DnsName("name");
            var recipient = new DnsName("otherName");

            var header = new MessageHeader(id, sender, recipient);
            Assert.IsFalse(header.Equals(new MessageHeader(MessageId.Next(), sender, recipient)));
        }

        [Test]
        [Description("Checks that a MessageHeader is equal to an identical MessageHeader.")]
        public void EqualsWithEqualObject()
        {
            var id = MessageId.Next();
            var sender = new DnsName("name");
            var recipient = new DnsName("otherName");

            var header = new MessageHeader(id, sender, recipient);
            Assert.IsTrue(header.Equals(new MessageHeader(header)));
        }
    }
}
