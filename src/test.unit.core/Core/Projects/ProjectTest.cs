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
using System.Threading.Tasks.Schedulers;
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
    public sealed class ProjectTest
    {
        [Test]
        public void Create()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => new Task<DatasetOnlineInformation>(
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
            var project = new Project(distributor);

            Assert.IsNotNull(project);
            Assert.IsNotNull(project.BaseDataset());
        }

        [Test]
        [Ignore("Not implemented yet.")]
        public void CreateFromPersistenceInformation()
        { 
        }

        [Test]
        public void SaveWithullPersistenceInformation()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => new Task<DatasetOnlineInformation>(
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
            var project = new Project(distributor);

            Assert.Throws<ArgumentNullException>(() => project.Save(null));
        }

        [Test]
        public void SaveAfterClosing()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
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
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
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
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
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
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);

            Assert.Throws<ArgumentNullException>(() => project.Export(null, false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        public void ExportWithUnknownDatasetId()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
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
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);

            Assert.Throws<UnknownDatasetException>(() => project.Export(new DatasetId(), false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        public void ExportWithNullPersistenceInformation()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
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
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => project.Export(dataset.Id, false, null));
        }

        [Test]
        public void ExportAfterClosing()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
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
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
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
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
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
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);

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
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        logger),
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
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);

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
    }
}
