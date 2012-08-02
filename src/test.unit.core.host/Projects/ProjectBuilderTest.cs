//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using Apollo.Utilities.History;
using MbUnit.Framework;
using Moq;
using QuickGraph;

namespace Apollo.Core.Host.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectBuilderTest
    {
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
        public void BuildWithoutDistributor()
        {
            ITimeline timeline = new Timeline(BuildStorage);
            var builder = new ProjectBuilder();
            Assert.Throws<CannotCreateProjectWithoutDatasetDistributorException>(() => builder.WithTimeline(timeline).Build());
        }

        [Test]
        public void BuildWithoutTimeline()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var builder = new ProjectBuilder();
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
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            Assert.Throws<CannotCreateProjectWithoutTimelineException>(() => builder.WithDatasetDistributor(distributor).Build());
        }

        [Test]
        public void BuildWithDistributorOnly()
        {
            ITimeline timeline = new Timeline(BuildStorage);
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();
            var builder = new ProjectBuilder();

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
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = builder.Define()
                .WithTimeline(timeline)
                .WithDatasetDistributor(distributor)
                .Build();

            Assert.IsNotNull(project);
        }

        [Test]
        public void BuildWithDistributorAndStorage()
        {
            ITimeline timeline = new Timeline(BuildStorage);
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var offline = new Mock<IDatasetOfflineInformation>();
            var builder = new ProjectBuilder();

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
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var project = builder.Define()
                .WithTimeline(timeline)
                .WithDatasetDistributor(distributor)
                .FromStorage(new Mock<IPersistenceInformation>().Object)
                .Build();

            Assert.IsNotNull(project);
        }
    }
}
