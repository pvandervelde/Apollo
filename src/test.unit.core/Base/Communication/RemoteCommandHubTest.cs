//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)));
                        });
            }

            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = 
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(() => new SuccessMessage(localEndpoint, new MessageId()));
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, new CommandProxyBuilder(localEndpoint, sender), logger);
            
            var connectionInfo = new ChannelConnectionInformation(new EndpointId("other"), typeof(NamedPipeChannelType), new Uri("net.pipe://localhost/apollo_test"));
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
                            return Task<ICommunicationMessage>.Factory.StartNew(() => { throw new Exception(); });
                        });
            }

            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(() => new SuccessMessage(localEndpoint, new MessageId()));
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, new CommandProxyBuilder(localEndpoint, sender), logger);
            hub.OnEndpointSignedIn += (s, e) => Assert.Fail();

            var connectionInfo = new ChannelConnectionInformation(new EndpointId("other"), typeof(NamedPipeChannelType), new Uri("net.pipe://localhost/apollo_test"));
            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            // Now wait for everything to sort itself out
            Thread.Sleep(50);
            Assert.IsFalse(hub.HasCommandsFor(connectionInfo.Id));
            Assert.IsFalse(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
        }

        [Test]
        [Description("Checks that endpoint sign off is correctly handled.")]
        public void HandleEndpointSignOff()
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)));
                        });
            }

            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(() => new SuccessMessage(localEndpoint, new MessageId()));
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

            var connectionInfo = new ChannelConnectionInformation(new EndpointId("other"), typeof(NamedPipeChannelType), new Uri("net.pipe://localhost/apollo_test"));
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

            Thread.Sleep(50);
            layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(connectionInfo.Id));
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)));
                        });
                layer.Setup(l => l.DisconnectFromEndpoint(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(e)));
            }

            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(() => new SuccessMessage(localEndpoint, new MessageId()));
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

            var connectionInfo = new ChannelConnectionInformation(new EndpointId("other"), typeof(NamedPipeChannelType), new Uri("net.pipe://localhost/apollo_test"));
            hub.OnEndpointSignedIn += (s, e) =>
            {
                Assert.IsTrue(hub.HasCommandsFor(connectionInfo.Id));
                Assert.IsTrue(hub.HasCommandFor(connectionInfo.Id, typeof(IMockCommandSetWithTaskReturn)));
            };

            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            Thread.Sleep(50);
            Assert.Throws<CommandNotSupportedException>(() => hub.CommandsFor<IMachineCommands>(connectionInfo.Id));
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)));
                        });
            }

            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(() => new SuccessMessage(localEndpoint, new MessageId()));
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

            var connectionInfo = new ChannelConnectionInformation(new EndpointId("other"), typeof(NamedPipeChannelType), new Uri("net.pipe://localhost/apollo_test"));
            layer.Raise(l => l.OnEndpointSignedIn += null, new ConnectionInformationEventArgs(connectionInfo));

            Thread.Sleep(50);
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)));
                        });
                layer.Setup(l => l.DisconnectFromEndpoint(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(e)));
            }

            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(() => new SuccessMessage(localEndpoint, new MessageId()));
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

            var connectionInfo = new ChannelConnectionInformation(new EndpointId("other"), typeof(NamedPipeChannelType), new Uri("net.pipe://localhost/apollo_test"));
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

            Thread.Sleep(50);
            hub.CloseConnectionTo(connectionInfo.Id);

            Thread.Sleep(50);
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
                                () => new EndpointInformationResponseMessage(e, m.Id, typeof(IMockCommandSetWithTaskReturn)));
                        });
                layer.Setup(l => l.DisconnectFromEndpoint(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => layer.Raise(l => l.OnEndpointSignedOut += null, new EndpointEventArgs(e)));
            }

            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender =
                (e, m) => Task<ICommunicationMessage>.Factory.StartNew(() => new SuccessMessage(localEndpoint, new MessageId()));
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var hub = new RemoteCommandHub(layer.Object, new CommandProxyBuilder(localEndpoint, sender), logger);

            var connectionInfo = new ChannelConnectionInformation(new EndpointId("other"), typeof(NamedPipeChannelType), new Uri("net.pipe://localhost/apollo_test"));
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

            Thread.Sleep(50);
            hub.CloseConnections();

            Thread.Sleep(50);
            Assert.IsFalse(hub.HasCommandsFor(connectionInfo.Id));
        }
    }
}
