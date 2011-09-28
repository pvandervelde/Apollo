//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Communication.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationRaisedProcessActionTest
    {
        public interface IMockNotificationSetWithTypedEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Test]
        public void MessageTypeToProcess()
        {
            var commands = new Mock<INotifyOfRemoteEndpointEvents>();
            Action<LogSeverityProxy, string> logger = (p, t) => { };

            var action = new NotificationRaisedProcessAction(commands.Object, logger);
            Assert.AreEqual(typeof(NotificationRaisedMessage), action.MessageTypeToProcess);
        }
        
        [Test]
        public void Invoke()
        {
            var local = new EndpointId("local");
            Action<EndpointId, ICommunicationMessage> messageSender = (e, m) => { };

            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var builder = new NotificationProxyBuilder(local, messageSender, logger);

            var remoteEndpoint = new EndpointId("other");
            var proxy = builder.ProxyConnectingTo(remoteEndpoint, typeof(IMockNotificationSetWithTypedEventHandler));

            object sender = null;
            EventArgs args = null;
            ((IMockNotificationSetWithTypedEventHandler)proxy).OnMyEvent += 
                (s, e) => 
                {
                    sender = s;
                    args = e;
                };

            var commandSets = new List<KeyValuePair<Type, INotificationSet>> 
                { 
                    new KeyValuePair<Type, INotificationSet>(typeof(IMockNotificationSetWithTypedEventHandler), proxy)
                };

            var notifications = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notifications.Setup(c => c.NotificationsFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns(commandSets[0].Value);
            }

            var action = new NotificationRaisedProcessAction(notifications.Object, logger);

            var eventArgs = new EventArgs();
            action.Invoke(
                new NotificationRaisedMessage(
                    new EndpointId("otherId"),
                    ProxyExtensions.FromEventInfo(typeof(IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent")),
                    eventArgs));

            Assert.AreSame(proxy, sender);
            Assert.AreSame(eventArgs, args);
        }
    }
}
