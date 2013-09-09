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
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Core.Host.Plugins;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Moq;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;
using QuickGraph;

namespace Apollo.Core.Host.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetProxyTest : EqualityContractVerifierTest
    {
        private sealed class DatasetProxyEqualityContractVerifier : EqualityContractVerifier<DatasetProxy>
        {
            private readonly DatasetProxy m_First = GenerateDataset(CreateProject());

            private readonly DatasetProxy m_Second = GenerateDataset(CreateProject());

            protected override DatasetProxy Copy(DatasetProxy original)
            {
                var proxyLayer = new Mock<IProxyCompositionLayer>();
                Func<DatasetOnlineInformation, DatasetStorageProxy> func = d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object);
                return DatasetProxy.CreateInstance(
                    new HistoryId(), 
                    new List<Tuple<byte, IStoreTimelineValues>>
                        {
                            new Tuple<byte, IStoreTimelineValues>(0, new ValueHistory<string>()),
                            new Tuple<byte, IStoreTimelineValues>(1, new ValueHistory<string>()),
                            new Tuple<byte, IStoreTimelineValues>(2, new ValueHistory<NetworkIdentifier>())
                        },
                    new DatasetConstructionParameters
                        {
                            CanBeAdopted = false,
                            CanBecomeParent = false,
                            CanBeCopied = true,
                            CanBeDeleted = true,
                            CreatedOnRequestOf = DatasetCreator.User,
                            DistributionPlanGenerator = null,
                            Id = original.Id,
                            IsRoot = true,
                            LoadFrom = null,
                        }, 
                    func);
            }

            protected override DatasetProxy FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override DatasetProxy SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class DatasetProxyHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<DatasetProxy> m_DistinctInstances
                = new List<DatasetProxy> 
                     {
                        GenerateDataset(CreateProject()),
                        GenerateDataset(CreateProject()),
                        GenerateDataset(CreateProject()),
                        GenerateDataset(CreateProject()),
                        GenerateDataset(CreateProject()),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private static IStoreTimelineValues BuildStorage(ITimeline timeline, Type type)
        {
            if (typeof(IDictionaryTimelineStorage<DatasetId, DatasetProxy>).IsAssignableFrom(type))
            {
                return new HistoryObjectDictionaryHistory<DatasetId, DatasetProxy>(id => timeline.IdToObject<DatasetProxy>(id));
            }

            if (typeof(IBidirectionalGraphHistory<DatasetId, Edge<DatasetId>>).IsAssignableFrom(type))
            {
                return new BidirectionalGraphHistory<DatasetId, Edge<DatasetId>>();
            }

            if (typeof(IVariableTimeline<string>).IsAssignableFrom(type))
            {
                return new ValueHistory<string>();
            }

            if (typeof(IVariableTimeline<NetworkIdentifier>).IsAssignableFrom(type))
            {
                return new ValueHistory<NetworkIdentifier>();
            }

            throw new UnknownHistoryMemberTypeException();
        }

        private static IProject CreateProject()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            return new Project(
                timeline,
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
        }

        private static DatasetProxy GenerateDataset(IProject project)
        {
            return (DatasetProxy)project.BaseDataset();
        }

        private readonly DatasetProxyHashcodeContractVerfier m_HashcodeVerifier = new DatasetProxyHashcodeContractVerfier();

        private readonly DatasetProxyEqualityContractVerifier m_EqualityVerifier = new DatasetProxyEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }

        [Test]
        public void GetDataset()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            Assert.IsNotNull(dataset);
            Assert.IsNotNull(dataset.Id);
        }

        [Test]
        public void Name()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            var name = string.Empty;
            dataset.OnNameChanged += (s, e) => { name = e.Value; };
            dataset.Name = "MyNewName";
            Assert.AreEqual(dataset.Name, name);

            // Set the name again, to the same thing. This 
            // shouldn't notify
            name = string.Empty;
            dataset.Name = "MyNewName";
            Assert.AreEqual(string.Empty, name);
        }

        [Test]
        public void Summary()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            var summary = string.Empty;
            dataset.OnSummaryChanged += (s, e) => { summary = e.Value; };
            dataset.Summary = "MyNewName";
            Assert.AreEqual(dataset.Summary, summary);

            // Set the summary again, to the same thing. This 
            // shouldn't notify
            summary = string.Empty;
            dataset.Summary = "MyNewName";
            Assert.AreEqual(string.Empty, summary);
        }

        [Test]
        public void LoadOntoMachineWithIllegalLoadingLocation()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal(plan);

            Assert.Throws<CannotLoadDatasetWithoutLoadingLocationException>(
                () => dataset.LoadOntoMachine(
                    DistributionLocations.None,
                    selector,
                    new CancellationToken()));
        }

        [Test]
        public void LoadOntoMachineWhenAlreadyLoaded()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var endpoint = new EndpointId("id");
            var notifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(n => n.HasNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(notifications.Object);
            }

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        endpoint,
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        notificationHub.Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());

            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal(plan);

            dataset.LoadOntoMachine(DistributionLocations.All, selector, new CancellationToken());

            Assert.IsTrue(dataset.IsLoaded);

            bool wasLoaded = false;
            dataset.OnLoaded += (s, e) => wasLoaded = true;
            dataset.LoadOntoMachine(DistributionLocations.All, selector, new CancellationToken());

            Assert.IsFalse(wasLoaded);
            Assert.IsTrue(dataset.IsLoaded);
        }

        [Test]
        public void LoadOntoMachineWithSelectionCancellation()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());

            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            // Explicitly return nothing so that we cancel the process
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal();

            bool wasLoaded = false;
            dataset.OnLoaded += (s, e) => wasLoaded = true;

            var source = new CancellationTokenSource();
            dataset.LoadOntoMachine(DistributionLocations.All, selector, source.Token);

            Assert.IsFalse(wasLoaded);
        }

        [Test]
        public void LoadOntoMachine()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var endpoint = new EndpointId("id");
            var notifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(n => n.HasNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(notifications.Object);
            }

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () =>
                    {
                        r(100, "a");
                        return new DatasetOnlineInformation(
                            new DatasetId(),
                            endpoint,
                            new NetworkIdentifier("machine"),
                            new Mock<ISendCommandsToRemoteEndpoints>().Object,
                            notificationHub.Object,
                            systemDiagnostics);
                    },
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());

            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal(plan);

            bool wasLoaded = false;
            dataset.OnLoaded += (s, e) => wasLoaded = true;

            int progress = -1;
            dataset.OnProgressOfCurrentAction += (s, e) => progress = e.Progress;
            dataset.LoadOntoMachine(DistributionLocations.All, selector, new CancellationToken());

            Assert.IsTrue(dataset.IsLoaded);
            Assert.IsTrue(wasLoaded);
            Assert.AreEqual(100, progress);
        }

        [Test]
        public void UnloadFromMachine()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var commands = new Mock<IDatasetApplicationCommands>();
            {
                commands.Setup(c => c.Close())
                    .Returns(Task.Factory.StartNew(() => { }));
            }

            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandHub.Setup(h => h.CommandsFor<IDatasetApplicationCommands>(It.IsAny<EndpointId>()))
                    .Returns(commands.Object);
            }

            var endpoint = new EndpointId("id");
            var notifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(n => n.HasNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(notifications.Object);
            }

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        endpoint,
                        new NetworkIdentifier("machine"),
                        commandHub.Object,
                        notificationHub.Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());

            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal(plan);

            dataset.LoadOntoMachine(DistributionLocations.All, selector, new CancellationToken());

            Assert.IsTrue(dataset.IsLoaded);

            bool wasUnloaded = false;
            dataset.OnUnloaded += (s, e) => wasUnloaded = true;
            dataset.UnloadFromMachine();

            Assert.IsFalse(dataset.IsLoaded);
            Assert.IsTrue(wasUnloaded);
        }

        [Test]
        public void Children()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = false,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var child = dataset.CreateNewChild(creationInformation);
            var children = dataset.Children();
            Assert.AreEqual(1, children.Count());
            Assert.AreEqual(child.Id, children.First().Id);
        }

        [Test]
        public void CreateNewChildWithNullCreationInformation()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => dataset.CreateNewChild(null));
        }

        [Test]
        public void CreateNewChildWhenDatasetCannotBeParent()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
                {
                    CreatedOnRequestOf = DatasetCreator.User,
                    CanBecomeParent = false,
                    LoadFrom = new Mock<IPersistenceInformation>().Object,
                };

            var child = dataset.CreateNewChild(creationInformation);
            Assert.Throws<DatasetCannotBecomeParentException>(() => child.CreateNewChild(creationInformation));
        }

        [Test]
        public void CreateNewChild()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            bool wasInvoked = false;
            project.OnDatasetCreated += (s, e) => { wasInvoked = true; };

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = false,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var child = dataset.CreateNewChild(creationInformation);
            Assert.AreNotEqual(dataset.Id, child.Id);
            Assert.AreEqual(creationInformation.CreatedOnRequestOf, child.CreatedBy);
            Assert.AreEqual(creationInformation.LoadFrom, child.StoredAt);
            Assert.IsFalse(child.IsLoaded);
            Assert.IsFalse(child.CanBecomeParent);
            Assert.IsFalse(child.CanBeAdopted);
            Assert.IsFalse(child.CanBeCopied);
            Assert.IsFalse(child.CanBeDeleted);
            
            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void CreateNewChildrenWithNullCollection()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => dataset.CreateNewChildren(null));
        }

        [Test]
        public void CreateNewChildrenWithEmptyCollection()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentException>(() => dataset.CreateNewChildren(new List<DatasetCreationInformation>()));
        }

        [Test]
        public void CreateNewChildren()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var dataset = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = false,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var children = dataset.CreateNewChildren(new List<DatasetCreationInformation> { creationInformation });
            foreach (var child in children)
            {
                Assert.AreNotEqual(dataset.Id, child.Id);
                Assert.IsFalse(child.CanBecomeParent);
                Assert.IsFalse(child.CanBeAdopted);
                Assert.IsFalse(child.CanBeCopied);
                Assert.IsFalse(child.CanBeDeleted);
            }
        }

        [Test]
        public void DeleteWhenClosed()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var root = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = true,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var dataset = root.CreateNewChild(creationInformation);
            project.Close();

            Assert.Throws<ArgumentException>(() => dataset.Delete());
        }

        [Test]
        public void DeleteUndeletableDataset()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var root = project.BaseDataset();

            Assert.Throws<CannotDeleteDatasetException>(() => root.Delete());
        }

        [Test]
        public void DeleteDatasetWithUndeletableChild()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var root = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = true,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = true,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var child1 = root.CreateNewChild(creationInformation);

            creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = false,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var child2 = child1.CreateNewChild(creationInformation);

            Assert.Throws<CannotDeleteDatasetException>(() => child1.Delete());
        }

        [Test]
        public void DeleteDatasetWithChildren()
        {
            ITimeline timeline = null;
            timeline = new Timeline(t => BuildStorage(timeline, t));
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offline.Object,
                new NetworkIdentifier("mymachine"),
                new DatasetActivationProposal());
            Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object));
            var root = project.BaseDataset();

            bool projectWasNotified = false;
            project.OnDatasetDeleted += (s, e) => { projectWasNotified = true; };

            // Create a 'binary' tree of datasets. This should create the following tree:
            //                            X
            //                          /   \
            //                         /     \
            //                        /       \
            //                       /         \
            //                      /           \
            //                     X             X
            //                   /   \         /   \
            //                  /     \       /     \
            //                 /       \     /       \
            //                X         X   X         X
            //              /   \     /   \
            //             X     X   X     X
            var children = new List<IProxyDataset>();
            var datasets = new Queue<IProxyDataset>();
            datasets.Enqueue(root);

            int count = 0;
            int deleteCount = 0;
            while (count < 10)
            {
                var creationInformation = new DatasetCreationInformation()
                {
                    CreatedOnRequestOf = DatasetCreator.User,
                    CanBecomeParent = true,
                    CanBeAdopted = false,
                    CanBeCopied = false,
                    CanBeDeleted = true,
                    LoadFrom = new Mock<IPersistenceInformation>().Object,
                };

                var parent = datasets.Dequeue();
                var newChildren = parent.CreateNewChildren(new DatasetCreationInformation[] { creationInformation, creationInformation });
                foreach (var child in newChildren)
                {
                    child.OnDeleted += (s, e) => { deleteCount++; };

                    datasets.Enqueue(child);
                    children.Add(child);
                    count++;
                }
            }

            // We assume that the children are created in an ordered manner so just delete the first child
            children[0].Delete();

            Assert.AreEqual(4, project.NumberOfDatasets);
            Assert.IsFalse(children[0].IsValid);
            Assert.IsTrue(children[1].IsValid);
            Assert.IsFalse(children[2].IsValid);
            Assert.IsFalse(children[3].IsValid);
            Assert.IsTrue(children[4].IsValid);
            Assert.IsTrue(children[5].IsValid);
            Assert.IsFalse(children[6].IsValid);
            Assert.IsFalse(children[7].IsValid);
            Assert.IsFalse(children[8].IsValid);
            Assert.IsFalse(children[9].IsValid);

            Assert.IsTrue(projectWasNotified);
            Assert.AreEqual(7, deleteCount);
        }
    }
}
