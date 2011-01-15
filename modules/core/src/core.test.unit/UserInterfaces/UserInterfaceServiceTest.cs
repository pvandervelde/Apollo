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

            public NotificationName CanSystemShutdown
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

        [Test]
        [Description("Checks that an object cannot be created without an ICommandContainer object.")]
        public void CreateWithNullCommands()
        {
            var dnsNames = new MockDnsNameConstants();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                null, 
                dnsNames, 
                notificationNames, 
                processor.Object,
                storage,
                onStartService));
        }

        [Test]
        [Description("Checks that an object cannot be created without an IDnsNameConstants object.")]
        public void CreateWithNullDnsNames()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object, 
                null, 
                notificationNames,
                processor.Object,
                storage,
                onStartService));
        }

        [Test]
        [Description("Checks that an object cannot be created without an INotificationNameConstants object.")]
        public void CreateWithNullNotificationNames()
        {
            var commands = new Mock<ICommandContainer>();
            var dnsNames = new MockDnsNameConstants();
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                null,
                processor.Object, 
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
            var processor = new Mock<IHelpMessageProcessing>();
            Action<IModule> onStartService = module => { };

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object,
                dnsNames,
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();

            Assert.Throws<ArgumentNullException>(() => new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object, 
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
                storage,
                onStartService);
            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.DisconnectFrom(new Mock<KernelService>().Object);
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
                storage,
                onStartService);

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);
            
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
                storage,
                onStartService);

            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { throw new Exception(); });

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);
            
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
            var processor = new Mock<IHelpMessageProcessing>();
            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                dnsNames, 
                notificationNames,
                processor.Object,
                storage,
                onStartService);

            bool wasInvoked = false;
            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { wasInvoked = true; });

            var pipeline = new MessagePipeline(new DnsNameConstants());
            service.ConnectTo(pipeline);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);
            
            service.Stop();
            Assert.AreEqual(StartupState.Stopped, service.StartupState);
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

            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            bool isStarted = false;
            Action<INotificationArguments> onApplicationStartup = obj => { isStarted = true; };

            var service = new UserInterfaceService(
                commands.Object,
                dnsNames,
                notificationNames,
                processor.Object,
                storage,
                onStartService);
            service.RegisterNotification(notificationNames.StartupComplete, onApplicationStartup);

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();

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

            var storage = new LicenseValidationResultStorage();
            Action<IModule> onStartService = module => { };

            bool hasMessageBeenReceived = false;
            Action<INotificationArguments> onCanSystemShutDown = obj => { hasMessageBeenReceived = true; };

            var service = new UserInterfaceService(
                commands.Object,
                dnsNames,
                notificationNames,
                processor.Object,
                storage,
                onStartService);
            service.RegisterNotification(notificationNames.CanSystemShutdown, onCanSystemShutDown);

            var pipeline = new Mock<KernelService>();
            var pipelineInterface = pipeline.As<IMessagePipeline>();
            service.ConnectTo(pipeline.Object);

            service.Start();

            Assert.IsTrue(actions.ContainsKey(typeof(ServiceShutdownCapabilityRequestMessage)));
            {
                var body = new ServiceShutdownCapabilityRequestMessage();
                var header = new MessageHeader(MessageId.Next(), new DnsName("bla"), dnsNames.AddressOfUserInterface);
                actions[typeof(ServiceShutdownCapabilityRequestMessage)](new KernelMessage(header, body));

                Assert.IsTrue(hasMessageBeenReceived);

                Assert.AreEqual(header.Sender, storedSender);
                Assert.AreEqual(header.Id, storedInReplyTo);
                Assert.IsInstanceOfType<ServiceShutdownCapabilityResponseMessage>(storedBody);
            }
        }
    }
}
