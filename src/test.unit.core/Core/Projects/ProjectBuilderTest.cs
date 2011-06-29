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
    [Description("Tests the ProjectBuilder class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectBuilderTest
    {
        [Test]
        [Description("Checks that it is not possible to build a project without a distributor.")]
        public void BuildWithoutDistributor()
        {
            var builder = new ProjectBuilder();
            Assert.Throws<CannotCreateProjectWithoutDatasetDistributorException>(() => builder.Build());
        }

        [Test]
        [Description("Checks that it is possible to build a project with only a distributor.")]
        public void BuildWithDistributorOnly()
        {
            var builder = new ProjectBuilder();

            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
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
        [Description("Checks that it is not possible to build a project with only a storage object.")]
        public void BuildWithWithStorageOnly()
        {
            var builder = new ProjectBuilder();
            Assert.Throws<CannotCreateProjectWithoutDatasetDistributorException>(
                () => builder.FromStorage(new Mock<IPersistenceInformation>().Object).Build());
        }

        [Test]
        [Description("Checks that it is possible to build a project with a distributor and storage.")]
        public void BuildWithDistributorAndStorage()
        {
            var builder = new ProjectBuilder();

            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
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
