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
    [Description("Tests the MessageProcessingAssistence class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MessageProcessingAssistenceTest
    {
        [Test]
        [Description("Checks that it is not possible to provide a null pipeline.")]
        public void DefinePipelineInformationWithNullPipeline()
        {
            var assistence = new MessageProcessingAssistance();
            Assert.Throws<ArgumentNullException>(() => assistence.DefinePipelineInformation(null, new DnsName("name"), e => { }));
        }

        [Test]
        [Description("Checks that it is not possible to provide a null sender DNS name.")]
        public void DefinePipelineInformationWithNullSender()
        {
            var assistence = new MessageProcessingAssistance();
            Assert.Throws<ArgumentNullException>(() => assistence.DefinePipelineInformation(new Mock<IMessagePipeline>().Object, null, e => { }));
        }

        [Test]
        [Description("Checks that it is not possible to provide a sender DNS name that is equal to Nobody.")]
        public void DefinePipelineInformationWithNobodySender()
        {
            var assistence = new MessageProcessingAssistance();
            Assert.Throws<ArgumentException>(() => assistence.DefinePipelineInformation(new Mock<IMessagePipeline>().Object, DnsName.Nobody, e => { }));
        }

        [Test]
        [Description("Checks that it is not possible to register an action without a type.")]
        public void RegisterActionWithNullMessageType()
        {
            var assistence = new MessageProcessingAssistance();
            Assert.Throws<ArgumentNullException>(() => assistence.RegisterAction(null, msg => msg.ToString()));
        }

        [Test]
        [Description("Checks that it is not possible to register an action with an incorrect type.")]
        public void RegisterActionWithIncorrectType()
        {
            var assistence = new MessageProcessingAssistance();
            Assert.Throws<ArgumentException>(() => assistence.RegisterAction(typeof(object), msg => msg.ToString()));
        }

        [Test]
        [Description("Checks that it is not possible to register an action without an action.")]
        public void RegisterActionWithNullAction()
        {
            var assistence = new MessageProcessingAssistance();
            Assert.Throws<ArgumentException>(() => assistence.RegisterAction(typeof(ShutdownResponseMessage), null));
        }

        [Test]
        [Description("Checks that it is not possible to send a message without registering a pipeline.")]
        public void SendMessageWithoutRegisteringPipeline()
        {
            var assistence = new MessageProcessingAssistance();
            Assert.Throws<MissingPipelineException>(() => assistence.SendMessage(new DnsName("name"), new ShutdownResponseMessage(true), MessageId.None));
        }

        [Test]
        [Description("Checks that it is not possible to send a message without a recipient.")]
        public void SendMessageWithNullRecipient()
        {
            var assistence = new MessageProcessingAssistance();
            assistence.DefinePipelineInformation(new Mock<IMessagePipeline>().Object, new DnsName("name"), e => { });
            Assert.Throws<ArgumentNullException>(() => assistence.SendMessage(null, new ShutdownResponseMessage(true), MessageId.None));
        }

        [Test]
        [Description("Checks that it is not possible to send a message without a recipient.")]
        public void SendMessageWithRecipientEqualToSender()
        {
            var assistence = new MessageProcessingAssistance();
            assistence.DefinePipelineInformation(new Mock<IMessagePipeline>().Object, new DnsName("name"), e => { });
            Assert.Throws<ArgumentException>(() => assistence.SendMessage(new DnsName("name"), new ShutdownResponseMessage(true), MessageId.None));
        }

        [Test]
        [Description("Checks that it is not possible to send a message with a Nobody recipient.")]
        public void SendMessageWithNobodyRecipient()
        {
            var assistence = new MessageProcessingAssistance();
            assistence.DefinePipelineInformation(new Mock<IMessagePipeline>().Object, new DnsName("name"), e => { });
            Assert.Throws<ArgumentException>(() => assistence.SendMessage(DnsName.Nobody, new ShutdownResponseMessage(true), MessageId.None));
        }

        [Test]
        [Description("Checks that it is not possible to send a message without a message body.")]
        public void SendMessageWithNullBody()
        {
            var assistence = new MessageProcessingAssistance();
            assistence.DefinePipelineInformation(new Mock<IMessagePipeline>().Object, new DnsName("name"), e => { });
            Assert.Throws<ArgumentException>(() => assistence.SendMessage(new DnsName("otherName"), null, MessageId.None));
        }

        [Test]
        [Description("Checks that it is not possible to send a message with a null original ID.")]
        public void SendMessageWithNullOriginalId()
        {
            var assistence = new MessageProcessingAssistance();
            assistence.DefinePipelineInformation(new Mock<IMessagePipeline>().Object, new DnsName("name"), e => { });
            Assert.Throws<ArgumentException>(() => assistence.SendMessage(new DnsName("otherName"), new ShutdownResponseMessage(true), null));
        }

        [Test]
        [Description("Checks that message replies get processed correctly if the reply comes in before the message is stored.")]
        public void SendMessageWithResponseWithAnswerBeforeContinue()
        {
            var assistence = new MessageProcessingAssistance();

            var senderDns = new DnsName("sender");
            var recipientDns = new DnsName("recipient");

            var id = MessageId.Next();
            var mockPipeline = new Mock<IMessagePipeline>();
            {
                mockPipeline.Setup(pipeline => pipeline.Send(It.IsAny<DnsName>(), It.IsAny<DnsName>(), It.IsAny<MessageBody>(), It.IsAny<MessageId>()))
                    .Callback<DnsName, DnsName, MessageBody, MessageId>((sender, recipient, body, returnId) =>
                        {
                            var header = new MessageHeader(MessageId.Next(), recipient, sender, id);
                            assistence.ReceiveMessage(new KernelMessage(header, body));
                        })
                    .Returns(() => id);
            }

            assistence.DefinePipelineInformation(mockPipeline.Object, senderDns, e => { });

            var messageBody = new ShutdownResponseMessage(true);
            var future = assistence.SendMessageWithResponse(recipientDns, messageBody, MessageId.None);

            var futureBody = future.Result();
            Assert.AreSame(messageBody, futureBody);
        }

        [Test]
        [Description("Checks that message replies get processed correctly if the reply comes in after the message is stored.")]
        public void SendMessageWithResponseWithAnswerAfterContinue()
        {
            KernelMessage messageToSend = null;

            var senderDns = new DnsName("sender");
            var recipientDns = new DnsName("recipient");

            var id = MessageId.Next();
            var mockPipeline = new Mock<IMessagePipeline>();
            {
                mockPipeline.Setup(pipeline => pipeline.Send(It.IsAny<DnsName>(), It.IsAny<DnsName>(), It.IsAny<MessageBody>(), It.IsAny<MessageId>()))
                    .Callback<DnsName, DnsName, MessageBody, MessageId>((sender, recipient, body, returnId) =>
                    {
                        var header = new MessageHeader(MessageId.Next(), recipient, sender, id);
                        messageToSend = new KernelMessage(header, body);
                    })
                    .Returns(() => id);
            }

            var assistence = new MessageProcessingAssistance();
            assistence.DefinePipelineInformation(mockPipeline.Object, senderDns, e => { });

            var messageBody = new ShutdownResponseMessage(true);
            var future = assistence.SendMessageWithResponse(recipientDns, messageBody, MessageId.None);
            assistence.ReceiveMessage(messageToSend);

            var futureBody = future.Result();
            Assert.AreSame(messageBody, futureBody);
        }

        [Test]
        [Description("Checks that message get processed correctly if the reply comes in before the message is stored.")]
        public void ReceiveMessageAsNonAnswer()
        {
            KernelMessage receivedMessage = null;

            var senderDns = new DnsName("sender");
            var mockPipeline = new Mock<IMessagePipeline>();

            var assistence = new MessageProcessingAssistance();
            assistence.RegisterAction(typeof(ShutdownResponseMessage), msg => { receivedMessage = msg; });
            assistence.DefinePipelineInformation(mockPipeline.Object, senderDns, e => { });

            var messageBody = new ShutdownResponseMessage(true);
            var kernelMessage = new KernelMessage(
                new MessageHeader(
                    MessageId.Next(), 
                    new DnsName("somebody"), 
                    senderDns), 
                messageBody);
            assistence.ReceiveMessage(kernelMessage);

            Assert.AreSame(kernelMessage, receivedMessage);
        }
    }
}
