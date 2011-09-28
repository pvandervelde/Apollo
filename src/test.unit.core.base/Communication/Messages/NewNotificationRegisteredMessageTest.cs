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
    public sealed class NewNotificationRegisteredMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var notificationType = typeof(string);
            var msg = new NewNotificationRegisteredMessage(id, notificationType);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreEqual(new SerializedType(notificationType.AssemblyQualifiedName), msg.Notification);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var notificationType = typeof(string);
            var msg = new NewNotificationRegisteredMessage(id, notificationType);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(new SerializedType(notificationType.AssemblyQualifiedName), otherMsg.Notification);
        }
    }
}
