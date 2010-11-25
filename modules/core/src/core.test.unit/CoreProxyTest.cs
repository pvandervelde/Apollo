//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Utils.Commands;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the CoreProxy class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CoreProxyTest
    {
        #region internal class - MockDnsNameConstants

        private sealed class MockDnsNameConstants : IDnsNameConstants
        {
            public DnsName AddressOfMessagePipeline
            {
                get 
                { 
                    return new DnsName("pipeline");
                }
            }

            public DnsName AddressOfKernel
            {
                get 
                { 
                    return new DnsName("kernel");
                }
            }

            public DnsName AddressOfUserInterface
            {
                get 
                { 
                    return new DnsName("ui");
                }
            }

            public DnsName AddressOfLogger
            {
                get 
                { 
                    return new DnsName("logger");
                }
            }
        }
        
        #endregion

        [Test]
        [Description("Checks that the object returns the correct names for services that should be available.")]
        public void ServicesToBeAvailable()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.AreElementsEqual(new[] { typeof(LogSink) }, service.ServicesToBeAvailable());
        }

        [Test]
        [Description("Checks that the object returns the correct names for services to which it should be connected.")]
        public void ServicesToConnectTo()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.AreElementsEqual(new[] { typeof(IMessagePipeline) }, service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from an unknown dependency.")]
        public void DisconnectFromWithNonMatchingServiceType()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new Mock<KernelService>().Object);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MessagePipeline(new DnsNameConstants()));
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object can be disconnected from the dependencies.")]
        public void DisconnectFrom()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object, new Mock<ICommandContainer>().Object, new Mock<IHelpMessageProcessing>().Object, new DnsNameConstants());
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);

            service.DisconnectFrom(pipeline);
            Assert.IsFalse(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the Contains method returns the correct value.")]
        public void Contains()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Contains(It.IsAny<CommandId>()))
                    .Returns(false);
            }

            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();

            var service = new CoreProxy(
                new Mock<IKernel>().Object, 
                commands.Object,
                processor.Object, 
                dnsNames);

            Assert.IsFalse(service.Contains(new CommandId("bla")));
        }

        [Test]
        [Description("Checks that the Invoke(CommandId) method returns without invoking if the service is not fully functional.")]
        public void InvokeWithIdNotFullyFunctional()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>()))
                    .Verifiable();
            }

            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();

            var service = new CoreProxy(
                new Mock<IKernel>().Object, 
                commands.Object,
                processor.Object, 
                dnsNames);

            Assert.Throws<ArgumentException>(() => service.Invoke(new CommandId("bla")));
        }

        [Test]
        [Description("Checks that the Invoke(CommandId) method invokes the command if the service is fully functional.")]
        public void InvokeWithIdFullyFunctional()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>()))
                    .Verifiable();
            }

            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();

            var service = new CoreProxy(
                new Mock<IKernel>().Object, 
                commands.Object,
                processor.Object, 
                dnsNames);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            service.Start();

            service.Invoke(new CommandId("bla"));
            commands.Verify(c => c.Invoke(It.IsAny<CommandId>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that the Invoke(CommandId, ICommandContext) method returns without invoking if the service is not fully functional.")]
        public void InvokeWithIdAndContextNotFullyFunctional()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Verifiable();
            }

            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();

            var service = new CoreProxy(
                new Mock<IKernel>().Object, 
                commands.Object,
                processor.Object, 
                dnsNames);

            Assert.Throws<ArgumentException>(() => service.Invoke(new CommandId("bla"), new Mock<ICommandContext>().Object));
        }

        [Test]
        [Description("Checks that the Invoke(CommandId, ICommandContext) method invokes the command if the service is fully functional.")]
        public void InvokeWithIdAndContextFullyFunctional()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Verifiable();
            }

            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            
            var service = new CoreProxy(
                new Mock<IKernel>().Object, 
                commands.Object, 
                processor.Object, 
                dnsNames);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            service.Start();

            service.Invoke(new CommandId("bla"), new Mock<ICommandContext>().Object);
            commands.Verify(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that ShutdownRequestMessage is handled correctly when the kernel is unable to stop.")]
        public void HandleShutdownRequestMessageWithKernelUnableToStop()
        {
            var commandCollection = new List<CommandId>();

            bool wasShutdown = false;
            var kernel = new Mock<IKernel>();
            {
                kernel.Setup(k => k.CanShutdown())
                    .Returns(false);
                kernel.Setup(k => k.Shutdown())
                    .Callback(() => wasShutdown = true);
            }

            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();

            var actions = new Dictionary<Type, Action<KernelMessage>>();
            var processor = new Mock<IHelpMessageProcessing>();
            {
                processor.Setup(p => p.RegisterAction(It.IsAny<Type>(), It.IsAny<Action<KernelMessage>>()))
                    .Callback<Type, Action<KernelMessage>>(
                        (t, a) =>
                        {
                            actions.Add(t, a);
                        });
            }

            var service = new CoreProxy(
                kernel.Object,
                commands.Object,
                processor.Object,
                dnsNames);

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            Assert.IsTrue(actions.ContainsKey(typeof(ShutdownRequestMessage)));
            {
                var body = new ShutdownRequestMessage(false);
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfKernel);
                actions[typeof(ShutdownRequestMessage)](new KernelMessage(header, body));

                Assert.IsFalse(wasShutdown);
            }
        }

        [Test]
        [Description("Checks that ShutdownRequestMessage is handled correctly when the kernel is unable to stop yet it is forced too.")]
        public void HandleShutdownRequestMessageWithKernelUnableToStopButForced()
        {
            var commandCollection = new List<CommandId>();
            
            bool wasShutdown = false;
            var kernel = new Mock<IKernel>();
            {
                kernel.Setup(k => k.CanShutdown())
                    .Returns(true);
                kernel.Setup(k => k.Shutdown())
                    .Callback(() => wasShutdown = true);
            }

            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();

            var actions = new Dictionary<Type, Action<KernelMessage>>();
            var processor = new Mock<IHelpMessageProcessing>();
            {
                processor.Setup(p => p.RegisterAction(It.IsAny<Type>(), It.IsAny<Action<KernelMessage>>()))
                    .Callback<Type, Action<KernelMessage>>(
                        (t, a) =>
                        {
                            actions.Add(t, a);
                        });
            }

            var service = new CoreProxy(
                kernel.Object, 
                commands.Object, 
                processor.Object, 
                dnsNames);

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            Assert.IsTrue(actions.ContainsKey(typeof(ShutdownRequestMessage)));
            {
                var body = new ShutdownRequestMessage(true);
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfKernel);
                actions[typeof(ShutdownRequestMessage)](new KernelMessage(header, body));

                Assert.IsTrue(wasShutdown);
            }
        }

        [Test]
        [Description("Checks that ShutdownRequestMessage is handled correctly when the kernel is able to stop.")]
        public void HandleShutdownRequestMessageWithKernelAbleToStop()
        {
            var commandCollection = new List<CommandId>();

            bool wasShutdown = false;
            var kernel = new Mock<IKernel>();
            {
                kernel.Setup(k => k.CanShutdown())
                    .Returns(true);
                kernel.Setup(k => k.Shutdown())
                    .Callback(() => wasShutdown = true);
            }

            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();

            var actions = new Dictionary<Type, Action<KernelMessage>>();
            var processor = new Mock<IHelpMessageProcessing>();
            {
                processor.Setup(p => p.RegisterAction(It.IsAny<Type>(), It.IsAny<Action<KernelMessage>>()))
                    .Callback<Type, Action<KernelMessage>>(
                        (t, a) =>
                        {
                            actions.Add(t, a);
                        });
            }

            var service = new CoreProxy(
                kernel.Object, 
                commands.Object, 
                processor.Object, 
                dnsNames);

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            Assert.IsTrue(actions.ContainsKey(typeof(ShutdownRequestMessage)));
            {
                var body = new ShutdownRequestMessage(false);
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfKernel);
                actions[typeof(ShutdownRequestMessage)](new KernelMessage(header, body));

                Assert.IsTrue(wasShutdown);
            }
        }

        [Test]
        [Description("Checks that ApplicationShutdownCapabilityRequestMessage is handled correctly.")]
        public void HandleApplicationShutdownCapabilityRequestMessage()
        {
            var commandCollection = new List<CommandId>();
            var kernel = new Mock<IKernel>();
            {
                kernel.Setup(k => k.CanShutdown())
                    .Returns(true);
            }

            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();

            var actions = new Dictionary<Type, Action<KernelMessage>>();
            DnsName storedSender = null;
            MessageBody storedBody = null;
            MessageId storedInReplyTo = null;
            var processor = new Mock<IHelpMessageProcessing>();
            {
                processor.Setup(p => p.RegisterAction(It.IsAny<Type>(), It.IsAny<Action<KernelMessage>>()))
                    .Callback<Type, Action<KernelMessage>>(
                        (t, a) =>
                        {
                            actions.Add(t, a);
                        });
                processor.Setup(p => p.SendMessage(It.IsAny<DnsName>(), It.IsAny<MessageBody>(), It.IsAny<MessageId>()))
                    .Callback<DnsName, MessageBody, MessageId>(
                        (d, b, m) =>
                        {
                            storedSender = d;
                            storedBody = b;
                            storedInReplyTo = m;
                        });
            }
            
            var service = new CoreProxy(
                kernel.Object, 
                commands.Object, 
                processor.Object, 
                dnsNames);

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            Assert.IsTrue(actions.ContainsKey(typeof(ApplicationShutdownCapabilityRequestMessage)));
            {
                var body = new ApplicationShutdownCapabilityRequestMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfKernel);
                actions[typeof(ApplicationShutdownCapabilityRequestMessage)](new KernelMessage(header, body));

                Assert.AreEqual(header.Sender, storedSender);
                Assert.AreEqual(header.Id, storedInReplyTo);
                Assert.IsInstanceOfType<ApplicationShutdownCapabilityResponseMessage>(storedBody);
            }
        }
    }
}
