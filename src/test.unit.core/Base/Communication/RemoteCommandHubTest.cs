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
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the RemoteCommandHub class.")]
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
        [Description("Checks that endpoint sign in is correctly handled.")]
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = 
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);
            
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
        [Description("Checks that endpoint sign in is correctly handled if something goes wrong during the communication.")]
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

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);
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
        [Description("Checks that endpoint sign out is correctly handled.")]
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

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
        [Description("Checks that new commands can be reported even if there are no other registered commands for the given endpoint.")]
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
                                () => new EndpointInformationResponseMessage(e, m.Id, null),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

            var otherId = new EndpointId("other");
            reporter.Raise(
                r => r.OnNewCommandRegistered += null, 
                new CommandInformationEventArgs(
                    otherId, 
                    CommandSetProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn))));

            Assert.IsTrue(hub.HasCommandsFor(otherId));
            Assert.IsTrue(hub.HasCommandFor(otherId, typeof(IMockCommandSetWithTaskReturn)));
        }

        [Test]
        [Description("Checks that new commands can be reported at any time.")]
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

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
                r => r.OnNewCommandRegistered += null, 
                new CommandInformationEventArgs(
                    connectionInfo.Id,
                    CommandSetProxyExtensions.FromType(typeof(IMockCommandSetWithTypedTaskReturn))));

            Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
            Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTypedTaskReturn)));
        }

        [Test]
        [Description("Checks that new commands can be reported at any time.")]
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

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
                r => r.OnNewCommandRegistered += null, 
                new CommandInformationEventArgs(
                    connectionInfo.Id,
                    CommandSetProxyExtensions.FromType(typeof(IMockCommandSetWithTypedTaskReturn))));

            Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
            Assert.IsFalse(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTypedTaskReturn)));

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));
        }

        [Test]
        [Description("Checks that getting access to an unknown command set throws an exception.")]
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
                layer.Setup(l => l.DisconnectFromEndpoint(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(e)));
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

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
        [Description("Checks that it is possible to get access to a command set.")]
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

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

        [Test]
        [Description("Checks that closing the connection to an endpoint correctly cleans up all the commands for that endpoint.")]
        public void CloseConnectionTo()
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
                layer.Setup(l => l.DisconnectFromEndpoint(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(e)));
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

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

            hub.CloseConnectionTo(connectionInfo.Id);
            Assert.IsFalse(hub.HasCommandsFor(connectionInfo.Id));
        }

        [Test]
        [Description("Checks that closing the connection to all endpoints correctly cleans up all the commands for that endpoint.")]
        public void CloseConnections()
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)),
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
                layer.Setup(l => l.DisconnectFromEndpoint(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(e)));
            }

            var reporter = new Mock<IReportNewCommands>();
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(
                    () => new SuccessMessage(localEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, reporter.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

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

            hub.CloseConnections();
            Assert.IsFalse(hub.HasCommandsFor(connectionInfo.Id));
        }
    }
}
