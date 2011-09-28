﻿//-----------------------------------------------------------------------
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
using Apollo.Utilities;
using Apollo.Utilities.Commands;
using Autofac.Core;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.UserInterfaces
{
    [TestFixture]
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
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
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
        public void Contains()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Contains(It.IsAny<CommandId>()))
                    .Returns(false);
            }

            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            Assert.IsFalse(service.Contains(new CommandId("bla")));
        }

        [Test]
        public void InvokeWithIdNotFullyFunctional()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>()))
                    .Verifiable();
            }

            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            Assert.Throws<ArgumentException>(() => service.Invoke(new CommandId("bla")));
        }

        [Test]
        public void InvokeWithIdFullyFunctional()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>()))
                    .Verifiable();
            }

            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
            service.Start();

            service.Invoke(new CommandId("bla"));
            commands.Verify(c => c.Invoke(It.IsAny<CommandId>()), Times.Exactly(1));
        }

        [Test]
        public void InvokeWithIdAndContextNotFullyFunctional()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Verifiable();
            }

            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            Assert.Throws<ArgumentException>(() => service.Invoke(new CommandId("bla"), new Mock<ICommandContext>().Object));
        }

        [Test]
        public void InvokeWithIdAndContextFullyFunctional()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Verifiable();
            }

            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
            service.Start();

            service.Invoke(new CommandId("bla"), new Mock<ICommandContext>().Object);
            commands.Verify(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
        public void ServicesToConnectTo()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
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
        public void ConnectTo()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        public void RegisterNotificationWithNullName()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            Assert.Throws<ArgumentNullException>(() => service.RegisterNotification(null, obj => { }));
        }

        [Test]
        public void RegisterNotificationWithNullCallback()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            Assert.Throws<ArgumentNullException>(() => service.RegisterNotification(new NotificationName("bla"), null));
        }

        [Test]
        public void RegisterNotificationWithExistingName()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            Action<INotificationArguments> callback = obj => { };
            service.RegisterNotification(notificationNames.SystemShuttingDown, callback);

            Assert.Throws<DuplicateNotificationException>(() => service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { }));
        }

        [Test]
        public void StopWithMissingAction()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);
            
            Assert.Throws<MissingNotificationActionException>(() => service.Stop());
        }

        [Test]
        public void StopWithFailingAction()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { throw new Exception(); });

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);
            
            Assert.Throws<Exception>(() => service.Stop());
        }

        [Test]
        public void Stop()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            var service = new UserInterfaceService(
                commands.Object, 
                notificationNames,
                logger,
                onStartService);

            bool wasInvoked = false;
            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { wasInvoked = true; });

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
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
        public void HandleApplicationStartupCompleteMessage()
        {
            var commandCollection = new List<CommandId>();
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();

            Action<LogSeverityProxy, string> logger = (p, s) => { };
            Action<IModule> onStartService = module => { };

            bool isStarted = false;
            Action<INotificationArguments> onApplicationStartup = obj => { isStarted = true; };

            var service = new UserInterfaceService(
                commands.Object,
                notificationNames,
                logger,
                onStartService);
            service.RegisterNotification(notificationNames.StartupComplete, onApplicationStartup);

            var proxy = new CoreProxy(new Mock<IKernel>().Object);
            service.ConnectTo(proxy);

            var projects = new ProjectService(
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();

            proxy.NotifyServicesOfStartupCompletion();
            Assert.IsTrue(isStarted);
        }
    }
}