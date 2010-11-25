//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Messaging;
using Apollo.Core.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Logging
{
    [TestFixture]
    [Description("Tests the LogSink class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LogSinkTest
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
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(0, service.ServicesToBeAvailable().Count());
        }

        [Test]
        [Description("Checks that the object returns the correct names for services to which it should be connected.")]
        public void ServicesToConnectTo()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreElementsEqual(new[] { typeof(IMessagePipeline) }, service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from an unknown dependency.")]
        public void DisconnectFromWithNonMatchingServiceType()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new Mock<KernelService>().Object);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MessagePipeline(new DnsNameConstants()));
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object can be disconnected from the dependencies.")]
        public void DisconnectFrom()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);

            service.DisconnectFrom(pipeline);
            Assert.IsFalse(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the log level for the debug logger can be obtained.")]
        public void LogLevelForTheDebugLogger()
        {
            var template = new DebugLogTemplate(() => DateTime.Now);
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                template,
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), service.Level(LogType.Debug));
        }

        [Test]
        [Description("Checks that the log level for the command logger can be obtained.")]
        public void LogLevelForTheCommandLogger()
        {
            var template = new CommandLogTemplate(() => DateTime.Now);
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                template,
                new FileConstants(new ApplicationConstants()));

            Assert.AreEqual(template.DefaultLogLevel(), service.Level(LogType.Command));
        }

        [Test]
        [Description("Checks that the object never logs messages for unknown log types.")]
        public void ShouldLogWithUnknownLogType()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.ShouldLogMessage(LogType.None, new LogMessage("fromHere", LevelToLog.Fatal, "Panic!")));
        }

        [Test]
        [Description("Checks that the object does not log messages for log levels that are below the loggers level.")]
        public void ShouldLogWithTooLowLevel()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Trace, "Panic!")));
        }

        [Test]
        [Description("Checks that the object does not log messages if the service is not fully functional.")]
        public void ShouldLogWhileNotFullyFunctional()
        {
            var service = new LogSink(
                new Mock<IHelpMessageProcessing>().Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Error, "Panic!")));
        }

        [Test]
        [Description("Checks that the object logs messages for log levels that are equal or higher than the loggers level.")]
        public void ShouldLog()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();

            var service = new LogSink(
                processor.Object,
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            Assert.IsTrue(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Error, "Panic!")));
        }

        [Test]
        [Description("Checks that LogLevelChangeRequestMessage is handled correctly.")]
        public void HandleLogLevelChangeRequestMessage()
        {
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

            var service = new LogSink(
                processor.Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            Assert.IsTrue(actions.ContainsKey(typeof(LogLevelChangeRequestMessage)));

            var newLevel = LevelToLog.Fatal;
            var body = new LogLevelChangeRequestMessage(newLevel);
            var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfLogger);
            actions[typeof(LogLevelChangeRequestMessage)](new KernelMessage(header, body));

            Assert.AreEqual(newLevel, service.Level(LogType.Command));
            Assert.AreEqual(newLevel, service.Level(LogType.Debug));
        }

        [Test]
        [Description("Checks that ServiceShutdownCapabilityRequestMessage is handled correctly.")]
        public void HandleServiceShutdownCapabilityRequestMessage()
        {
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

            var service = new LogSink(
                processor.Object, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();
            Assert.IsTrue(actions.ContainsKey(typeof(ServiceShutdownCapabilityRequestMessage)));
            {
                var body = new ServiceShutdownCapabilityRequestMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfLogger);
                actions[typeof(ServiceShutdownCapabilityRequestMessage)](new KernelMessage(header, body));

                Assert.AreEqual(header.Sender, storedSender);
                Assert.AreEqual(header.Id, storedInReplyTo);
                Assert.IsInstanceOfType<ServiceShutdownCapabilityResponseMessage>(storedBody);
            }
        }
    }
}