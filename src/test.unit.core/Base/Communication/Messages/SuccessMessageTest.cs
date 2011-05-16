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
    [Description("Tests the SuccessMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class SuccessMessageTest
    {
        [Test]
        [Description("Checks that the message serialises and deserialises correctly.")]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var response = new MessageId();
            var msg = new SuccessMessage(id, response);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(response, otherMsg.InResponseTo);
        }
    }
}
