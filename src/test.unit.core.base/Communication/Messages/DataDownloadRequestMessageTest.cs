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
    public sealed class DataDownloadRequestMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var token = new UploadToken();
            var streamInfo = new NamedPipeStreamTransferInformation 
                {
                    Name = "net.pipe://localhost"
                };
            var msg = new DataDownloadRequestMessage(id, token, streamInfo);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(token, msg.Token);
            Assert.AreSame(streamInfo, msg.TransferInformation);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var token = new UploadToken();
            var streamInfo = new NamedPipeStreamTransferInformation
            {
                Name = "net.pipe://localhost"
            };
            var msg = new DataDownloadRequestMessage(id, token, streamInfo);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(msg.Id, otherMsg.Id);
            Assert.AreEqual(MessageId.None, otherMsg.InResponseTo);
            Assert.AreEqual(token, otherMsg.Token);
            Assert.AreEqual(
                (NamedPipeStreamTransferInformation)streamInfo,
                (NamedPipeStreamTransferInformation)otherMsg.TransferInformation, 
                (NamedPipeStreamTransferInformation x, NamedPipeStreamTransferInformation y) => 
                    x.ChannelType.Equals(y.ChannelType)
                    && (x.StartPosition == y.StartPosition) 
                    && string.Equals(x.Name, y.Name, System.StringComparison.Ordinal));
        }
    }
}
