//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Projects;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.Core.Utilities.Licensing;
using Apollo.Utilities;
using Apollo.Utilities.Commands;
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

            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            // Check that everything is the right place
            {
                var original = new List<CommandId>()
                    {
                        ShutdownApplicationCommand.CommandId,
                        CreateProjectCommand.CommandId,
                        LoadProjectCommand.CommandId,
                        UnloadProjectCommand.CommandId,
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

            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
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

            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
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

            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
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

            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
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

            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
            service.Start();

            service.Invoke(new CommandId("bla"), new Mock<ICommandContext>().Object);
            commands.Verify(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that the service returns the correct service types to connect to.")]
        public void ServicesToConnectTo()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            Assert.AreElementsEqualIgnoringOrder(
                new Type[] 
                    { 
                        typeof(CoreProxy),
                        typeof(ProjectService),
                    }, 
                service.ServicesToConnectTo());
        }

        [Test]
        [Description("Checks that the object can be connected to the dependencies.")]
        public void ConnectTo()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        [Description("Checks that a notification cannot be registered without a NotificationName.")]
        public void RegisterNotificationWithNullName()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            Assert.Throws<ArgumentNullException>(() => service.RegisterNotification(null, obj => { }));
        }

        [Test]
        [Description("Checks that a notification cannot be registered without a callback method.")]
        public void RegisterNotificationWithNullCallback()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            Assert.Throws<ArgumentNullException>(() => service.RegisterNotification(new NotificationName("bla"), null));
        }

        [Test]
        [Description("Checks that a notification cannot be registered with an existing name.")]
        public void RegisterNotificationWithExistingName()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
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
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

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
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { throw new Exception(); });

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

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
            var notificationNames = new MockNotificationNameConstants();
            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                storage,
                logger,
                onStartService);

            bool wasInvoked = false;
            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { wasInvoked = true; });

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

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
            var notificationNames = new MockNotificationNameConstants();

            var storage = new LicenseValidationResultStorage();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            bool isStarted = false;
            Action<INotificationArguments> onApplicationStartup = obj => { isStarted = true; };

            var service = new UserInterfaceService(
                commands.Object,
                notificationNames,
                storage,
                logger,
                onStartService);
            service.RegisterNotification(notificationNames.StartupComplete, onApplicationStartup);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new LicenseValidationResultStorage(),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();

            proxy.NotifyServicesOfStartupCompletion();
            Assert.IsTrue(isStarted);
        }
    }
}
