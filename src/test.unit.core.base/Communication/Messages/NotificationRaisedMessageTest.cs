//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Base.Communication.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationRaisedMessageTest
    {
        [Serializable]
        private sealed class MockEventArgs : EventArgs 
        {
            public MockEventArgs(int someValue)
            {
                Value = someValue;
            }

            public int Value
            {
                get;
                private set;
            }
        }

        [Test]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var notification = new SerializedEvent(new SerializedType("a"), "b");
            var args = new MockEventArgs(1);
            var msg = new NotificationRaisedMessage(id, notification, args);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(notification, msg.Notification);
            Assert.AreSame(args, msg.Arguments);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var notification = new SerializedEvent(new SerializedType("a"), "b");
            var args = new MockEventArgs(1);
            var msg = new NotificationRaisedMessage(id, notification, args);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(notification, otherMsg.Notification);
            Assert.AreEqual(args.GetType(), otherMsg.Arguments.GetType());
            Assert.AreEqual(args.Value, ((MockEventArgs)otherMsg.Arguments).Value);
        }
    }
}
