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
using Apollo.Utils;
using MbUnit.Framework;

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

        #region internal class - MockMessageProcessingHelp

        private sealed class MockMessageProcessingHelp : IHelpMessageProcessing
        {
            private readonly IFuture<MessageBody> m_Future;

            private List<Type> m_MessageTypes = new List<Type>();

            private List<Action<KernelMessage>> m_MessageActions = new List<Action<KernelMessage>>();

            public MockMessageProcessingHelp() 
                : this(null)
            {
            }

            public MockMessageProcessingHelp(IFuture<MessageBody> future)
            {
                m_Future = future;
            }

            public void DefinePipelineInformation(IMessagePipeline pipeline, DnsName sender, Action<Exception> errorLogSender)
            {
                // Do nothing
            }

            public void DeletePipelineInformation()
            {
                // Do nothing
            }

            public void RegisterAction(Type messageType, Action<KernelMessage> messageAction)
            {
                m_MessageTypes.Add(messageType);
                m_MessageActions.Add(messageAction);
            }

            public void SendMessage(DnsName recipient, MessageBody body, MessageId originalMessage)
            {
                Recipient = recipient;
                Body = body;
                ReplyId = originalMessage;
            }

            public IFuture<MessageBody> SendMessageWithResponse(DnsName recipient, MessageBody body, MessageId originalMessage)
            {
                Recipient = recipient;
                Body = body;
                ReplyId = originalMessage;

                return m_Future;
            }

            public void ReceiveMessage(KernelMessage message)
            {
                // Do nothing
            }

            public List<Type> MessageTypes
            {
                get
                {
                    return m_MessageTypes;
                }
            }

            public List<Action<KernelMessage>> MessageActions
            {
                get
                {
                    return m_MessageActions;
                }
            }

            public DnsName Recipient
            {
                get;
                set;
            }

            public MessageBody Body
            {
                get;
                set;
            }

            public MessageId ReplyId
            {
                get;
                set;
            }
        }

        #endregion

        #region Internal class - MockKernelService

        /// <summary>
        /// Defines a mock implementation of the <see cref="KernelService"/> abstract class.
        /// </summary>
        /// <design>
        /// This class is defined because Moq is unable to create Mock objects for 
        /// classes that are not publicly available (like the KernelService class),
        /// even if those interfaces are reachable through an InternalsVisibleToAttribute.
        /// </design>
        private sealed class MockKernelService : KernelService
        {
            #region Overrides of KernelService

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform startup tasks.
            /// </summary>
            protected override void StartService()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region Internal class - MockPipeline

        /// <summary>
        /// A mock implementation of <see cref="KernelService"/> and <see cref="IMessagePipeline"/>.
        /// </summary>
        private sealed class MockPipeline : KernelService, IMessagePipeline
        {
            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StartupAction;

            /// <summary>
            /// The action executed on startup.
            /// </summary>
            private readonly Action<KernelService> m_StopAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="MockPipeline"/> class.
            /// </summary>
            public MockPipeline() : this(null, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MockPipeline"/> class.
            /// </summary>
            /// <param name="startupAction">The startup action.</param>
            /// <param name="stopAction">The stop action.</param>
            public MockPipeline(Action<KernelService> startupAction, Action<KernelService> stopAction)
            {
                m_StartupAction = startupAction;
                m_StopAction = stopAction;
            }

            #region Overrides of KernelService

            /// <summary>
            /// Starts the service.
            /// </summary>
            protected override void StartService()
            {
                if (m_StartupAction != null)
                {
                    m_StartupAction(this);
                }
            }

            /// <summary>
            /// Provides derivative classes with a possibility to
            /// perform shutdown tasks.
            /// </summary>
            protected override void StopService()
            {
                if (m_StopAction != null)
                {
                    m_StopAction(this);
                }
            }

            #endregion

            #region Implementation of IMessagePipeline

            /// <summary>
            /// Determines whether a service with the specified name is registered.
            /// </summary>
            /// <param name="name">The name of the service.</param>
            /// <returns>
            ///     <see langword="true"/> if a service with the specified name is registered; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsRegistered(DnsName name)
            {
                return false;
            }

            /// <summary>
            /// Determines whether the specified service is registered.
            /// </summary>
            /// <param name="service">The service.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified service is registered; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsRegistered(IProcessMessages service)
            {
                return false;
            }

            /// <summary>
            /// Determines whether the specified service is registered.
            /// </summary>
            /// <param name="service">The service.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified service is registered; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool IsRegistered(ISendMessages service)
            {
                return false;
            }

            /// <summary>
            /// Registers as listener.
            /// </summary>
            /// <param name="service">The service.</param>
            public void RegisterAsListener(IProcessMessages service)
            {
            }

            /// <summary>
            /// Registers as sender.
            /// </summary>
            /// <param name="service">The service.</param>
            public void RegisterAsSender(ISendMessages service)
            {
            }

            /// <summary>
            /// Registers the specified service.
            /// </summary>
            /// <param name="service">The service.</param>
            public void Register(object service)
            {
            }

            /// <summary>
            /// Unregisters the specified service.
            /// </summary>
            /// <param name="service">The service.</param>
            public void Unregister(object service)
            {
            }

            /// <summary>
            /// Unregisters as listener.
            /// </summary>
            /// <param name="service">The service.</param>
            public void UnregisterAsListener(IProcessMessages service)
            {
            }

            /// <summary>
            /// Unregisters as sender.
            /// </summary>
            /// <param name="service">The service.</param>
            public void UnregisterAsSender(ISendMessages service)
            {
            }

            /// <summary>
            /// Sends the specified sender.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="recipient">The recipient.</param>
            /// <param name="information">The information.</param>
            /// <returns>The ID number of the newly send message.</returns>
            public MessageId Send(DnsName sender, DnsName recipient, MessageBody information)
            {
                Sender = sender;
                Body = information;
                return MessageId.Next();
            }

            /// <summary>
            /// Sends the specified sender.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="recipient">The recipient.</param>
            /// <param name="information">The information.</param>
            /// <param name="inReplyTo">The in reply to.</param>
            /// <returns>The ID number of the newly send message.</returns>
            public MessageId Send(DnsName sender, DnsName recipient, MessageBody information, MessageId inReplyTo)
            {
                Sender = sender;
                Body = information;
                ReplyId = inReplyTo;

                return MessageId.Next();
            }

            public DnsName Sender
            {
                get;
                set;
            }

            public MessageBody Body
            {
                get;
                set;
            }

            public MessageId ReplyId
            {
                get;
                set;
            }

            #endregion
        }

        #endregion

        [Test]
        [Description("Checks that the object returns the correct names for services that should be available.")]
        public void ServicesToBeAvailable()
        {
            var service = new LogSink(
                new MockMessageProcessingHelp(), 
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
                new MockMessageProcessingHelp(), 
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
                new MockMessageProcessingHelp(), 
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
                new MockMessageProcessingHelp(), 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MockKernelService());
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var service = new LogSink(
                new MockMessageProcessingHelp(), 
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
                new MockMessageProcessingHelp(), 
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
                new MockMessageProcessingHelp(), 
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
                new MockMessageProcessingHelp(), 
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
                new MockMessageProcessingHelp(), 
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
                new MockMessageProcessingHelp(), 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsFalse(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Trace, "Panic!")));
        }

        [Test]
        [Description("Checks that the object does log messages for log levels that are equal or higher than the loggers level.")]
        public void ShouldLog()
        {
            var service = new LogSink(
                new MockMessageProcessingHelp(), 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            Assert.IsTrue(service.ShouldLogMessage(LogType.Debug, new LogMessage("fromHere", LevelToLog.Error, "Panic!")));
        }

        [Test]
        [Description("Checks that LogLevelChangeRequestMessage is handled correctly.")]
        public void HandleLogLevelChangeRequestMessage()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new MockMessageProcessingHelp();

            var service = new LogSink(
                processor, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MockPipeline();
            service.ConnectTo(pipeline);

            service.Start();
            Assert.IsTrue(processor.MessageTypes.Contains(typeof(LogLevelChangeRequestMessage)));

            var index = processor.MessageTypes.IndexOf(typeof(LogLevelChangeRequestMessage));
            var newLevel = LevelToLog.Fatal;
            var body = new LogLevelChangeRequestMessage(newLevel);
            var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfLogger);
            processor.MessageActions[index](new KernelMessage(header, body));

            Assert.AreEqual(newLevel, service.Level(LogType.Command));
            Assert.AreEqual(newLevel, service.Level(LogType.Debug));
        }

        [Test]
        [Description("Checks that ServiceShutdownCapabilityRequestMessage is handled correctly.")]
        public void HandleServiceShutdownCapabilityRequestMessage()
        {
            var dnsNames = new MockDnsNameConstants();
            var processor = new MockMessageProcessingHelp();

            var service = new LogSink(
                processor, 
                new LoggerConfiguration("someDir", 1, 1),
                new DebugLogTemplate(() => DateTime.Now),
                new CommandLogTemplate(() => DateTime.Now),
                new FileConstants(new ApplicationConstants()));

            var pipeline = new MockPipeline();
            service.ConnectTo(pipeline);

            service.Start();
            Assert.AreEqual(3, processor.MessageTypes.Count);
            Assert.IsTrue(processor.MessageTypes.Contains(typeof(ServiceShutdownCapabilityRequestMessage)));

            var index = processor.MessageTypes.IndexOf(typeof(ServiceShutdownCapabilityRequestMessage));
            var body = new ServiceShutdownCapabilityRequestMessage();
            var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfLogger);
            processor.MessageActions[index](new KernelMessage(header, body));

            Assert.AreEqual(header.Sender, processor.Recipient);
            Assert.AreEqual(header.Id, processor.ReplyId);
            Assert.IsInstanceOfType<ServiceShutdownCapabilityResponseMessage>(processor.Body);
        }
    }
}