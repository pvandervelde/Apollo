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
    public sealed class NewNotificationRegisteredProcessActionTest
    {
        public interface IMockNotificationSetWithTypedEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Test]
        public void MessageTypeToProcess()
        {
            var commands = new Mock<IAcceptExternalProxyInformation>();

            var action = new NewNotificationRegisteredProcessAction(commands.Object);
            Assert.AreEqual(typeof(NewNotificationRegisteredMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            EndpointId endpoint = null;
            ISerializedType type = null;
            var notifications = new Mock<IAcceptExternalProxyInformation>();
            {
                notifications.Setup(c => c.RecentlyRegisteredProxy(It.IsAny<EndpointId>(), It.IsAny<ISerializedType>()))
                    .Callback<EndpointId, ISerializedType>(
                        (e, s) => 
                        {
                            endpoint = e;
                            type = s;
                        });
            }

            var action = new NewNotificationRegisteredProcessAction(notifications.Object);

            var newEndpoint = new EndpointId("otherEndpoint");
            action.Invoke(
                new NewNotificationRegisteredMessage(
                    newEndpoint,
                    typeof(IMockNotificationSetWithTypedEventHandler)));

            Assert.AreSame(newEndpoint, endpoint);
            Assert.AreEqual(ProxyExtensions.FromType(typeof(IMockNotificationSetWithTypedEventHandler)), type);
        }
    }
}
