//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetModelTest
    {
        [Test]
        public void Create()
        {
            var context = new Mock<IContextAware>();
            var progressTracker = new Mock<ITrackSteppingProgress>();

            var history = new Mock<ITimeline>();
            {
                history.Setup(h => h.Mark())
                    .Verifiable();
            }

            var project = new Mock<IProject>();
            {
                project.Setup(p => p.History)
                    .Returns(history.Object);
            }

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            var name = "a";
            var summary = "b";
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.Name)
                    .Returns(name);
                proxy.Setup(p => p.Summary)
                    .Returns(summary);
                proxy.Setup(p => p.IsActivated)
                    .Returns(true);
            }

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);

            Assert.AreEqual(name, model.Name);
            Assert.AreEqual(summary, model.Summary);

            Assert.IsTrue(model.IsActivated);
        }

        [Test]
        public void UpdateName()
        {
            var context = new Mock<IContextAware>();
            var progressTracker = new Mock<ITrackSteppingProgress>();
            var mockChangeSet = new Mock<IChangeSet>();
            {
                mockChangeSet.Setup(m => m.StoreChanges())
                    .Verifiable();
            }

            var history = new Mock<ITimeline>();
            {
                history.Setup(h => h.RecordHistory())
                    .Returns(mockChangeSet.Object);
            }

            var project = new Mock<IProject>();
            {
                project.Setup(p => p.History)
                    .Returns(history.Object);
            }

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.Name)
                    .Verifiable();
            }

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);

            var name = "a";
            model.Name = name;
            proxy.VerifySet(p => p.Name = It.Is<string>(s => s.Equals(name)), Times.Once());
            mockChangeSet.Verify(h => h.StoreChanges(), Times.Once());
        }

        [Test]
        public void UpdateSummary()
        {
            var context = new Mock<IContextAware>();
            var progressTracker = new Mock<ITrackSteppingProgress>();
            var mockChangeSet = new Mock<IChangeSet>();
            {
                mockChangeSet.Setup(m => m.StoreChanges())
                    .Verifiable();
            }

            var history = new Mock<ITimeline>();
            {
                history.Setup(h => h.RecordHistory())
                    .Returns(mockChangeSet.Object);
            }

            var project = new Mock<IProject>();
            {
                project.Setup(p => p.History)
                    .Returns(history.Object);
            }

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.Summary)
                    .Verifiable();
            }

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);

            var summary = "a";
            model.Summary = summary;
            proxy.VerifySet(p => p.Summary = It.Is<string>(s => s.Equals(summary)), Times.Once());
            mockChangeSet.Verify(h => h.StoreChanges(), Times.Once());
        }

        [Test]
        public void OnNameChange()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var progressTracker = new Mock<ITrackSteppingProgress>();
            var projectLink = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);

            var wasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                wasRaised++;
                properties.Add(e.PropertyName);
            };

            proxy.Raise(p => p.OnNameChanged += null, new ValueChangedEventArgs<string>("a"));
            Assert.AreEqual(1, wasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "Name",
                    }));
        }

        [Test]
        public void OnSummaryChange()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var progressTracker = new Mock<ITrackSteppingProgress>();
            var projectLink = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);

            var wasRaised = false;
            model.PropertyChanged += (s, e) =>
            {
                wasRaised = true;
                Assert.AreEqual("Summary", e.PropertyName);
            };

            proxy.Raise(p => p.OnSummaryChanged += null, new ValueChangedEventArgs<string>("a"));
            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void OnActivated()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var progressTracker = new Mock<ITrackSteppingProgress>();
            {
                progressTracker.Setup(p => p.StopTracking())
                    .Verifiable();
            }

            var projectLink = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };
            var onLoadedWasRaised = false;
            model.OnActivated += (s, e) =>
            {
                onLoadedWasRaised = true;
            };

            proxy.Raise(p => p.OnActivated += null, EventArgs.Empty);
            Assert.AreEqual(4, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "IsActivated",
                        "RunsOn",
                        "Progress",
                        "ProgressDescription",
                    }));
            Assert.IsTrue(onLoadedWasRaised);
            progressTracker.Verify(p => p.StopTracking(), Times.Once());
        }

        [Test]
        public void OnDeactivated()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var progressTracker = new Mock<ITrackSteppingProgress>();
            var projectLink = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };
            var onUnloadedWasRaised = false;
            model.OnDeactivated += (s, e) =>
            {
                onUnloadedWasRaised = true;
            };

            proxy.Raise(p => p.OnDeactivated += null, EventArgs.Empty);
            Assert.AreEqual(2, propertyChangedWasRaised);
            Assert.That(
                properties,
                Is.EquivalentTo(
                    new List<string>
                    {
                        "IsActivated",
                        "RunsOn",
                    }));
            Assert.IsTrue(onUnloadedWasRaised);
        }

        [Test]
        public void OnProgress()
        {
            var context = new Mock<IContextAware>();
            var progressTracker = new Mock<ITrackSteppingProgress>();
            {
                progressTracker.Setup(p => p.StartTracking())
                    .Verifiable();
                progressTracker.Setup(p => p.StopTracking())
                    .Verifiable();
                progressTracker.Setup(p => p.UpdateProgress(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Verifiable();
            }

            var projectLink = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();

            var dataset = new DatasetFacade(proxy.Object);
            var model = new DatasetModel(context.Object, progressTracker.Object, projectLink.Object, dataset);

            // Verify here to indicate that we do really need this instance.
            Assert.IsNotNull(model);

            proxy.Raise(p => p.OnProgressOfCurrentAction += null, new ProgressEventArgs(0, "a", false));
            progressTracker.Verify(p => p.StartTracking(), Times.Once());
            progressTracker.Verify(p => p.UpdateProgress(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            progressTracker.Verify(p => p.StopTracking(), Times.Never());

            proxy.Raise(p => p.OnProgressOfCurrentAction += null, new ProgressEventArgs(50, "b", false));
            progressTracker.Verify(p => p.StartTracking(), Times.Once());
            progressTracker.Verify(p => p.UpdateProgress(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(2));
            progressTracker.Verify(p => p.StopTracking(), Times.Never());

            proxy.Raise(p => p.OnProgressOfCurrentAction += null, new ProgressEventArgs(100, "c", false));
            progressTracker.Verify(p => p.StartTracking(), Times.Once());
            progressTracker.Verify(p => p.UpdateProgress(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(3));
            progressTracker.Verify(p => p.StopTracking(), Times.Once());
        }
    }
}
