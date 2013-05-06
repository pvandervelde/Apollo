//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base.Loaders;
using MbUnit.Framework;
using Moq;
using Nuclei.Communication;
using Nuclei.Diagnostics;

namespace Apollo.Core.Base
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetOnlineInformationTest
    {
        public interface IMockCommandSetWithTaskReturn : ICommandSet
        {
            Task MyMethod(int input);
        }

        public interface IMockCommandSetWithTypedTaskReturn : ICommandSet
        {
            Task<int> MyMethod(int input);
        }

        [InternalCommand]
        public interface IMockCommandSetForInternalUse : ICommandSet
        {
            Task MyInternalMethod();
        }

        public interface IMockNotificationSetWithEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Serializable]
        public class MySerializableEventArgs : EventArgs
        {
        }

        public interface IMockNotificationSetWithTypedEventHandler : INotificationSet
        {
            event EventHandler<MySerializableEventArgs> OnMyEvent;
        }

        [InternalNotification]
        public interface IMockNotificationSetForInternalUse : INotificationSet
        {
            event EventHandler<MySerializableEventArgs> OnMyEvent;
        }

        [Test]
        public void Create()
        {
            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var notifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(n => n.HasNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(notifications.Object);
            }

            var info = new DatasetOnlineInformation(
                id, 
                endpoint, 
                networkId, 
                commandHub.Object,
                notificationHub.Object,
                systemDiagnostics);
            Assert.AreSame(id, info.Id);
            Assert.AreSame(endpoint, info.Endpoint);
            Assert.AreSame(networkId, info.RunsOn);
        }

        [Test]
        public void AvailableCommands()
        {
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var commandList = new Dictionary<Type, ICommandSet> 
                {
                    { 
                        typeof(IMockCommandSetWithTaskReturn),
                        new Mock<IMockCommandSetWithTaskReturn>().Object
                    },
                    {
                        typeof(IMockCommandSetWithTypedTaskReturn),
                        new Mock<IMockCommandSetWithTaskReturn>().Object
                    },
                    {
                        typeof(IMockCommandSetForInternalUse),
                        new Mock<IMockCommandSetWithTaskReturn>().Object
                    },
                };

            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandHub.Setup(h => h.AvailableCommandsFor(It.IsAny<EndpointId>()))
                    .Returns(commandList.Keys);
                commandHub.Setup(h => h.CommandsFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns<EndpointId, Type>((e, t) => commandList[t]);
            }

            var notifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(n => n.HasNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(notifications.Object);
            }

            var info = new DatasetOnlineInformation(
                id, 
                endpoint, 
                networkId, 
                commandHub.Object,
                notificationHub.Object,
                systemDiagnostics);
            var commands = info.AvailableCommands();

            Assert.AreEqual(2, commands.Count());
        }

        [Test]
        public void Command()
        {
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var commandList = new SortedList<Type, ICommandSet> 
                {
                    { 
                        typeof(IMockCommandSetWithTaskReturn),
                        new Mock<IMockCommandSetWithTaskReturn>().Object
                    },
                };
            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandHub.Setup(h => h.CommandsFor<IMockCommandSetWithTaskReturn>(It.IsAny<EndpointId>()))
                    .Returns((IMockCommandSetWithTaskReturn)commandList.Values[0]);
            }

            var notifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(n => n.HasNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(notifications.Object);
            }

            var info = new DatasetOnlineInformation(
                id, 
                endpoint, 
                networkId, 
                commandHub.Object,
                notificationHub.Object,
                systemDiagnostics);
            var commands = info.Command<IMockCommandSetWithTaskReturn>();
            Assert.AreSame(commandList.Values[0], commands);
        }

        [Test]
        public void AvailableNotifications()
        {
            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var notificationList = new Dictionary<Type, INotificationSet> 
                {
                    { 
                        typeof(IMockNotificationSetWithEventHandler),
                        new Mock<IMockNotificationSetWithEventHandler>().Object
                    },
                    {
                        typeof(IMockNotificationSetWithTypedEventHandler),
                        new Mock<IMockNotificationSetWithTypedEventHandler>().Object
                    },
                    {
                        typeof(IMockNotificationSetForInternalUse),
                        new Mock<IMockNotificationSetForInternalUse>().Object
                    },
                };

            var datasetNotifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(h => h.AvailableNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(notificationList.Keys);
                notificationHub.Setup(h => h.NotificationsFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns<EndpointId, Type>((e, t) => notificationList[t]);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(datasetNotifications.Object);
            }

            var info = new DatasetOnlineInformation(
                id,
                endpoint,
                networkId,
                commandHub.Object,
                notificationHub.Object,
                systemDiagnostics);
            var notifications = info.AvailableNotifications();

            Assert.AreEqual(2, notifications.Count());
        }

        [Test]
        public void Notification()
        {
            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var notificationList = new SortedList<Type, INotificationSet> 
                {
                    { 
                        typeof(IMockNotificationSetWithTypedEventHandler),
                        new Mock<IMockNotificationSetWithTypedEventHandler>().Object
                    },
                };

            var datasetNotifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(
                        h => h.NotificationsFor<IMockNotificationSetWithTypedEventHandler>(It.IsAny<EndpointId>()))
                    .Returns((IMockNotificationSetWithTypedEventHandler)notificationList.Values[0]);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(datasetNotifications.Object);
            }

            var info = new DatasetOnlineInformation(
                id,
                endpoint,
                networkId,
                commandHub.Object,
                notificationHub.Object,
                systemDiagnostics);
            var notifications = info.Notification<IMockNotificationSetWithTypedEventHandler>();
            Assert.AreSame(notificationList.Values[0], notifications);
        }

        [Test]
        public void Close()
        {
            var id = new DatasetId();
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var networkId = NetworkIdentifier.ForLocalMachine();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var datasetCommands = new Mock<IDatasetApplicationCommands>();
            {
                datasetCommands.Setup(d => d.Close())
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationToken(),
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandHub.Setup(h => h.HasCommandFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns(true);
                commandHub.Setup(h => h.CommandsFor<IDatasetApplicationCommands>(It.IsAny<EndpointId>()))
                    .Returns(datasetCommands.Object);
            }

            var notifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(n => n.HasNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(notifications.Object);
            }

            var info = new DatasetOnlineInformation(
                id, 
                endpoint, 
                networkId, 
                commandHub.Object,
                notificationHub.Object,
                systemDiagnostics);
            info.Close();

            datasetCommands.Verify(d => d.Close(), Times.Once());
        }
    }
}
