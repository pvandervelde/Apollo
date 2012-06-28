//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteDatasetDistributorTest
    {
        private static DistributionPlan CreateNewDistributionPlan(
           DatasetLoadingProposal proposal,
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
            var result = new DatasetLoadingProposal
            {
                Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                IsAvailable = true,
                LoadingTime = new TimeSpan(0, 1, 0),
                TransferTime = new TimeSpan(0, 1, 0),
                PercentageOfAvailableDisk = 50,
                PercentageOfMaximumMemory = 50,
                PercentageOfPhysicalMemory = 50,
            };

            var plan = CreateNewDistributionPlan(
                result,
                offlineInfo,
                systemDiagnostics);

            var loaderCommands = new Mock<IDatasetLoaderCommands>();
            {
                loaderCommands.Setup(l => l.ProposeFor(It.IsAny<ExpectedDatasetLoad>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => result,
                            new CancellationToken(),
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var commandHub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                commandHub.Setup(h => h.CommandsFor<IDatasetLoaderCommands>(It.IsAny<EndpointId>()))
                    .Returns(loaderCommands.Object);
            }

            var notificationHub = new Mock<INotifyOfRemoteEndpointEvents>();

            var offlimitsMachines = new SortedList<string, object> 
                {
                    { "someKeyStuff", "otherMachine" }
                };
            var config = new Mock<IConfiguration>();
            {
                config.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(true);
                config.Setup(c => c.Value<IDictionary<string, object>>(It.IsAny<ConfigurationKey>()))
                    .Returns(offlimitsMachines);
            }

            Func<ChannelConnectionInformation> channelInfo =
                () => new ChannelConnectionInformation(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            typeof(NamedPipeChannelType),
                            new Uri("net.pipe://localhost/pipe"));

            var distributor = new RemoteDatasetDistributor(
                commandHub.Object,
                notificationHub.Object,
                config.Object,
                new WaitingUploads(),
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
                channelInfo,
                systemDiagnostics,
                new CurrentThreadTaskScheduler());

            // Add the remote endpoints
            var forbiddenMachineId = new EndpointId("otherMachine:8080");
            commandHub.Raise(
                h => h.OnEndpointSignedIn += null,
                new CommandSetAvailabilityEventArgs(
                    forbiddenMachineId,
                    new Type[] { typeof(IDatasetLoaderCommands) }));

            var legalMachineId = new EndpointId("myMachine:8080");
            commandHub.Raise(
                h => h.OnEndpointSignedIn += null,
                new CommandSetAvailabilityEventArgs(
                    legalMachineId,
                    new Type[] { typeof(IDatasetLoaderCommands) }));

            var request = new DatasetRequest
            {
                DatasetToLoad = offlineInfo,
                ExpectedLoadPerMachine = new ExpectedDatasetLoad(),
                PreferredLocations = LoadingLocations.All,
            };
            var plans = distributor.ProposeDistributionFor(request, new CancellationToken());
            Assert.AreElementsEqualIgnoringOrder(
                new DistributionPlan[] { plan },
                plans,
                (x, y) =>
                {
                    return ReferenceEquals(x.Proposal, y.Proposal);
                });

            loaderCommands.Verify(l => l.ProposeFor(It.IsAny<ExpectedDatasetLoad>()), Times.Once());
        }

        [Test]
        public void ImplementPlan()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var loaderEndpoint = new EndpointId("myMachine:8080");
            var proposal = new DatasetLoadingProposal
            {
                Endpoint = loaderEndpoint,
                IsAvailable = true,
                LoadingTime = new TimeSpan(0, 1, 0),
                TransferTime = new TimeSpan(0, 1, 0),
                PercentageOfAvailableDisk = 50,
                PercentageOfMaximumMemory = 50,
                PercentageOfPhysicalMemory = 50,
            };

            var filePath = @"c:\temp\myfile.txt";
            var storage = new Mock<IPersistenceInformation>();
            {
                storage.Setup(s => s.AsFile())
                    .Returns(new FileInfo(filePath));
            }

            var plan = CreateNewDistributionPlan(proposal, CreateOfflineInfo(storage.Object), systemDiagnostics);
            var datasetEndpoint = new EndpointId("OtherMachine:5678");
            var loaderCommands = new Mock<IDatasetLoaderCommands>();
            {
                loaderCommands.Setup(l => l.Load(It.IsAny<ChannelConnectionInformation>(), It.IsAny<DatasetId>()))
                    .Returns(
                        Task.Factory.StartNew(
                            () => datasetEndpoint,
                            new CancellationToken(),
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()));
            }

            var applicationCommands = new Mock<IDatasetApplicationCommands>();
            {
                applicationCommands.Setup(c => c.Load(It.IsAny<EndpointId>(), It.IsAny<UploadToken>()))
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
                commandHub.Setup(h => h.CommandsFor<IDatasetLoaderCommands>(It.IsAny<EndpointId>()))
                    .Returns(loaderCommands.Object);
                commandHub.Setup(h => h.CommandsFor<IDatasetApplicationCommands>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(datasetEndpoint, e))
                    .Returns(applicationCommands.Object);
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

            var config = new Mock<IConfiguration>();
            {
                config.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            Func<ChannelConnectionInformation> channelInfo =
                () => new ChannelConnectionInformation(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            typeof(NamedPipeChannelType),
                            new Uri("net.pipe://localhost/pipe"));

            var distributor = new RemoteDatasetDistributor(
                commandHub.Object,
                notificationHub.Object,
                config.Object,
                new WaitingUploads(),
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
                channelInfo,
                systemDiagnostics,
                new CurrentThreadTaskScheduler());

            // Add the remote endpoints
            commandHub.Raise(
                h => h.OnEndpointSignedIn += null,
                new CommandSetAvailabilityEventArgs(
                    loaderEndpoint,
                    new Type[] { typeof(IDatasetLoaderCommands) }));

            Action<int, IProgressMark, TimeSpan> progress = (p, m, t) => { };
            var result = distributor.ImplementPlan(plan, new CancellationToken(), progress);
            result.Wait();

            Assert.AreSame(datasetEndpoint, result.Result.Endpoint);
            Assert.AreSame(plan.DistributionFor.Id, result.Result.Id);
            Assert.AreSame(plan.MachineToDistributeTo, result.Result.RunsOn);
        }
    }
}
