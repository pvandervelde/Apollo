﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Utilities;
using Moq;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Apollo.Core.Base.Activation
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class LocalDatasetDistributorTest
    {
        private static DistributionPlan CreateNewDistributionPlan(
            DatasetActivationProposal proposal,
            IDatasetOfflineInformation offlineInfo,
            SystemDiagnostics systemDiagnostics)
        {
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
                    offlineInfo,
                NetworkIdentifier.ForLocalMachine(),
                proposal);
            return plan;
        }

        private static IDatasetOfflineInformation CreateOfflineInfo(IPersistenceInformation storage)
        {
            var mock = new Mock<IDatasetOfflineInformation>();
            {
                mock.Setup(d => d.Id)
                    .Returns(new DatasetId());
                mock.Setup(d => d.CanBeAdopted)
                    .Returns(false);
                mock.Setup(d => d.CanBecomeParent)
                    .Returns(true);
                mock.Setup(d => d.CanBeCopied)
                    .Returns(false);
                mock.Setup(d => d.CanBeDeleted)
                    .Returns(true);
                mock.Setup(d => d.CreatedBy)
                    .Returns(DatasetCreator.User);
                mock.Setup(d => d.StoredAt)
                    .Returns(storage);
            }

            return mock.Object;
        }

        [Test]
        public void ProposeDistributionFor()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offlineInfo = CreateOfflineInfo(new Mock<IPersistenceInformation>().Object);
            var result = new DatasetActivationProposal
                {
                    Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                    IsAvailable = true,
                    ActivationTime = new TimeSpan(0, 1, 0),
                    TransferTime = new TimeSpan(0, 1, 0),
                    PercentageOfAvailableDisk = 50,
                    PercentageOfMaximumMemory = 50,
                    PercentageOfPhysicalMemory = 50,
                };

            var localDistributor = new Mock<ICalculateDistributionParameters>();
            {
                localDistributor.Setup(l => l.ProposeForLocalMachine(It.IsAny<ExpectedDatasetLoad>()))
                    .Returns(result);
            }

            var communicationLayer = new Mock<ICommunicationLayer>();
            {
                communicationLayer.Setup(s => s.LocalConnectionFor(It.Is<ChannelType>(c => c == ChannelType.NamedPipe)))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            new Uri("net.pipe://localhost/pipe"),
                            new Uri("net.pipe://localhost/pipe/data")));
            }

            var loader = new Mock<IDatasetActivator>();
            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            var uploads = new Mock<IStoreUploads>();

            var distributor = new LocalDatasetDistributor(
                localDistributor.Object,
                loader.Object,
                commandHub.Object,
                notificationHub.Object,
                uploads.Object,
                (d, e, n) =>
                {
                    return new DatasetOnlineInformation(
                        d,
                        e,
                        n,
                        commandHub.Object,
                        notificationHub.Object,
                        systemDiagnostics);
                },
                communicationLayer.Object,
                systemDiagnostics,
                new CurrentThreadTaskScheduler());

            var request = new DatasetActivationRequest
                {
                    DatasetToActivate = offlineInfo,
                    ExpectedLoadPerMachine = new ExpectedDatasetLoad(),
                    PreferredLocations = DistributionLocations.All,
                };
            var plans = distributor.ProposeDistributionFor(request, new CancellationToken());
            Assert.AreEqual(1, plans.Count());

            var plan = plans.First();
            Assert.IsTrue(ReferenceEquals(offlineInfo, plan.DistributionFor));
            Assert.AreEqual(new NetworkIdentifier(result.Endpoint.OriginatesOnMachine()), plan.MachineToDistributeTo);
            Assert.IsTrue(ReferenceEquals(result, plan.Proposal));
        }

        [Test]
        public void ImplementPlan()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var filePath = @"c:\temp\myfile.txt";
            var storage = new Mock<IPersistenceInformation>();
            {
                storage.Setup(s => s.AsFile())
                    .Returns(new FileInfo(filePath));
            }

            var offlineInfo = CreateOfflineInfo(storage.Object);
            var plan = CreateNewDistributionPlan(new DatasetActivationProposal(), offlineInfo, systemDiagnostics);
            var localDistributor = new Mock<ICalculateDistributionParameters>();

            var datasetEndpoint = new EndpointId("OtherMachine:5678");
            var loader = new Mock<IDatasetActivator>();
            {
                loader.Setup(l => l.ActivateDataset(It.IsAny<EndpointId>(), It.IsAny<ChannelType>(), It.IsAny<Uri>()))
                    .Returns(datasetEndpoint);
            }

            var commands = new Mock<IDatasetApplicationCommands>();
            {
                commands.Setup(c => c.Load(It.IsAny<EndpointId>(), It.IsAny<UploadToken>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => { },
                            new CancellationToken(),
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandHub.Setup(h => h.HasCommandsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                commandHub.Setup(h => h.HasCommandFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns(true);
                commandHub.Setup(h => h.CommandsFor<IDatasetApplicationCommands>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(datasetEndpoint, e))
                    .Returns(commands.Object);
            }

            var notifications = new Mock<IDatasetApplicationNotifications>();
            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();
            {
                notificationHub.Setup(n => n.HasNotificationsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                notificationHub.Setup(n => n.NotificationsFor<IDatasetApplicationNotifications>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(datasetEndpoint, e))
                    .Returns(notifications.Object);
            }

            var communicationLayer = new Mock<ICommunicationLayer>();
            {
                communicationLayer.Setup(s => s.LocalConnectionFor(It.Is<ChannelType>(c => c == ChannelType.NamedPipe)))
                    .Returns(
                        new Tuple<EndpointId, Uri, Uri>(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            new Uri("net.pipe://localhost/pipe"),
                            new Uri("net.pipe://localhost/pipe/data")));
            }

            var uploads = new Mock<IStoreUploads>();

            var distributor = new LocalDatasetDistributor(
                localDistributor.Object,
                loader.Object,
                commandHub.Object,
                notificationHub.Object,
                uploads.Object,
                (d, e, n) =>
                {
                    return new DatasetOnlineInformation(
                        d,
                        e,
                        n,
                        commandHub.Object,
                        notificationHub.Object,
                        systemDiagnostics);
                },
                communicationLayer.Object,
                systemDiagnostics,
                new CurrentThreadTaskScheduler());

            Action<int, string, bool> progress = (p, m, e) => { };
            var result = distributor.ImplementPlan(plan, new CancellationToken(), progress);
            result.Wait();

            Assert.AreSame(datasetEndpoint, result.Result.Endpoint);
            Assert.AreSame(plan.DistributionFor.Id, result.Result.Id);
            Assert.AreSame(plan.MachineToDistributeTo, result.Result.RunsOn);
        }
    }
}
