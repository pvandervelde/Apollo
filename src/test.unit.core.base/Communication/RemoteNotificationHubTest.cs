//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteNotificationHubTest
    {
        [Serializable]
        public sealed class MockEventArgs : EventArgs
        {
        }

        public interface IMockNotificationSetWithEventHandlerEvent : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        public interface IMockNotificationSetWithTypedEventHandlerEvent : INotificationSet
        {
            event EventHandler<MockEventArgs> OnMyEvent;
        }

        [Test]
        public void HandleEndpointSignInWithSuccessfulTransfer()
        {
            var localEndpoint = new EndpointId("local");
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localEndpoint);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockNotificationSetWithEventHandlerEvent)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewProxies>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteNotificationHub(
                layer.Object, 
                reporter.Object, 
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    logger), 
                logger);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            };

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));
        }

        [Test]
        public void HandleEndpointSignInWithFailedTransfer()
        {
            var localEndpoint = new EndpointId("local");
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localEndpoint);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => { throw new Exception(); },
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewProxies>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteNotificationHub(
                layer.Object, 
                reporter.Object, 
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    logger), 
                logger);
            hub.OnEndpointSignedIn += (s, e) => Assert.Fail();

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            // Now wait for everything to sort itself out
            Assert.IsFalse(hub.HasNotificationsFor(connectionInfo.Id));
            Assert.IsFalse(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
        }

        [Test]
        public void HandleEndpointSignOut()
        {
            var localEndpoint = new EndpointId("local");
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localEndpoint);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockNotificationSetWithEventHandlerEvent)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewProxies>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteNotificationHub(
                layer.Object, 
                reporter.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    logger), 
                logger);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            };
            hub.OnEndpointSignedOff += (s, e) =>
            {
                Assert.IsFalse(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsFalse(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            };

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));
            layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(connectionInfo.Id));
        }

        [Test]
        public void HandleNewNotificationReportedWithoutNotifications()
        {
            var localEndpoint = new EndpointId("local");
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localEndpoint);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, null),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewProxies>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteNotificationHub(
                layer.Object,
                reporter.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    logger),
                logger);

            var otherId = new EndpointId("other");
            reporter.Raise(
                r => r.OnNewProxyRegistered += null,
                new ProxyInformationEventArgs(
                    otherId,
                    ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandlerEvent))));

            Assert.IsTrue(hub.HasNotificationsFor(otherId));
            Assert.IsTrue(hub.HasNotificationFor(otherId, typeof(IMockNotificationSetWithEventHandlerEvent)));
        }

        [Test]
        public void HandleNewNotificationReportedWithNotifications()
        {
            var localEndpoint = new EndpointId("local");
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localEndpoint);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockNotificationSetWithTypedEventHandlerEvent)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewProxies>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteNotificationHub(
                layer.Object,
                reporter.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    logger),
                logger);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithTypedEventHandlerEvent)));
            };

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            reporter.Raise(
                r => r.OnNewProxyRegistered += null,
                new ProxyInformationEventArgs(
                    connectionInfo.Id,
                    ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandlerEvent))));

            Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
            Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithTypedEventHandlerEvent)));
        }

        [Test]
        public void HandleNewNotificationReportedWithoutNotificationsRegisteringDuplicate()
        {
            var localEndpoint = new EndpointId("local");
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localEndpoint);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockNotificationSetWithEventHandlerEvent)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewProxies>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteNotificationHub(
                layer.Object,
                reporter.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    logger),
                logger);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithTypedEventHandlerEvent)));
            };

            reporter.Raise(
                r => r.OnNewProxyRegistered += null,
                new ProxyInformationEventArgs(
                    connectionInfo.Id,
                    ProxyExtensions.FromType(typeof(IMockNotificationSetWithTypedEventHandlerEvent))));

            Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
            Assert.IsFalse(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithTypedEventHandlerEvent)));

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));
        }

        [Test]
        public void NotificationsForWithUnknownNotification()
        {
            var localEndpoint = new EndpointId("local");
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localEndpoint);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockNotificationSetWithEventHandlerEvent)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
                layer.Setup(l => l.DisconnectFromEndpoint(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(e)));
            }

            var reporter = new Mock<IReportNewProxies>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteNotificationHub(
                layer.Object,
                reporter.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    logger),
                logger);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasNotificationsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasNotificationFor(connectionInfo.Id, typeof(IMockNotificationSetWithEventHandlerEvent)));
            };

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));
            Assert.Throws<NotificationNotSupportedException>(
                () => hub.NotificationsFor<IMockNotificationSetWithTypedEventHandlerEvent>(connectionInfo.Id));
        }

        [Test]
        public void NotificationsFor()
        {
            var localEndpoint = new EndpointId("local");
            var layer = new Mock<ICommunicationLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localEndpoint);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Returns<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockNotificationSetWithEventHandlerEvent)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewProxies>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteNotificationHub(
                layer.Object,
                reporter.Object,
                new NotificationProxyBuilder(
                    localEndpoint,
                    (e, msg) => { },
                    logger),
                logger);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            var proxy = hub.NotificationsFor<IMockNotificationSetWithEventHandlerEvent>(connectionInfo.Id);
            Assert.IsNotNull(proxy);
            Assert.IsInstanceOfType(typeof(NotificationSetProxy), proxy);
            Assert.IsInstanceOfType(typeof(IMockNotificationSetWithEventHandlerEvent), proxy);
        }
    }
}
