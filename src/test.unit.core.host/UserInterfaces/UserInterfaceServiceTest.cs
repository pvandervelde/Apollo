//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Host.Plugins;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Apollo.Utilities.Commands;
using Apollo.Utilities.History;
using Autofac;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;
using QuickGraph;

namespace Apollo.Core.Host.UserInterfaces
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

        private static IStoreTimelineValues BuildStorage(Type type)
        {
            if (typeof(IDictionaryTimelineStorage<DatasetId, DatasetProxy>).IsAssignableFrom(type))
            {
                return new DictionaryHistory<DatasetId, DatasetProxy>();
            }

            if (typeof(IDictionaryTimelineStorage<DatasetId, DatasetOnlineInformation>).IsAssignableFrom(type))
            {
                return new DictionaryHistory<DatasetId, DatasetOnlineInformation>();
            }

            if (typeof(IBidirectionalGraphHistory<DatasetId, Edge<DatasetId>>).IsAssignableFrom(type))
            {
                return new BidirectionalGraphHistory<DatasetId, Edge<DatasetId>>();
            }

            return null;
        }

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
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            // Check that everything is the right place
            {
                var original = new List<CommandId>
                    {
                        ShutdownApplicationCommand.CommandId,
                        CreateProjectCommand.CommandId,
                        LoadProjectCommand.CommandId,
                        UnloadProjectCommand.CommandId,
                    };
                Assert.That(commandCollection, Is.EquivalentTo(original));
            }
        }

        [Test]
        public void Contains()
        {
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Contains(It.IsAny<CommandId>()))
                    .Returns(false);
            }

            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            Assert.IsFalse(service.Contains(new CommandId("bla")));
        }

        [Test]
        public void InvokeWithIdNotFullyFunctional()
        {
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>()))
                    .Verifiable();
            }

            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            Assert.Throws<ArgumentException>(() => service.Invoke(new CommandId("bla")));
        }

        [Test]
        public void InvokeWithIdFullyFunctional()
        {
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>()))
                    .Verifiable();
            }

            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            var proxy = new CoreProxy(new Mock<IKernel>().Object, systemDiagnostics);
            service.ConnectTo(proxy);

            ITimeline timeline = new Timeline(BuildStorage);
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var projects = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
            service.Start();

            service.Invoke(new CommandId("bla"));
            commands.Verify(c => c.Invoke(It.IsAny<CommandId>()), Times.Exactly(1));
        }

        [Test]
        public void InvokeWithIdAndContextNotFullyFunctional()
        {
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Verifiable();
            }

            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            Assert.Throws<ArgumentException>(() => service.Invoke(new CommandId("bla"), new Mock<ICommandContext>().Object));
        }

        [Test]
        public void InvokeWithIdAndContextFullyFunctional()
        {
            var commands = new Mock<ICommandContainer>();
            {
                commands.Setup(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Verifiable();
            }

            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            var proxy = new CoreProxy(new Mock<IKernel>().Object, systemDiagnostics);
            service.ConnectTo(proxy);

            ITimeline timeline = new Timeline(BuildStorage);
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var projects = new ProjectService(
                () => timeline,
                 d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
            service.Start();

            service.Invoke(new CommandId("bla"), new Mock<ICommandContext>().Object);
            commands.Verify(c => c.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
        public void ServicesToConnectTo()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            Assert.That(
                service.ServicesToConnectTo(),
                Is.EquivalentTo(
                    new[] 
                    { 
                        typeof(CoreProxy),
                        typeof(ProjectService),
                    }));
        }

        [Test]
        public void ConnectTo()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);
            Assert.IsFalse(service.IsConnectedToAllDependencies);

            var proxy = new CoreProxy(new Mock<IKernel>().Object, systemDiagnostics);
            service.ConnectTo(proxy);

            ITimeline timeline = new Timeline(BuildStorage);
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var projects = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);
            Assert.IsTrue(service.IsConnectedToAllDependencies);
        }

        [Test]
        public void RegisterNotificationWithNullName()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            Assert.Throws<ArgumentNullException>(() => service.RegisterNotification(null, obj => { }));
        }

        [Test]
        public void RegisterNotificationWithNullCallback()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            Assert.Throws<ArgumentNullException>(() => service.RegisterNotification(new NotificationName("bla"), null));
        }

        [Test]
        public void RegisterNotificationWithExistingName()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            Action<INotificationArguments> callback = obj => { };
            service.RegisterNotification(notificationNames.SystemShuttingDown, callback);

            Assert.DoesNotThrow(() => service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { }));
        }

        [Test]
        public void StopWithMissingAction()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            var proxy = new CoreProxy(new Mock<IKernel>().Object, systemDiagnostics);
            service.ConnectTo(proxy);

            ITimeline timeline = new Timeline(BuildStorage);
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var projects = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);
            
            Assert.Throws<MissingNotificationActionException>(service.Stop);
        }

        [Test]
        public void StopWithFailingAction()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { throw new Exception(); });

            var proxy = new CoreProxy(new Mock<IKernel>().Object, systemDiagnostics);
            service.ConnectTo(proxy);

            ITimeline timeline = new Timeline(BuildStorage);
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var projects = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);
            
            Assert.DoesNotThrow(service.Stop);
        }

        [Test]
        public void Stop()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            bool wasInvoked = false;
            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { wasInvoked = true; });

            var proxy = new CoreProxy(new Mock<IKernel>().Object, systemDiagnostics);
            service.ConnectTo(proxy);

            ITimeline timeline = new Timeline(BuildStorage);
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var projects = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);
            
            service.Stop();
            Assert.AreEqual(StartupState.Stopped, service.StartupState);
            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void StopWithMultipleNotifications()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);

            bool wasFirstInvoked = false;
            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { wasFirstInvoked = true; });

            bool wasSecondInvoked = false;
            service.RegisterNotification(notificationNames.SystemShuttingDown, obj => { wasSecondInvoked = true; });

            var proxy = new CoreProxy(new Mock<IKernel>().Object, systemDiagnostics);
            service.ConnectTo(proxy);

            ITimeline timeline = new Timeline(BuildStorage);
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var projects = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);

            service.Stop();
            Assert.AreEqual(StartupState.Stopped, service.StartupState);
            Assert.IsTrue(wasFirstInvoked);
            Assert.IsTrue(wasSecondInvoked);
        }

        [Test]
        public void HandleApplicationStartupCompleteMessage()
        {
            var commands = new Mock<ICommandContainer>();
            var notificationNames = new MockNotificationNameConstants();

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            bool isStarted = false;
            Action<INotificationArguments> onApplicationStartup = obj => { isStarted = true; };

            var builder = new ContainerBuilder();
            {
                builder.Register(c => commands.Object).As<ICommandContainer>();
                builder.Register(c => notificationNames).As<INotificationNameConstants>();
                builder.Register(c => systemDiagnostics).As<SystemDiagnostics>();
            }

            Action<IContainer> onStartService = c => { };

            var service = new UserInterfaceService(
                builder.Build(),
                onStartService,
                systemDiagnostics);
            service.RegisterNotification(notificationNames.StartupComplete, onApplicationStartup);

            var proxy = new CoreProxy(new Mock<IKernel>().Object, systemDiagnostics);
            service.ConnectTo(proxy);

            ITimeline timeline = new Timeline(BuildStorage);
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var projects = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<IHelpDistributingDatasets>().Object,
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics,
                new Mock<IBuildProjects>().Object);
            service.ConnectTo(projects);

            service.Start();

            proxy.NotifyServicesOfStartupCompletion();
            Assert.IsTrue(isStarted);
        }
    }
}
