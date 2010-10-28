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
using Apollo.Utils;
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

        #region Internal class - MockIKernel

        /// <summary>
        /// Defines a mock implementation of the <see cref="IKernel"/> interface.
        /// </summary>
        /// <design>
        /// This class is defined because Moq is unable to create Mock objects for 
        /// interfaces that are not publicly available (like the IKernel interface),
        /// even if those interfaces are reachable through an InternalsVisibleToAttribute.
        /// </design>
        private sealed class MockIKernel : IKernel
        {
            private bool m_CanShutdown;

            private bool m_WasShutdown;

            /// <summary>
            /// Initializes a new instance of the <see cref="MockIKernel"/> class.
            /// </summary>
            public MockIKernel() 
                : this(false)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MockIKernel"/> class.
            /// </summary>
            /// <param name="canShutdown">If set to <see langword="true"/> then the kernel can shutdown.</param>
            public MockIKernel(bool canShutdown)
            {
                m_CanShutdown = canShutdown;
                m_WasShutdown = false;
            }

            #region Implementation of IKernel

            /// <summary>
            /// Installs the specified service.
            /// </summary>
            /// <param name="service">The service which should be installed.</param>
            /// <param name="serviceDomain">The <see cref="AppDomain"/> in which the service resides.</param>
            /// <remarks>
            /// <para>
            /// Only services that are 'installed' can be used by the service manager.
            /// Services that have not been installed are simply unknown to the service
            /// manager.
            /// </para>
            /// <para>
            /// Note that only one instance for each <c>Type</c> can be provided to
            /// the service manager.
            /// </para>
            /// </remarks>
            public void Install(KernelService service, AppDomain serviceDomain)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Uninstalls the specified service.
            /// </summary>
            /// <remarks>
            ///     Once a service is uninstalled it can no longer be started. It is effectively
            ///     removed from the list of known services.
            /// </remarks>
            /// <param name="service">
            ///     The service that needs to be uninstalled.
            /// </param>
            /// <param name="shouldUnloadDomain">
            /// Indicates if the <c>AppDomain</c> that held the service should be unloaded or not.
            /// </param>
            public void Uninstall(KernelService service, bool shouldUnloadDomain)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Determines whether the application can shutdown cleanly.
            /// </summary>
            /// <returns>
            ///     <see langword="true"/> if the application can shutdown cleanly; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool CanShutdown()
            {
                return m_CanShutdown;
            }

            /// <summary>
            /// Shuts the application down.
            /// </summary>
            public void Shutdown()
            {
                m_WasShutdown = true;
            }

            /// <summary>
            /// Gets a value indicating whether [was shutdown].
            /// </summary>
            /// <value>
            ///     <see langword="true"/> if the kernel was shutdown; otherwise, <see langword="false"/>.
            /// </value>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool WasShutdown
            {
                get
                {
                    return m_WasShutdown;
                }
            }

            #endregion
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
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new MockMessageProcessingHelp(), new DnsNameConstants());
            Assert.AreElementsEqual(new[] { typeof(LogSink) }, service.ServicesToBeAvailable());
        }

        [Test]
        [Description("Checks that the object returns the correct names for services to which it should be connected.")]
        public void ServicesToConnectTo()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new MockMessageProcessingHelp(), new DnsNameConstants());
            Assert.AreElementsEqual(new[] { typeof(IMessagePipeline) }, service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new MockMessageProcessingHelp(), new DnsNameConstants());
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from an unknown dependency.")]
        public void DisconnectFromWithNonMatchingServiceType()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new MockMessageProcessingHelp(), new DnsNameConstants());
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MockKernelService());
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new MockMessageProcessingHelp(), new DnsNameConstants());
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MessagePipeline(new DnsNameConstants()));
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object can be disconnected from the dependencies.")]
        public void DisconnectFrom()
        {
            var service = new CoreProxy(new MockIKernel(), new Mock<ICommandContainer>().Object, new MockMessageProcessingHelp(), new DnsNameConstants());
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
            var processor = new MockMessageProcessingHelp();

            var service = new CoreProxy(
                new MockIKernel(), 
                commands.Object, 
                processor, 
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
            var processor = new MockMessageProcessingHelp();

            var service = new CoreProxy(
                new MockIKernel(), 
                commands.Object, 
                processor, 
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
            var processor = new MockMessageProcessingHelp();

            var service = new CoreProxy(
                new MockIKernel(), 
                commands.Object, 
                processor, 
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
            var processor = new MockMessageProcessingHelp();

            var service = new CoreProxy(
                new MockIKernel(), 
                commands.Object, 
                processor, 
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
            var processor = new MockMessageProcessingHelp();
            
            var service = new CoreProxy(
                new MockIKernel(), 
                commands.Object, 
                processor, 
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
            var kernel = new MockIKernel(false);
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var processor = new MockMessageProcessingHelp();

            var service = new CoreProxy(
                kernel, 
                commands.Object, 
                processor, 
                dnsNames);

            var pipeline = new MockPipeline();
            service.ConnectTo(pipeline);

            service.Start();
            Assert.IsTrue(processor.MessageTypes.Contains(typeof(ShutdownRequestMessage)));

            var index = processor.MessageTypes.IndexOf(typeof(ShutdownRequestMessage));
            var body = new ShutdownRequestMessage(false);
            var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfKernel);
            processor.MessageActions[index](new KernelMessage(header, body));

            Assert.IsFalse(kernel.WasShutdown);
        }

        [Test]
        [Description("Checks that ShutdownRequestMessage is handled correctly when the kernel is unable to stop yet it is forced too.")]
        public void HandleShutdownRequestMessageWithKernelUnableToStopButForced()
        {
            var commandCollection = new List<CommandId>();
            var kernel = new MockIKernel(false);
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var processor = new MockMessageProcessingHelp();

            var service = new CoreProxy(
                kernel, 
                commands.Object, 
                processor, 
                dnsNames);

            var pipeline = new MockPipeline();
            service.ConnectTo(pipeline);

            service.Start();
            Assert.IsTrue(processor.MessageTypes.Contains(typeof(ShutdownRequestMessage)));

            var index = processor.MessageTypes.IndexOf(typeof(ShutdownRequestMessage));
            var body = new ShutdownRequestMessage(true);
            var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfKernel);
            processor.MessageActions[index](new KernelMessage(header, body));

            Assert.IsTrue(kernel.WasShutdown);
        }

        [Test]
        [Description("Checks that ShutdownRequestMessage is handled correctly when the kernel is able to stop.")]
        public void HandleShutdownRequestMessageWithKernelAbleToStop()
        {
            var commandCollection = new List<CommandId>();
            var kernel = new MockIKernel(true);
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var processor = new MockMessageProcessingHelp();

            var service = new CoreProxy(
                kernel, 
                commands.Object, 
                processor, 
                dnsNames);

            var pipeline = new MockPipeline();
            service.ConnectTo(pipeline);

            service.Start();
            Assert.IsTrue(processor.MessageTypes.Contains(typeof(ShutdownRequestMessage)));

            var index = processor.MessageTypes.IndexOf(typeof(ShutdownRequestMessage));
            var body = new ShutdownRequestMessage(false);
            var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfKernel);
            processor.MessageActions[index](new KernelMessage(header, body));

            Assert.IsTrue(kernel.WasShutdown);
        }

        [Test]
        [Description("Checks that ApplicationShutdownCapabilityRequestMessage is handled correctly.")]
        public void HandleApplicationShutdownCapabilityRequestMessage()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var processor = new MockMessageProcessingHelp();

            var service = new CoreProxy(
                new MockIKernel(true), 
                commands.Object, 
                processor, 
                dnsNames);

            var pipeline = new MockPipeline();
            service.ConnectTo(pipeline);

            service.Start();
            Assert.IsTrue(processor.MessageTypes.Contains(typeof(ApplicationShutdownCapabilityRequestMessage)));

            var index = processor.MessageTypes.IndexOf(typeof(ApplicationShutdownCapabilityRequestMessage));
            var body = new ApplicationShutdownCapabilityRequestMessage();
            var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfKernel);
            processor.MessageActions[index](new KernelMessage(header, body));

            Assert.AreEqual(header.Sender, processor.Recipient);
            Assert.AreEqual(header.Id, processor.ReplyId);
            Assert.IsInstanceOfType<ApplicationShutdownCapabilityResponseMessage>(processor.Body);
        }
    }
}
