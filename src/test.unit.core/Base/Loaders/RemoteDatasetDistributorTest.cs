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
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteDatasetDistributorTest
    {
        [Test]
        public void ProposeDistributionFor()
        {
            var offlineInfo = new DatasetOfflineInformation(
                new DatasetId(),
                new DatasetCreationInformation()
                {
                    CreatedOnRequestOf = DatasetCreator.User,
                    CanBecomeParent = true,
                    CanBeAdopted = false,
                    CanBeCopied = false,
                    CanBeDeleted = true,
                    LoadFrom = new Mock<IPersistenceInformation>().Object,
                });
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
            var plan = new DistributionPlan(
                (p, t) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                offlineInfo,
                NetworkIdentifier.ForLocalMachine(),
                result);

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

            var hub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                hub.Setup(h => h.CommandsFor<IDatasetLoaderCommands>(It.IsAny<EndpointId>()))
                    .Returns(loaderCommands.Object);
            }

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
                hub.Object,
                config.Object,
                new WaitingUploads(),
                channelInfo,
                new CurrentThreadTaskScheduler());

            // Add the remote endpoints
            var forbiddenMachineId = new EndpointId("otherMachine:8080");
            hub.Raise(
                h => h.OnEndpointSignedIn += null,
                new CommandSetAvailabilityEventArgs(
                    forbiddenMachineId,
                    new Type[] { typeof(IDatasetLoaderCommands) }));

            var legalMachineId = new EndpointId("myMachine:8080");
            hub.Raise(
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
            var filePath = @"c:\temp\myfile.txt";
            var storage = new Mock<IPersistenceInformation>();
            {
                storage.Setup(s => s.AsFile())
                    .Returns(new FileInfo(filePath));
            }

            var loaderEndpoint = new EndpointId("myMachine:8080");
            var plan = new DistributionPlan(
                (p, t) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = storage.Object,
                    }),
                NetworkIdentifier.ForLocalMachine(),
                new DatasetLoadingProposal 
                    {
                        Endpoint = loaderEndpoint,
                    });

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

            var hub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                hub.Setup(h => h.HasCommandsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                hub.Setup(h => h.CommandsFor<IDatasetLoaderCommands>(It.IsAny<EndpointId>()))
                    .Returns(loaderCommands.Object);
                hub.Setup(h => h.CommandsFor<IDatasetApplicationCommands>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(datasetEndpoint, e))
                    .Returns(applicationCommands.Object);
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
                hub.Object,
                config.Object,
                new WaitingUploads(),
                channelInfo,
                new CurrentThreadTaskScheduler());

            // Add the remote endpoints
            hub.Raise(
                h => h.OnEndpointSignedIn += null,
                new CommandSetAvailabilityEventArgs(
                    loaderEndpoint,
                    new Type[] { typeof(IDatasetLoaderCommands) }));

            var result = distributor.ImplementPlan(plan, new CancellationToken());
            result.Wait();

            Assert.AreSame(datasetEndpoint, result.Result.Endpoint);
            Assert.AreSame(plan.DistributionFor.Id, result.Result.Id);
            Assert.AreSame(plan.MachineToDistributeTo, result.Result.RunsOn);
        }
    }
}
