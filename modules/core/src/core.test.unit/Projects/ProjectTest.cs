//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Projects;
using Apollo.Utils;
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
        [Description("Checks that an object cannot be created with a null distributor function.")]
        public void CreateWithNullDistributor()
        {
            Assert.Throws<ArgumentNullException>(() => new Project(null));
        }

        [Test]
        [Description("Checks that an object can be created with only a distribution function.")]
        public void Create()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
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
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            Assert.Throws<ArgumentNullException>(() => project.Save(null));
        }

        [Test]
        [Description("Checks that a project can be closed.")]
        public void SaveAfterClosing()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
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
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            Assert.Throws<ArgumentNullException>(() => project.Export(null, false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Description("Checks that a dataset cannot be persisted with an unknown dataset ID.")]
        public void ExportWithUnknownDatasetId()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            Assert.Throws<UnknownDatasetException>(() => project.Export(new DatasetId(), false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Description("Checks that a dataset cannot be persisted without a persistence storage.")]
        public void ExportWithNullPersistenceInformation()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => project.Export(dataset.Id, false, null));
        }

        [Test]
        [Description("Checks that a project can be closed.")]
        public void ExportAfterClosing()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();
            project.Close();

            Assert.Throws<CannotUseProjectAfterClosingItException>(() => project.Export(dataset.Id, false, new Mock<IPersistenceInformation>().Object));
        }

        [Test]
        [Description("Checks that a dataset can be persisted.")]
        [Ignore("Not implemented yet.")]
        public void ExportWithoutChildren()
        { 
        }

        [Test]
        [Description("Checks that a dataset and its children can be persisted.")]
        [Ignore("Not implemented yet.")]
        public void ExportWithChildren()
        { 
        }

        [Test]
        [Description("Checks that when the name of a project is set a notification is send out.")]
        public void Name()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            var observer = new Mock<INotifyOnProjectChanges>();
            {
                observer.Setup(o => o.NameUpdated())
                    .Verifiable();
            }

            project.RegisterForEvents(observer.Object);
            project.Name = "MyNewName";

            // Set the name again, to the same thing. This 
            // shouldn't notify
            project.Name = "MyNewName";
            observer.Verify(o => o.NameUpdated(), Times.Once());
        }

        [Test]
        [Description("Checks that when the summary of a project is set a notification is send out.")]
        public void Summary()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            var observer = new Mock<INotifyOnProjectChanges>();
            {
                observer.Setup(o => o.SummaryUpdated())
                    .Verifiable();
            }

            project.RegisterForEvents(observer.Object);
            project.Summary = "MyNewName";

            // Set the summary again, to the same thing. This 
            // shouldn't notify
            project.Summary = "MyNewName";
            observer.Verify(o => o.SummaryUpdated(), Times.Once());
        }

        [Test]
        [Description("Checks that an exception is thrown when registering an observer after the project is closed.")]
        public void RegisterForEventsWhenClosed()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            project.Close();

            var observer = new Mock<INotifyOnProjectChanges>();
            Assert.Throws<ArgumentException>(() => project.RegisterForEvents(observer.Object));
        }

        [Test]
        [Description("Checks that an exception is thrown when registering a null observer.")]
        public void RegisterForEventsWithNullObserver()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            Assert.Throws<ArgumentNullException>(() => project.RegisterForEvents(null));
        }

        [Test]
        [Description("Checks that an observer cannot be registered twice.")]
        public void RegisterForEventsTwice()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            var observer = new Mock<INotifyOnProjectChanges>();
            {
                observer.Setup(o => o.NameUpdated())
                    .Verifiable();
            }

            project.RegisterForEvents(observer.Object);
            project.RegisterForEvents(observer.Object);

            project.Name = "MyNewName";
            observer.Verify(o => o.NameUpdated(), Times.Once());
        }

        [Test]
        [Description("Checks that an exception is thrown when unregistering an observer after the project is closed.")]
        public void UnregisterFromEventsWhenClosed()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            project.Close();

            var observer = new Mock<INotifyOnProjectChanges>();
            Assert.Throws<ArgumentException>(() => project.UnregisterFromEvents(observer.Object));
        }

        [Test]
        [Description("Checks that an exception is thrown when unregistering a null observer.")]
        public void UnregisterFromEventsWithNullObserver()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            Assert.Throws<ArgumentNullException>(() => project.UnregisterFromEvents(null));
        }

        [Test]
        [Description("Checks that an observer can be unregistered.")]
        public void UnregisterFromEvents()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);

            var observer = new Mock<INotifyOnProjectChanges>();
            {
                observer.Setup(o => o.NameUpdated())
                    .Verifiable();
            }

            project.RegisterForEvents(observer.Object);

            project.Name = "MyNewName";
            observer.Verify(o => o.NameUpdated(), Times.Once());

            project.UnregisterFromEvents(observer.Object);

            project.Name = "MyOtherNewName";
            observer.Verify(o => o.NameUpdated(), Times.Once());
        }
    }
}
