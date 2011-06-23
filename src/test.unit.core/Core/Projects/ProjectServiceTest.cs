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
using Apollo.Core.Utilities.Licensing;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the FusionHelper class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectServiceTest
    {
        [Test]
        [Description("Checks that the service can stop successfully.")]
        public void Stop()
        {
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                storage,
                distributor.Object,
                builder.Object);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);

            service.Stop();
            Assert.AreEqual(StartupState.Stopped, service.StartupState);
        }

        [Test]
        [Description("Checks that the service can stop successfully.")]
        public void StopWithProject()
        {
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            bool wasClosed = false;
            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            {
                closable.Setup(c => c.Close())
                    .Callback(() => { wasClosed = true; });
            }

            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var service = new ProjectService(
                storage,
                distributor.Object,
                builder.Object);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);

            service.CreateNewProject();

            var createdProject = service.Current;
            Assert.IsNotNull(createdProject);

            service.Stop();
            Assert.AreEqual(StartupState.Stopped, service.StartupState);
            Assert.IsTrue(wasClosed);
        }

        [Test]
        [Description("Checks that a new project can be created.")]
        public void CreateNewProject()
        {
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var service = new ProjectService(
                storage,
                distributor.Object,
                builder.Object);

            service.CreateNewProject();

            builder.Verify(b => b.Define(), Times.Exactly(1));
            builder.Verify(
                b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>>>()), 
                Times.Exactly(1));
            builder.Verify(b => b.Build(), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that a new project cannot be loaded without a null persistence object.")]
        public void LoadProjectWithNullPersistenceInformation()
        {
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var service = new ProjectService(
                storage,
                distributor.Object,
                builder.Object);

            Assert.Throws<ArgumentNullException>(() => service.LoadProject(null));
        }

        [Test]
        [Description("Checks that a new project can be loaded.")]
        public void LoadProject()
        {
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.FromStorage(It.IsAny<IPersistenceInformation>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var service = new ProjectService(
                storage,
                distributor.Object,
                builder.Object);

            service.LoadProject(new Mock<IPersistenceInformation>().Object);

            builder.Verify(b => b.Define(), Times.Exactly(1));
            builder.Verify(
                b => b.WithDatasetDistributor(
                    It.IsAny<Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>>>()), 
                    Times.Exactly(1));
            builder.Verify(b => b.FromStorage(It.IsAny<IPersistenceInformation>()), Times.Exactly(1));
            builder.Verify(b => b.Build(), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that a project can be unloaded.")]
        public void UnloadProject()
        {
            var storage = new LicenseValidationResultStorage();
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            {
                closable.Setup(p => p.Close())
                    .Verifiable();
            }

            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object);

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, Task<IEnumerable<DistributionPlan>>>>()))
                    .Returns(builder.Object);

                builder.Setup(b => b.Build())
                    .Returns(project.Object);
            }

            var service = new ProjectService(
                storage,
                distributor.Object,
                builder.Object);

            service.CreateNewProject();
            service.UnloadProject();

            closable.Verify(p => p.Close(), Times.Exactly(1));
        }
    }
}
