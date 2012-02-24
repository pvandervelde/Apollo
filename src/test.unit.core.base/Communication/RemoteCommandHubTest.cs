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
    public sealed class RemoteCommandHubTest
    {
        public interface IMockCommandSetWithTaskReturn : ICommandSet
        {
            Task MyMethod(int input);
        }

        public interface IMockCommandSetWithTypedTaskReturn : ICommandSet
        {
            Task<int> MyMethod(int input);
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
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteCommandHub(
                layer.Object, 
                reporter.Object, 
                new CommandProxyBuilder(localEndpoint, sender, systemDiagnostics), 
                systemDiagnostics);
            
            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"), 
                typeof(NamedPipeChannelType), 
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) => 
                {
                    Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                    Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteCommandHub(
                layer.Object, 
                reporter.Object, 
                new CommandProxyBuilder(localEndpoint, sender, systemDiagnostics), 
                systemDiagnostics);
            hub.OnEndpointSignedIn += (s, e) => Assert.Fail();

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"), 
                typeof(NamedPipeChannelType), 
                new Uri("net.pipe://localhost/apollo_test"));
            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            // Now wait for everything to sort itself out
            Assert.IsFalse(hub.HasCommandsFor(connectionInfo.Id));
            Assert.IsFalse(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
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
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteCommandHub(
                layer.Object, 
                reporter.Object, 
                new CommandProxyBuilder(localEndpoint, sender, systemDiagnostics), 
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"), 
                typeof(NamedPipeChannelType), 
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
                {
                    Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                    Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
                };
            hub.OnEndpointSignedOff += (s, e) =>
                {
                    Assert.IsFalse(hub.HasCommandsFor(connectionInfo.Id));
                    Assert.IsFalse(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
                };

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));
            layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(connectionInfo.Id));
        }

        [Test]
        public void HandleNewCommandReportedWithoutCommands()
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteCommandHub(
                layer.Object, 
                reporter.Object, 
                new CommandProxyBuilder(localEndpoint, sender, systemDiagnostics), 
                systemDiagnostics);

            var otherId = new EndpointId("other");
            reporter.Raise(
                r => r.OnNewProxyRegistered += null, 
                new ProxyInformationEventArgs(
                    otherId, 
                    ProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn))));

            Assert.IsTrue(hub.HasCommandsFor(otherId));
            Assert.IsTrue(hub.HasCommandFor(otherId, typeof(IMockCommandSetWithTaskReturn)));
        }

        [Test]
        public void HandleNewCommandReportedWithCommands()
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
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteCommandHub(
                layer.Object, 
                reporter.Object, 
                new CommandProxyBuilder(localEndpoint, sender, systemDiagnostics), 
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            };

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            reporter.Raise(
                r => r.OnNewProxyRegistered += null, 
                new ProxyInformationEventArgs(
                    connectionInfo.Id,
                    ProxyExtensions.FromType(typeof(IMockCommandSetWithTypedTaskReturn))));

            Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
            Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTypedTaskReturn)));
        }

        [Test]
        public void HandleNewCommandReportedWithoutCommandsRegisteringDuplicate()
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
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteCommandHub(
                layer.Object, 
                reporter.Object, 
                new CommandProxyBuilder(localEndpoint, sender, systemDiagnostics), 
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"),
                typeof(NamedPipeChannelType),
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
                Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTypedTaskReturn)));
            };

            reporter.Raise(
                r => r.OnNewProxyRegistered += null, 
                new ProxyInformationEventArgs(
                    connectionInfo.Id,
                    ProxyExtensions.FromType(typeof(IMockCommandSetWithTypedTaskReturn))));

            Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
            Assert.IsFalse(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTypedTaskReturn)));

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));
        }

        [Test]
        public void CommandsForWithUnknownCommand()
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
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteCommandHub(
                layer.Object, 
                reporter.Object, 
                new CommandProxyBuilder(localEndpoint, sender, systemDiagnostics), 
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"), 
                typeof(NamedPipeChannelType), 
                new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            };

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));
            Assert.Throws<CommandNotSupportedException>(() => hub.CommandsFor<IHostCommands>(connectionInfo.Id));
        }

        [Test]
        public void CommandsFor()
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
                                () => new EndpointProxyTypesResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var hub = new RemoteCommandHub(
                layer.Object, 
                reporter.Object, 
                new CommandProxyBuilder(localEndpoint, sender, systemDiagnostics), 
                systemDiagnostics);

            var connectionInfo = new ChannelConnectionInformation(
                new EndpointId("other"), 
                typeof(NamedPipeChannelType), 
                new Uri("net.pipe://localhost/apollo_test"));
            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            var proxy = hub.CommandsFor<IMockCommandSetWithTaskReturn>(connectionInfo.Id);
            Assert.IsNotNull(proxy);
            Assert.IsInstanceOfType(typeof(CommandSetProxy), proxy);
            Assert.IsInstanceOfType(typeof(IMockCommandSetWithTaskReturn), proxy);
        }
    }
}
