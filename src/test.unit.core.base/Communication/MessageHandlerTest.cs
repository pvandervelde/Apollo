//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Communication.Messages.Processors;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MessageHandlerTest
    {
        [Test]
        public void ForwardResponse()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var handler = new MessageHandler(logger);

            var endpoint = new EndpointId("endpoint");
            var messageId = new MessageId();
            var task = handler.ForwardResponse(endpoint, messageId);
            Assert.IsFalse(task.IsCompleted);

            var msg = new SuccessMessage(endpoint, messageId);
            handler.ProcessMessage(msg);

            task.Wait();
            Assert.IsTrue(task.IsCompleted);
            Assert.AreSame(msg, task.Result);
        }

        [Test]
        public void ForwardResponseWithDisconnectingEndpoint()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var handler = new MessageHandler(logger);

            var endpoint = new EndpointId("endpoint");
            var messageId = new MessageId();
            var task = handler.ForwardResponse(endpoint, messageId);
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);

            var msg = new EndpointDisconnectMessage(endpoint);
            handler.ProcessMessage(msg);

            Assert.Throws<AggregateException>(() => task.Wait());
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCanceled);
        }

        [Test]
        public void ActOnArrival()
        {
            ICommunicationMessage storedMessage = null;
            var processAction = new Mock<IMessageProcessAction>();
            {
                processAction.Setup(p => p.Invoke(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(m => { storedMessage = m; });
            }

            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var handler = new MessageHandler(logger);
            handler.ActOnArrival(new MessageKindFilter(typeof(EndpointConnectMessage)), processAction.Object);

            var endpoint = new EndpointId("endpoint");
            var messageId = new MessageId();
            var msg = new EndpointConnectMessage(endpoint, "net.pipe://localhost/apollo_test", typeof(NamedPipeChannelType));
            handler.ProcessMessage(msg);

            Assert.AreSame(msg, storedMessage);
        }

        [Test]
        public void ActOnArrivalWithLastChanceHandler()
        {
            var localEndpoint = new EndpointId("id");

            EndpointId storedEndpoint = null;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
            {
                storedEndpoint = e;
                storedMsg = m;
            };

            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var processAction = new UnknownMessageTypeProcessAction(localEndpoint, sendAction, logger);
            var handler = new MessageHandler(logger);
            handler.ActOnArrival(new MessageKindFilter(processAction.MessageTypeToProcess), processAction);

            var endpoint = new EndpointId("endpoint");
            var messageId = new MessageId();
            var msg = new EndpointConnectMessage(endpoint, "net.pipe://localhost/apollo_test", typeof(NamedPipeChannelType));
            handler.ProcessMessage(msg);

            Assert.IsInstanceOfType<UnknownMessageTypeMessage>(storedMsg);
        }
    }
}
