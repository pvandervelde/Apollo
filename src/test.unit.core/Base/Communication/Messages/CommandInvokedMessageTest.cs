//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using MbUnit.Framework;

namespace Apollo.Base.Communication.Messages
{
    [TestFixture]
    [Description("Tests the CommandInvokedMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommandInvokedMessageTest
    {
        [Test]
        [Description("Checks that a message can be created.")]
        public void Create()
        {
            var id = new EndpointId("endpoint");
            var methodInvocation = CommandSetProxyExtensions.FromMethodInfo(MethodInfo.GetCurrentMethod(), new object[0]);
            var msg = new CommandInvokedMessage(id, methodInvocation);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(methodInvocation, msg.Invocation);
        }

        [Test]
        [Description("Checks that the message serialises and deserialises correctly.")]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("endpoint");
            var methodInvocation = CommandSetProxyExtensions.FromMethodInfo(MethodInfo.GetCurrentMethod(), new object[0]);
            var msg = new CommandInvokedMessage(id, methodInvocation);
            var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(msg.Id, otherMsg.Id);
            Assert.AreEqual(MessageId.None, otherMsg.InResponseTo);
            Assert.AreEqual(methodInvocation, otherMsg.Invocation, (x, y) => x.Equals(y));
        }
    }
}
