//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using MbUnit.Framework;

namespace Apollo.Base.Communication.Messages
{
    [TestFixture]
    [Description("Tests the CommandInvokedResponseMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommandInvokedResponseMessageTest
    {
        [Test]
        [Description("Checks that a message can be created.")]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var response = new MessageId();
            var result = 10;
            var msg = new CommandInvokedResponseMessage(id, response, result);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(response, msg.InResponseTo);
            Assert.AreEqual(result, (int)msg.Result);
        }

        [Test]
        [Description("Checks that the message serialises and deserialises correctly.")]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var response = new MessageId();
            var result = 10;
            var msg = new CommandInvokedResponseMessage(id, response, result);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(response, otherMsg.InResponseTo);
            Assert.AreEqual(msg.Id, otherMsg.Id);
            Assert.AreEqual(result, (int)otherMsg.Result);
        }
    }
}
