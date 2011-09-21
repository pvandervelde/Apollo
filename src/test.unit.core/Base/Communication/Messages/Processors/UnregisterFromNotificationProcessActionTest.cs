//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Core.Base.Communication.Messages.Processors;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class UnregisterFromNotificationProcessActionTest
    {
        public interface IMockNotificationSetWithTypedEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Test]
        public void MessageTypeToProcess()
        {
            var sink = new Mock<ISendNotifications>();

            var action = new UnregisterFromNotificationProcessAction(sink.Object);
            Assert.AreEqual(typeof(UnregisterFromNotificationMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            EndpointId processedId = null;
            ISerializedEventRegistration registration = null;
            var sink = new Mock<ISendNotifications>();
            {
                sink.Setup(s => s.UnregisterFromNotification(It.IsAny<EndpointId>(), It.IsAny<ISerializedEventRegistration>()))
                    .Callback<EndpointId, ISerializedEventRegistration>((e, s) =>
                    {
                        processedId = e;
                        registration = s;
                    });
            }

            var action = new UnregisterFromNotificationProcessAction(sink.Object);

            var id = new EndpointId("id");
            ISerializedEventRegistration reg =
                ProxyExtensions.FromEventInfo(typeof(IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent"));
            var msg = new UnregisterFromNotificationMessage(id, reg);
            action.Invoke(msg);

            Assert.AreEqual(id, processedId);
            Assert.AreEqual(reg, registration);
        }
    }
}
