﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using NUnit.Framework;
using QuickGraph;

namespace Apollo.Core.Host.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectTest
    {
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

        [Test]
        public void Create()
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
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

            Assert.IsNotNull(project);
            Assert.IsNotNull(project.BaseDataset());
        }

        [Test]
        [Ignore("Not implemented yet.")]
        public void CreateFromPersistenceInformation()
        {
        }

        [Test]
        public void SaveWithNullPersistenceInformation()
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
            var project = new Project(
                timeline, 
                distributor,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

            Assert.Throws<ArgumentNullException>(() => project.Save(null));
        }

        [Test]
        public void SaveAfterClosing()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);
            project.Close();

            Assert.Throws<CannotUseProjectAfterClosingItException>(() => project.Save(new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Ignore("Not implemented yet.")]
        public void Save()
        {
        }

        [Test]
        public void ExportWithNullDatasetId()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

            Assert.Throws<ArgumentNullException>(() => project.Export(null, false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        public void ExportWithUnknownDatasetId()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

            Assert.Throws<UnknownDatasetException>(() => project.Export(new DatasetId(), false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        public void ExportWithNullPersistenceInformation()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => project.Export(dataset.Id, false, null));
        }

        [Test]
        public void ExportAfterClosing()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);
            var dataset = project.BaseDataset();
            project.Close();

            Assert.Throws<CannotUseProjectAfterClosingItException>(
                () => project.Export(dataset.Id, false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Ignore("Export is not implemented yet.")]
        public void ExportWithoutChildren()
        {
        }

        [Test]
        [Ignore("Export is not implemented yet.")]
        public void ExportWithChildren()
        {
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

            var name = string.Empty;
            project.OnNameChanged += (s, e) => { name = e.Value; };
            project.Name = "MyNewName";
            Assert.AreEqual(project.Name, name);

            // Set the name again, to the same thing. This 
            // shouldn't notify
            name = string.Empty;
            project.Name = "MyNewName";
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

            var summary = string.Empty;
            project.OnSummaryChanged += (s, e) => { summary = e.Value; };
            project.Summary = "MyNewName";
            Assert.AreEqual(project.Summary, summary);

            // Set the summary again, to the same thing. This 
            // shouldn't notify
            summary = string.Empty;
            project.Summary = "MyNewName";
            Assert.AreEqual(string.Empty, summary);
        }

        [Test]
        public void RollBackWithCreatesOnly()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

            var marks = new List<TimeMarker>();
            marks.Add(TimeMarker.TheBeginOfTime);
            marks.Add(project.History.Mark());

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
            var root = project.BaseDataset();
            datasets.Enqueue(root);

            int count = 0;
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
                    datasets.Enqueue(child);
                    children.Add(child);
                    count++;
                }

                marks.Add(project.History.Mark());
            }

            for (int i = marks.Count - 1; i > -1; i--)
            {
                project.History.RollBackTo(marks[i]);
                if (i > 1)
                {
                    Assert.AreEqual(((i - 1) * 2) + 1, project.NumberOfDatasets);
                }
                else
                {
                    Assert.AreEqual(1, project.NumberOfDatasets);
                }

                for (int j = 0; j < children.Count; j++)
                {
                    if (j < (i - 1) * 2)
                    {
                        Assert.IsTrue(children[j].IsValid);
                    }
                    else
                    {
                        Assert.IsFalse(children[j].IsValid);
                    }
                }
            }
        }

        [Test]
        public void RollForwardWithCreatesOnly()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

            var marks = new List<TimeMarker>();
            marks.Add(TimeMarker.TheBeginOfTime);
            marks.Add(project.History.Mark());

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
            var children = new List<DatasetId>();
            var datasets = new Queue<IProxyDataset>();
            var root = project.BaseDataset();
            datasets.Enqueue(root);

            int count = 0;
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
                    datasets.Enqueue(child);
                    children.Add(child.Id);
                    count++;
                }

                marks.Add(project.History.Mark());
            }

            project.History.RollBackTo(TimeMarker.TheBeginOfTime);
            for (int i = 1; i < marks.Count; i++)
            {
                project.History.RollForwardTo(marks[i]);
                if (i > 1)
                {
                    Assert.AreEqual(((i - 1) * 2) + 1, project.NumberOfDatasets);
                }
                else
                {
                    Assert.AreEqual(i, project.NumberOfDatasets);
                }

                for (int j = 0; j < children.Count; j++)
                {
                    var child = project.Dataset(children[j]);
                    if (j < (i - 1) * 2)
                    {
                        Assert.IsTrue(child.IsValid);
                    }
                    else
                    {
                        Assert.IsNull(child);
                    }
                }
            }
        }

        [Test]
        public void RollBackWithDeletesOnly()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

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
            var children = new List<DatasetId>();
            var datasets = new Queue<IProxyDataset>();
            var root = project.BaseDataset();
            datasets.Enqueue(root);

            int count = 0;
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
                    datasets.Enqueue(child);
                    children.Add(child.Id);
                    count++;
                }
            }

            var marks = new List<TimeMarker>();
            marks.Add(project.History.Mark());
            for (int i = children.Count - 1; i > -1; i--)
            {
                var child = project.Dataset(children[i]);
                child.Delete();
                marks.Add(project.History.Mark());
            }

            for (int i = marks.Count - 1; i > -1; i--)
            {
                project.History.RollBackTo(marks[i]);
                Assert.AreEqual(marks.Count - i, project.NumberOfDatasets);
                for (int j = 0; j < children.Count; j++)
                {
                    var child = project.Dataset(children[j]);
                    if (j < (marks.Count - i - 1))
                    {
                        Assert.IsTrue(child.IsValid);
                    }
                    else
                    {
                        Assert.IsNull(child);
                    }
                }
            }
        }

        [Test]
        public void RollForwardWithDeletesOnly()
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
                    proxyLayer.Object),
                new Mock<ICollectNotifications>().Object,
                systemDiagnostics);

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
            var children = new List<DatasetId>();
            var datasets = new Queue<IProxyDataset>();
            var root = project.BaseDataset();
            datasets.Enqueue(root);

            int count = 0;
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
                    datasets.Enqueue(child);
                    children.Add(child.Id);
                    count++;
                }
            }

            var marks = new List<TimeMarker>();
            marks.Add(project.History.Mark());
            for (int i = children.Count - 1; i > -1; i--)
            {
                var child = project.Dataset(children[i]);
                child.Delete();
                marks.Add(project.History.Mark());
            }

            project.History.RollBackTo(marks[0]);

            for (int i = 1; i < marks.Count; i++)
            {
                project.History.RollForwardTo(marks[i]);
                Assert.AreEqual(children.Count - i + 1, project.NumberOfDatasets);
                for (int j = 0; j < children.Count; j++)
                {
                    var child = project.Dataset(children[j]);
                    if (j < children.Count - i)
                    {
                        Assert.IsTrue(child.IsValid);
                    }
                    else
                    {
                        Assert.IsNull(child);
                    }
                }
            }
        }
    }
}
