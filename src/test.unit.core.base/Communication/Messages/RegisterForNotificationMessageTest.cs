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
    public sealed class RegisterForNotificationMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var notification = new SerializedEvent(new SerializedType("a"), "b");
            var msg = new RegisterForNotificationMessage(id, notification);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(notification, msg.Notification);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var notification = new SerializedEvent(new SerializedType("a"), "b");
            var msg = new RegisterForNotificationMessage(id, notification);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(notification, otherMsg.Notification);
        }
    }
}
