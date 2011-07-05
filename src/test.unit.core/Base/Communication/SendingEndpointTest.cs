//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class SendingEndpointTest
    {
        [Test]
        public void SendToUnknownReceiver()
        {
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);
            var proxy = new Mock<IChannelProxy>();
            {
                proxy.Setup(p => p.Send(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(input => Assert.AreSame(msg, input));
            }

            Func<EndpointId, IChannelProxy> builder = id => proxy.Object;
            var sender = new SendingEndpoint(builder);

            sender.Send(endpointId, msg);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
        }

        [Test]
        public void SendToKnownReceiver()
        {
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);
            var proxy = new Mock<IChannelProxy>();
            {
                proxy.Setup(p => p.Send(It.IsAny<ICommunicationMessage>()))
                    .Callback<ICommunicationMessage>(input => Assert.AreSame(msg, input))
                    .Verifiable();
            }

            Func<EndpointId, IChannelProxy> builder = id => proxy.Object;
            var sender = new SendingEndpoint(builder);

            sender.Send(endpointId, msg);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
            proxy.Verify(p => p.Send(It.IsAny<ICommunicationMessage>()), Times.Exactly(1));

            sender.Send(endpointId, msg);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());
            proxy.Verify(p => p.Send(It.IsAny<ICommunicationMessage>()), Times.Exactly(2));
        }

        [Test]
        public void CloseChannelTo()
        {
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);
            var proxy = new Mock<IChannelProxy>();
            var disposable = proxy.As<IDisposable>();

            Func<EndpointId, IChannelProxy> builder = id => proxy.Object;
            var sender = new SendingEndpoint(builder);

            sender.Send(endpointId, msg);
            Assert.AreEqual(1, sender.KnownEndpoints().Count());

            sender.CloseChannelTo(endpointId);
            Assert.AreEqual(0, sender.KnownEndpoints().Count());
        }
    }
}
