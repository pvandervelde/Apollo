//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointConnectProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var sink = new Mock<IAcceptExternalEndpointInformation>();
            var channelTypes = new Type[] 
                { 
                    typeof(TcpChannelType),
                };
            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new EndpointConnectProcessAction(sink.Object, channelTypes, logger);
            Assert.AreEqual(typeof(EndpointConnectMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            EndpointId processedId = null;
            Type processedType = null;
            Uri processedUri = null;
            var sink = new Mock<IAcceptExternalEndpointInformation>();
            {
                sink.Setup(s => s.RecentlyConnectedEndpoint(It.IsAny<EndpointId>(), It.IsAny<Type>(), It.IsAny<Uri>()))
                    .Callback<EndpointId, Type, Uri>((e, t, u) => 
                        {
                            processedId = e;
                            processedType = t;
                            processedUri = u;
                        });
            }

            var channelTypes = new Type[] 
                { 
                    typeof(TcpChannelType),
                };
            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new EndpointConnectProcessAction(sink.Object, channelTypes, logger);
            
            var id = new EndpointId("id");
            var uri = "http://localhost";
            var type = typeof(TcpChannelType);
            var msg = new EndpointConnectMessage(id, uri, type);
            action.Invoke(msg);

            Assert.AreEqual(id, processedId);
            Assert.AreEqual(type, processedType);
            Assert.AreEqual(uri, processedUri.OriginalString);
        }
    }
}
