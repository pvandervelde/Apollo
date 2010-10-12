//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Messaging
{
    [TestFixture]
    [Description("Tests the MessageHeader class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MessageHeaderTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<MessageHeader>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Join(
                    DataGenerators.Sequential.Numbers(0, 1000),
                    new List<DnsName>
                        {
                            new DnsName("a"),
                        },
                    new List<DnsName> 
                        { 
                            new DnsName("aa"),
                        })
                .Select(o => new MessageHeader(MessageId.Next(), o.Second, o.Third)),
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<MessageHeader>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<MessageHeader> 
                { 
                    new MessageHeader(MessageId.Next(), new DnsName("a"), new DnsName("b")),
                    new MessageHeader(MessageId.Next(), new DnsName("a"), new DnsName("c")),
                    new MessageHeader(MessageId.Next(), new DnsName("d"), new DnsName("b")),
                },
        };

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
        [Description("Checks that the message serialises and deserialises correctly.")]
        public void RoundTripSerialise()
        {
            var msg = new MessageHeader(MessageId.Next(), new DnsName("a"), new DnsName("b"));
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            AssertEx.That(
               () => msg.Id == otherMsg.Id
                  && msg.Recipient == otherMsg.Recipient
                  && msg.Sender == otherMsg.Sender);
        }
    }
}
