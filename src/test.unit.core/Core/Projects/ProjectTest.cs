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
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the Project class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectTest
    {
        [Test]
        [Description("Checks that an object can be created with only a distribution function.")]
        public void Create()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor = 
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
            var project = new Project(distributor);

            Assert.IsNotNull(project);
            Assert.IsNotNull(project.BaseDataset());
        }

        [Test]
        [Description("Checks that an object can be created from a persisted state.")]
        [Ignore("Not implemented yet.")]
        public void CreateFromPersistenceInformation()
        { 
        }

        [Test]
        [Description("Checks that a project cannot be persisted without a persistence storage.")]
        public void SaveWithullPersistenceInformation()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor =
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
            var project = new Project(distributor);

            Assert.Throws<ArgumentNullException>(() => project.Save(null));
        }

        [Test]
        [Description("Checks that a project can be closed.")]
        public void SaveAfterClosing()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor =
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
            var project = new Project(distributor);
            project.Close();

            Assert.Throws<CannotUseProjectAfterClosingItException>(() => project.Save(new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Description("Checks that an object can be saved to a persisted state.")]
        [Ignore("Not implemented yet.")]
        public void Save()
        { 
        }

        [Test]
        [Description("Checks that a dataset cannot be persisted with a null ID reference.")]
        public void ExportWithNullDatasetId()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor =
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
            var project = new Project(distributor);

            Assert.Throws<ArgumentNullException>(() => project.Export(null, false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Description("Checks that a dataset cannot be persisted with an unknown dataset ID.")]
        public void ExportWithUnknownDatasetId()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor =
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
            var project = new Project(distributor);

            Assert.Throws<UnknownDatasetException>(() => project.Export(new DatasetId(), false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Description("Checks that a dataset cannot be persisted without a persistence storage.")]
        public void ExportWithNullPersistenceInformation()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor =
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => project.Export(dataset.Id, false, null));
        }

        [Test]
        [Description("Checks that a project can be closed.")]
        public void ExportAfterClosing()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor =
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
            var project = new Project(distributor);
            var dataset = project.BaseDataset();
            project.Close();

            Assert.Throws<CannotUseProjectAfterClosingItException>(
                () => project.Export(dataset.Id, false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Description("Checks that a dataset can be persisted.")]
        [Ignore("Export is not implemented yet.")]
        public void ExportWithoutChildren()
        { 
        }

        [Test]
        [Description("Checks that a dataset and its children can be persisted.")]
        [Ignore("Export is not implemented yet.")]
        public void ExportWithChildren()
        { 
        }

        [Test]
        [Description("Checks that when the name of a project is set a notification is send out.")]
        public void Name()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor =
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
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
        [Description("Checks that when the summary of a project is set a notification is send out.")]
        public void Summary()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(() => new DatasetOnlineInformation(), t),
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
            Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>> distributor =
                (r, c) => new Task<IEnumerable<DistributionPlan>>(() => new List<DistributionPlan> { plan });
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
