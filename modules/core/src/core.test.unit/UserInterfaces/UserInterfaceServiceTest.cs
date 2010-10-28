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
using Apollo.Core.Utils.Licensing;
using Apollo.Utils;
using Apollo.Utils.Commands;
using Autofac.Core;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.UserInterfaces
{
    [TestFixture]
    [Description("Tests the FusionHelper class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class UserInterfaceServiceTest
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

        #region internal class - MockNotificationNameConstants

        private sealed class MockNotificationNameConstants : INotificationNameConstants
        {
            public NotificationName StartupComplete
            {
                get
                {
                    return new NotificationName("StartupComplete");
                }
            }

            public NotificationName CanSystemShutDown
            {
                get
                {
                    return new NotificationName("CanSystemShutDown");
                }
            }

            public NotificationName SystemShuttingDown
            {
                get 
                {
                    return new NotificationName("SystemShuttingDown");
                }
            }
        }
        
        #endregion

        #region internal class - MockMessageProcessingHelp

        private sealed class MockMessageProcessingHelp : IHelpMessageProcessing
        {
            private readonly IFuture<MessageBody> m_Future;

            private Dictionary<Type, Action<KernelMessage>> m_MessageActions = new Dictionary<Type, Action<KernelMessage>>();

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
                m_MessageActions.Add(messageType, messageAction);
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

            public IDictionary<Type, Action<KernelMessage>> MessageActions
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
        [Description("Checks that an object cannot be created without an ICommandContainer object.")]
        public void CreateWithNullCommands()
        {
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                null, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IDnsNameConstants object.")]
        public void CreateWithNullDnsNames()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object, 
                null, 
                notificationNames, 
                processor,
                storage,
                onStartService));
        }

        [Test]
        [Description("Checks that an object cannot be created without an INotificationNameConstants object.")]
        public void CreateWithNullNotificationNames()
        {
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                null, 
                processor, 
                storage,
                onStartService));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IHelpMessageProcessing object.")]
        public void CreateWithNullMessageProcessor()
        {
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object,
                dnsNames,
                notificationNames,
                null,
                storage,
                onStartService));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IValidationResultStorage object.")]
        public void CreateWithNullValidationResultStorage()
        {
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object,
                dnsNames,
                notificationNames,
                processor,
                null,
                onStartService));
        }

        [Test]
        [Description("Checks that an object cannot be created without an start service delegate.")]
        public void CreateWithNullOnStartService()
        {
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor, 
                storage,
                null));
        }

        [Test]
        [Description("Checks that the object is created correctly.")]
        public void Create()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Add(It.IsAny<CommandId>(), It.IsAny<Func<ICommand>>()))
                    .Callback<CommandId, Func<ICommand>>(
                        (id, command) => 
                            {
                                commandCollection.Add(id);
                            });
            }

            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            // Check that everything is the right place
            {
                var original = new List<CommandId>()
                    {
                        CheckApplicationCanShutdownCommand.CommandId,
                        ShutdownApplicationCommand.CommandId
                    };
                Assert.AreElementsEqualIgnoringOrder(original, commandCollection);
            }
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
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

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
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

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
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

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
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

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
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            service.Start();

            service.Invoke(new CommandId("bla"), new Mock<ICommandContext>().Object);
            commands.Verify(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that the service returns the correct service types that should be available.")]
        public void ServicesToBeAvailable()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            Assert.AreElementsEqualIgnoringOrder(new Type[] { typeof(LogSink) }, service.ServicesToBeAvailable());
        }

        [Test]
        [Description("Checks that the service returns the correct service types to connect to.")]
        public void ServicesToConnectTo()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            Assert.AreElementsEqualIgnoringOrder(new Type[] { typeof(IMessagePipeline) }, service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from an unknown dependency.")]
        public void DisconnectFromWithNonMatchingServiceType()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MockKernelService());
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object cannot be disconnected from a non-matching reference.")]
        public void DisconnectFromWithNonMatchingObjectReference()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new MessagePipeline(new DnsNameConstants()));
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that the object can be disconnected from the dependencies.")]
        public void DisconnectFrom()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);
            Assert.IsTrue(service.IsConnectedToAllDependencies);

            service.DisconnectFrom(pipeline);
            Assert.IsFalse(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that a notification cannot be registered without a NotificationName.")]
        public void RegisterNotificationWithNullName()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            Assert.Throws<ArgumentNullException>(() => service.RegisterNotification(null, obj => { }));
        }

        [Test]
        [Description("Checks that a notification cannot be registered without a callback method.")]
        public void RegisterNotificationWithNullCallback()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            Assert.Throws<ArgumentNullException>(() => service.RegisterNotification(new NotificationName("bla"), null));
        }

        [Test]
        [Description("Checks that a notification cannot be registered with an existing name.")]
        public void RegisterNotificationWithExistingName()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            Action<INotificationArguments> callback = obj => { };
            service.RegisterNotification(notificationNames.SystemShuttingDown, callback);

            Assert.Throws<DuplicateNotificationException>(() => service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { }));
        }

        [Test]
        [Description("Checks that stopping the service without a shutdown callback results in an exception.")]
        public void StopWithMissingAction()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.GetStartupState());
            
            Assert.Throws<MissingNotificationActionException>(() => service.Stop());
        }

        [Test]
        [Description("Checks that stopping the service with a failing shutdown callback results in an exception.")]
        public void StopWithFailingAction()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { throw new Exception(); });

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.GetStartupState());
            
            Assert.Throws<Exception>(() => service.Stop());
        }

        [Test]
        [Description("Checks that stopping the service calls the shutdown action.")]
        public void Stop()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames, 
                processor,
                storage,
                onStartService);

            bool wasInvoked = false;
            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { wasInvoked = true; });

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.GetStartupState());
            
            service.Stop();
            Assert.AreEqual(StartupState.Stopped, service.GetStartupState());
            Assert.IsTrue(wasInvoked);
        }

        [Test]
        [Description("Checks that ApplicationStartupCompleteMessage is handled correctly.")]
        public void HandleApplicationStartupCompleteMessage()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            bool isStarted = false;
            Action<INotificationArguments> onApplicationStartup = obj => { isStarted = true; };

            var service = new UserInterfaceService(
                commands.Object,
                dnsNames,
                notificationNames,
                processor,
                storage,
                onStartService);
            service.RegisterNotification(notificationNames.StartupComplete, onApplicationStartup);

            var pipeline = new MockPipeline();
            service.ConnectTo(pipeline);

            service.Start();

            var actions = processor.MessageActions;

            Assert.IsTrue(actions.ContainsKey(typeof(ApplicationStartupCompleteMessage)));
            {
                var body = new ApplicationStartupCompleteMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla1"), dnsNames.AddressOfUserInterface);
                actions[typeof(ApplicationStartupCompleteMessage)](new KernelMessage(header, body));

                Assert.IsTrue(isStarted);
            }
        }

        [Test]
        [Description("Checks that ServiceShutdownCapabilityRequestMessage is handled correctly.")]
        public void HandleServiceShutdownCapabilityRequestMessage()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new MockMessageProcessingHelp();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            bool hasMessageBeenReceived = false;
            Action<INotificationArguments> onCanSystemShutDown = obj => { hasMessageBeenReceived = true; };

            var service = new UserInterfaceService(
                commands.Object,
                dnsNames,
                notificationNames,
                processor,
                storage,
                onStartService);
            service.RegisterNotification(notificationNames.CanSystemShutDown, onCanSystemShutDown);

            var pipeline = new MockPipeline();
            service.ConnectTo(pipeline);

            service.Start();

            var actions = processor.MessageActions;

            Assert.IsTrue(actions.ContainsKey(typeof(ServiceShutdownCapabilityRequestMessage)));
            {
                var body = new ServiceShutdownCapabilityRequestMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfUserInterface);
                actions[typeof(ServiceShutdownCapabilityRequestMessage)](new KernelMessage(header, body));

                Assert.IsTrue(hasMessageBeenReceived);

                Assert.AreEqual(header.Sender, processor.Recipient);
                Assert.AreEqual(header.Id, processor.ReplyId);
                Assert.IsInstanceOfType<ServiceShutdownCapabilityResponseMessage>(processor.Body);
            }
        }
    }
}
