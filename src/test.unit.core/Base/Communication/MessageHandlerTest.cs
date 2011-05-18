//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the ManualDiscoverySource class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MessageHandlerTest
    {
        [Test]
        [Description("Checks that a message response is delivered to the correct waiting task.")]
        public void ForwardResponse()
        {
            var handler = new MessageHandler();

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
        [Description("Checks that a failure response is delivered to the correct waiting task if the endpoint disconnects.")]
        public void ForwardResponseWithDisconnectingEndpoint()
        {
            var handler = new MessageHandler();

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
        [Description("Checks that a message can be filtered.")]
        public void ActOnArrival()
        {
            ICommunicationMessage storedMessage = null;
            var processAction = new Mock<IMessageProcessAction>();
            {
                processAction.Setup(p => p.Invoke(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(m => { storedMessage = m; });
            }

            var handler = new MessageHandler();
            handler.ActOnArrival(new MessageKindFilter(typeof(EndpointConnectMessage)), processAction.Object);

            var endpoint = new EndpointId("endpoint");
            var messageId = new MessageId();
            var msg = new EndpointConnectMessage(endpoint, "net.pipe://localhost/apollo_test", typeof(NamedPipeChannelType));
            handler.ProcessMessage(msg);

            Assert.AreSame(msg, storedMessage);
        }
    }
}
