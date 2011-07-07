//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class LocalDatasetDistributorTest
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

            var localDistributor = new Mock<ICalculateDistributionParameters>();
            {
                localDistributor.Setup(l => l.ProposeForLocalMachine(It.IsAny<ExpectedDatasetLoad>()))
                    .Returns(result);
            }
            
            var loader = new Mock<IApplicationLoader>();
            var hub = new Mock<ISendCommandsToRemoteEndpoints>();
            var uploads = new WaitingUploads();
            Func<ChannelConnectionInformation> channelInfo =
                () => new ChannelConnectionInformation(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            typeof(NamedPipeChannelType),
                            new Uri("net.pipe://localhost/pipe"));

            var distributor = new LocalDatasetDistributor(
                localDistributor.Object,
                loader.Object,
                hub.Object,
                uploads,
                channelInfo,
                new CurrentThreadTaskScheduler());

            var request = new DatasetRequest 
                {
                    DatasetToLoad = offlineInfo,
                    ExpectedLoadPerMachine = new ExpectedDatasetLoad(),
                    PreferedLocations = LoadingLocations.All,
                };
            var plans = distributor.ProposeDistributionFor(request, new CancellationToken());
            Assert.AreElementsEqualIgnoringOrder(
                new DistributionPlan[] { plan }, 
                plans, 
                (x, y) => 
                {
                    return ReferenceEquals(x.Proposal, y.Proposal);
                });
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
                new DatasetLoadingProposal());

            var localDistributor = new Mock<ICalculateDistributionParameters>();

            var datasetEndpoint = new EndpointId("OtherMachine:5678");
            var loader = new Mock<IApplicationLoader>();
            {
                loader.Setup(l => l.LoadDataset(It.IsAny<ChannelConnectionInformation>()))
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

            var hub = new Mock<ISendCommandsToRemoteEndpoints>();
            {
                hub.Setup(h => h.HasCommandsFor(It.IsAny<EndpointId>()))
                    .Returns(true);
                hub.Setup(h => h.HasCommandFor(It.IsAny<EndpointId>(), It.IsAny<Type>()))
                    .Returns(true);
                hub.Setup(h => h.CommandsFor<IDatasetApplicationCommands>(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(datasetEndpoint, e))
                    .Returns(commands.Object);
            }

            Func<ChannelConnectionInformation> channelInfo =
                () => new ChannelConnectionInformation(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            typeof(NamedPipeChannelType),
                            new Uri("net.pipe://localhost/pipe"));

            var uploads = new WaitingUploads();

            var distributor = new LocalDatasetDistributor(
                localDistributor.Object,
                loader.Object,
                hub.Object,
                uploads,
                channelInfo,
                new CurrentThreadTaskScheduler());
            var result = distributor.ImplementPlan(plan, new CancellationToken());
            result.Wait();

            Assert.AreSame(datasetEndpoint, result.Result.Endpoint);
            Assert.AreSame(plan.DistributionFor.Id, result.Result.Id);
            Assert.AreSame(plan.MachineToDistributeTo, result.Result.RunsOn);
        }
    }
}
