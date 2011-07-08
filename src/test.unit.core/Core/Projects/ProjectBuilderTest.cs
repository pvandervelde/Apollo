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
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectBuilderTest
    {
        [Test]
        public void BuildWithoutDistributor()
        {
            var builder = new ProjectBuilder();
            Assert.Throws<CannotCreateProjectWithoutDatasetDistributorException>(() => builder.Build());
        }

        [Test]
        public void BuildWithDistributorOnly()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var builder = new ProjectBuilder();

            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = builder.Define()
                .WithDatasetDistributor(distributor)
                .Build();

            Assert.IsNotNull(project);
        }

        [Test]
        public void BuildWithWithStorageOnly()
        {
            var builder = new ProjectBuilder();
            Assert.Throws<CannotCreateProjectWithoutDatasetDistributorException>(
                () => builder.FromStorage(new Mock<IPersistenceInformation>().Object).Build());
        }

        [Test]
        public void BuildWithDistributorAndStorage()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };
            var builder = new ProjectBuilder();

            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var project = builder.Define()
                .WithDatasetDistributor(distributor)
                .FromStorage(new Mock<IPersistenceInformation>().Object)
                .Build();

            Assert.IsNotNull(project);
        }
    }
}
