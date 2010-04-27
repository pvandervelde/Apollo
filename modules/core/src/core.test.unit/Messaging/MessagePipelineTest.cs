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
    [Description("Tests the MessagePipeline class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MessagePipelineTest
    {
        [Test]
        [Description("Checks that a listener cannot be registered with a null object.")]
        public void RegisterAsListenerWithNullObject()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.Throws<ArgumentNullException>(() => pipeline.RegisterAsListener(null));
        }

        [Test]
        [Description("Checks that a listener cannot be registered with a name that is already registered.")]
        public void RegisterAsListenerWithDuplicateName()
        {
            var listenerMock = new Mock<IProcessMessages>();
            {
                listenerMock.Setup(listener => listener.Name)
                    .Returns(new DnsName("name"));
            }

            var pipeline = new MessagePipeline(new DnsNameConstants());
            pipeline.RegisterAsListener(listenerMock.Object);

            Assert.Throws<DuplicateDnsNameException>(() => pipeline.RegisterAsListener(listenerMock.Object));
        }

        [Test]
        [Description("Checks that a sender cannot be registered with a null object.")]
        public void RegisterAsSenderWithNullObject()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.Throws<ArgumentNullException>(() => pipeline.RegisterAsSender(null));
        }

        [Test]
        [Description("Checks that a sender cannot be registered with a name that is already registered.")]
        public void RegisterAsSenderWithDuplicateName()
        {
            var senderMock = new Mock<ISendMessages>();
            {
                senderMock.Setup(listener => listener.Name)
                    .Returns(new DnsName("name"));
            }

            var pipeline = new MessagePipeline(new DnsNameConstants());
            pipeline.RegisterAsSender(senderMock.Object);

            Assert.Throws<DuplicateDnsNameException>(() => pipeline.RegisterAsSender(senderMock.Object));
        }

        [Test]
        [Description("Checks that an object cannot be registered with a null object.")]
        public void RegisterWithNullObject()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.Throws<ArgumentNullException>(() => pipeline.Register(null));
        }

        [Test]
        [Description("Checks that a listener cannot be unregistered with a null object.")]
        public void UnregisterAsListenerWithNullObject()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.Throws<ArgumentNullException>(() => pipeline.UnregisterAsListener(null));
        }

        [Test]
        [Description("Checks that a listener cannot be unregistered with a name that is not registered.")]
        public void UnregisterAsListenerWithUnknownName()
        {
            var listenerMock = new Mock<IProcessMessages>();
            {
                listenerMock.Setup(listener => listener.Name)
                    .Returns(new DnsName("name"));
            }

            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.Throws<UnknownDnsNameException>(() => pipeline.UnregisterAsListener(listenerMock.Object));
        }

        [Test]
        [Description("Checks that a sender cannot be unregistered with a null object.")]
        public void UnregisterAsSenderWithNullObject()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.Throws<ArgumentNullException>(() => pipeline.UnregisterAsSender(null));
        }

        [Test]
        [Description("Checks that a sender cannot be unregistered with a name that is not registered.")]
        public void UnregisterAsSenderWithUnknownName()
        {
            var senderMock = new Mock<ISendMessages>();
            {
                senderMock.Setup(listener => listener.Name)
                    .Returns(new DnsName("name"));
            }

            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.Throws<UnknownDnsNameException>(() => pipeline.UnregisterAsSender(senderMock.Object));
        }

        [Test]
        [Description("Checks that an object cannot be unregistered with a null object.")]
        public void UnregisterWithNullObject()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.Throws<ArgumentNullException>(() => pipeline.Unregister(null));
        }

        [Test]
        [Description("Checks that a null name is never registered.")]
        public void IsRegisteredWithNullName()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.IsFalse(pipeline.IsRegistered((DnsName)null));
        }

        [Test]
        [Description("Checks that a registered listener evaluates as registered by name.")]
        public void IsRegisteredWithListenerName()
        {
            var listenerMock = new Mock<IProcessMessages>();
            {
                listenerMock.Setup(listener => listener.Name)
                    .Returns(new DnsName("name"));
            }

            var pipeline = new MessagePipeline(new DnsNameConstants());
            pipeline.RegisterAsListener(listenerMock.Object);
            Assert.IsTrue(pipeline.IsRegistered(listenerMock.Object.Name));
        }

        [Test]
        [Description("Checks that a registered sender evaluates as registered by name.")]
        public void IsRegisteredWithSenderName()
        {
            var senderMock = new Mock<ISendMessages>();
            {
                senderMock.Setup(sender => sender.Name)
                    .Returns(new DnsName("name"));
            }

            var pipeline = new MessagePipeline(new DnsNameConstants());
            pipeline.RegisterAsSender(senderMock.Object);
            Assert.IsTrue(pipeline.IsRegistered(senderMock.Object.Name));
        }

        [Test]
        [Description("Checks that a null listener is never registered.")]
        public void IsRegisteredWithNullListener()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.IsFalse(pipeline.IsRegistered((IProcessMessages)null));
        }

        [Test]
        [Description("Checks that a registered listener evaluates as registered by object.")]
        public void IsRegisteredWithListener()
        {
            var listenerMock = new Mock<IProcessMessages>();
            {
                listenerMock.Setup(listener => listener.Name)
                    .Returns(new DnsName("name"));
            }

            var pipeline = new MessagePipeline(new DnsNameConstants());
            pipeline.RegisterAsListener(listenerMock.Object);
            Assert.IsTrue(pipeline.IsRegistered(listenerMock.Object));
        }

        [Test]
        [Description("Checks that a null sender is never registered.")]
        public void IsRegisteredWithNullSender()
        {
            var pipeline = new MessagePipeline(new DnsNameConstants());
            Assert.IsFalse(pipeline.IsRegistered((ISendMessages)null));
        }

        [Test]
        [Description("Checks that a registered sender evaluates as registered by object.")]
        public void IsRegisteredWithSender()
        {
            var senderMock = new Mock<ISendMessages>();
            {
                senderMock.Setup(sender => sender.Name)
                    .Returns(new DnsName("name"));
            }

            var pipeline = new MessagePipeline(new DnsNameConstants());
            pipeline.RegisterAsSender(senderMock.Object);
            Assert.IsTrue(pipeline.IsRegistered(senderMock.Object));
        }

        [Test]
        [Description("Checks that a message cannot be send with a null sender object.")]
        public void SendWithNullSender()
        {
            var listener = new DnsName("listener");
            var pipeline = new MessagePipeline(new DnsNameConstants());

            var bodyMock = new Mock<MessageBody>(false);
            Assert.Throws<ArgumentNullException>(() => pipeline.Send(null, listener, bodyMock.Object));
        }

        [Test]
        [Description("Checks that a message cannot be send with the Nobody sender.")]
        public void SendWithNobodySender()
        {
            var listener = new DnsName("listener");
            var pipeline = new MessagePipeline(new DnsNameConstants());

            var bodyMock = new Mock<MessageBody>();
            Assert.Throws<ArgumentException>(() => pipeline.Send(DnsName.Nobody, listener, bodyMock.Object));
        }

        [Test]
        [Description("Checks that a message cannot be send with the Nobody recipient.")]
        public void SendWithNullRecipient()
        {
            var sender = new DnsName("sender");
            var pipeline = new MessagePipeline(new DnsNameConstants());

            var bodyMock = new Mock<MessageBody>(false);
            Assert.Throws<ArgumentNullException>(() => pipeline.Send(sender, null, bodyMock.Object));
        }

        [Test]
        [Description("Checks that a message cannot be send with the Nobody recipient.")]
        public void SendWithNobodyRecipient()
        {
            var sender = new DnsName("sender");
            var pipeline = new MessagePipeline(new DnsNameConstants());

            var bodyMock = new Mock<MessageBody>();
            Assert.Throws<ArgumentException>(() => pipeline.Send(sender, DnsName.Nobody, bodyMock.Object));
        }

        [Test]
        [Description("Checks that a message cannot be send when sender and recipient are the same.")]
        public void SendWithSenderAndRecipientEqual()
        {
            var sender = new DnsName("sender");
            var pipeline = new MessagePipeline(new DnsNameConstants());

            var bodyMock = new Mock<MessageBody>();
            Assert.Throws<ArgumentException>(() => pipeline.Send(sender, sender, bodyMock.Object));
        }

        [Test]
        [Description("Checks that a message cannot be send without a body.")]
        public void SendWithNullBody()
        {
            var sender = new DnsName("sender");
            var listener = new DnsName("listener");
            var pipeline = new MessagePipeline(new DnsNameConstants());

            Assert.Throws<ArgumentException>(() => pipeline.Send(sender, listener, null));
        }

        [Test]
        [Description("Checks that a message cannot be send with a null replyTo ID.")]
        public void SendWithNullReplyToId()
        {
            var sender = new DnsName("sender");
            var listener = new DnsName("listener");
            var pipeline = new MessagePipeline(new DnsNameConstants());

            var bodyMock = new Mock<MessageBody>();
            Assert.Throws<ArgumentException>(() => pipeline.Send(sender, listener, bodyMock.Object, null));
        }

        [Test]
        [Description("Checks that a message can be send correctly when no errors occur.")]
        public void SendWithoutError()
        {
            var senderMock = new Mock<ISendMessages>();
            {
                senderMock.Setup(senderObj => senderObj.Name)
                    .Returns(new DnsName("sender"));
            }

            KernelMessage storedMessage = null;
            var listenerMock = new Mock<IProcessMessages>();
            {
                listenerMock.Setup(listenerObj => listenerObj.Name)
                    .Returns(new DnsName("listener"));

                listenerMock.Setup(listenerObj => listenerObj.ProcessMessage(It.IsAny<KernelMessage>()))
                    .Callback<KernelMessage>(message => { storedMessage = message; });
            }

            var listener = listenerMock.Object;
            var sender = senderMock.Object;
            var pipeline = new MessagePipeline(new DnsNameConstants());
            
            pipeline.RegisterAsSender(sender);
            pipeline.RegisterAsListener(listener);

            var bodyMock = new Mock<MessageBody>(false);
            pipeline.Send(sender.Name, listener.Name, bodyMock.Object);

            Assert.IsNotNull(storedMessage);
            Assert.AreSame(sender.Name, storedMessage.Header.Sender);
            Assert.AreSame(listener.Name, storedMessage.Header.Recipient);
        }

        [Test]
        [Description("Checks that the message sending handles errors correctly.")]
        [Ignore("Current implementation is not sufficient...")]
        public void SendWithError()
        {
            MessageId storedId = null;
            DnsName storedName = null;
            MessageBody storedBody = null;

            var senderMock = new Mock<ISendMessages>();
            {
                senderMock.Setup(senderObj => senderObj.Name)
                    .Returns(new DnsName("sender"));
            }

            var listenerMock = new Mock<IProcessMessages>();
            {
                listenerMock.Setup(listenerObj => listenerObj.Name)
                    .Returns(new DnsName("listener"));

                listenerMock.Setup(listenerObj => listenerObj.ProcessMessage(It.IsAny<KernelMessage>()))
                    .Throws(new Exception());
            }

            var listener = listenerMock.Object;
            var sender = senderMock.Object;
            var pipeline = new MessagePipeline(new DnsNameConstants());

            pipeline.RegisterAsSender(sender);
            pipeline.RegisterAsListener(listener);

            var bodyMock = new Mock<MessageBody>();
            var returnedId = pipeline.Send(sender.Name, listener.Name, bodyMock.Object);

            Assert.AreSame(returnedId, storedId);
            Assert.AreSame(listener.Name, storedName);
            Assert.AreSame(bodyMock.Object, storedBody);
        }
    }
}
