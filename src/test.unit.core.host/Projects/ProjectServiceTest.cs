//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Host.Plugins;
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
    public sealed class ProjectServiceTest
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

            throw new UnknownHistoryMemberTypeException();
        }

        [Test]
        public void Stop()
        {
            ITimeline timeline = new Timeline(BuildStorage);
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var service = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                distributor.Object,
                builder.Object);

            service.Start();
            Assert.AreEqual(StartupState.Started, service.StartupState);

            service.Stop();
            Assert.AreEqual(StartupState.Stopped, service.StartupState);
        }

        [Test]
        public void StopWithProject()
        {
            ITimeline timeline = new Timeline(BuildStorage);
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

                builder.Setup(b => b.WithTimeline(It.IsAny<ITimeline>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDataStorageBuilder(It.IsAny<Func<DatasetOnlineInformation, DatasetStorageProxy>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var service = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
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
        public void CreateNewProject()
        {
            ITimeline timeline = new Timeline(BuildStorage);
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithTimeline(It.IsAny<ITimeline>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDataStorageBuilder(It.IsAny<Func<DatasetOnlineInformation, DatasetStorageProxy>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var service = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                distributor.Object,
                builder.Object);

            service.CreateNewProject();

            builder.Verify(b => b.Define(), Times.Exactly(1));
            builder.Verify(
                b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>>>()), 
                Times.Exactly(1));
            builder.Verify(b => b.Build(), Times.Exactly(1));
        }

        [Test]
        public void LoadProjectWithNullPersistenceInformation()
        {
            ITimeline timeline = new Timeline(BuildStorage);
            var distributor = new Mock<IHelpDistributingDatasets>();
            var builder = new Mock<IBuildProjects>();

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var service = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                distributor.Object,
                builder.Object);

            Assert.Throws<ArgumentNullException>(() => service.LoadProject(null));
        }

        [Test]
        public void LoadProject()
        {
            ITimeline timeline = new Timeline(BuildStorage);
            var distributor = new Mock<IHelpDistributingDatasets>();

            var project = new Mock<IProject>();
            var closable = project.As<ICanClose>();
            var builder = new Mock<IBuildProjects>();
            {
                builder.Setup(b => b.Define())
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithTimeline(It.IsAny<ITimeline>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.FromStorage(It.IsAny<IPersistenceInformation>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDataStorageBuilder(It.IsAny<Func<DatasetOnlineInformation, DatasetStorageProxy>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object)
                    .Verifiable();
            }

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var service = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                distributor.Object,
                builder.Object);

            service.LoadProject(new Mock<IPersistenceInformation>().Object);

            builder.Verify(b => b.Define(), Times.Exactly(1));
            builder.Verify(
                b => b.WithDatasetDistributor(
                    It.IsAny<Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>>>()), 
                    Times.Exactly(1));
            builder.Verify(b => b.FromStorage(It.IsAny<IPersistenceInformation>()), Times.Exactly(1));
            builder.Verify(b => b.Build(), Times.Exactly(1));
        }

        [Test]
        public void UnloadProject()
        {
            ITimeline timeline = new Timeline(BuildStorage);
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

                builder.Setup(b => b.WithTimeline(It.IsAny<ITimeline>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.WithDatasetDistributor(It.IsAny<Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>>>()))
                    .Returns(builder.Object);

                builder.Setup(b => b.WithDataStorageBuilder(It.IsAny<Func<DatasetOnlineInformation, DatasetStorageProxy>>()))
                    .Returns(builder.Object)
                    .Verifiable();

                builder.Setup(b => b.Build())
                    .Returns(project.Object);
            }

            var proxyLayer = new Mock<IProxyCompositionLayer>();
            var service = new ProjectService(
                () => timeline,
                d => new DatasetStorageProxy(
                    d,
                    new GroupSelector(
                        new Mock<IConnectGroups>().Object,
                        proxyLayer.Object),
                    proxyLayer.Object),
                distributor.Object,
                builder.Object);

            service.CreateNewProject();
            service.UnloadProject();

            closable.Verify(p => p.Close(), Times.Exactly(1));
        }
    }
}
