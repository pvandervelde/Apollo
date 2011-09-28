//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Base.Communication.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointConnectMessageTest
    {
        [Test]
        [Description("Checks that a message can be created.")]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var address = "bla";
            var channelType = typeof(TcpChannelType);
            var msg = new EndpointConnectMessage(id, address, channelType);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(address, msg.Address);
            Assert.AreEqual(channelType.FullName, msg.ChannelType);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var address = "bla";
            var channelType = typeof(TcpChannelType);
            var msg = new EndpointConnectMessage(id, address, channelType);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(msg.Id, otherMsg.Id);
            Assert.AreEqual(MessageId.None, otherMsg.InResponseTo);
            Assert.AreEqual(address, otherMsg.Address);
            Assert.AreEqual(channelType.FullName, otherMsg.ChannelType);
        }
    }
}
