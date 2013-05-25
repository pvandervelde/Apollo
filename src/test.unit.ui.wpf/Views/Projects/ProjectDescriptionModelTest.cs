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
using Apollo.Utilities.History;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Views.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectDescriptionModelTest
    {
        [Test]
        public void OnNameChange()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var project = new Mock<IProject>();
            var facade = new ProjectFacade(project.Object);

            var model = new ProjectDescriptionModel(context.Object, facade);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            var text = "a";
            project.Raise(p => p.OnNameChanged += null, new ValueChangedEventArgs<string>(text));

            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "Name",
                    },
                properties);
        }

        [Test]
        public void OnSummaryChange()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var project = new Mock<IProject>();
            var facade = new ProjectFacade(project.Object);

            var model = new ProjectDescriptionModel(context.Object, facade);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            var text = "a";
            project.Raise(p => p.OnSummaryChanged += null, new ValueChangedEventArgs<string>(text));

            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "Summary",
                    },
                properties);
        }

        [Test]
        public void OnDatasetCreated()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var project = new Mock<IProject>();
            var facade = new ProjectFacade(project.Object);

            var model = new ProjectDescriptionModel(context.Object, facade);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            project.Raise(p => p.OnDatasetCreated += null, EventArgs.Empty);

            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "NumberOfDatasets",
                    },
                properties);
        }

        [Test]
        public void OnDatasetDeleted()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var project = new Mock<IProject>();
            var facade = new ProjectFacade(project.Object);

            var model = new ProjectDescriptionModel(context.Object, facade);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            project.Raise(p => p.OnDatasetDeleted += null, EventArgs.Empty);

            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "NumberOfDatasets",
                    },
                properties);
        }

        [Test]
        public void Name()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

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
                project.SetupProperty(p => p.Name, "a")
                    .Verify();
                project.Setup(p => p.History)
                    .Returns(history.Object);
            }

            var facade = new ProjectFacade(project.Object);

            var model = new ProjectDescriptionModel(context.Object, facade);

            var text = "b";
            model.Name = text;

            project.VerifySet(p => p.Name = text, Times.Once());
            mockChangeSet.Verify(h => h.StoreChanges(), Times.Once());
        }

        [Test]
        public void Summary()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

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
                project.SetupProperty(p => p.Summary, "a")
                    .Verify();
                project.Setup(p => p.History)
                    .Returns(history.Object);
            }

            var facade = new ProjectFacade(project.Object);

            var model = new ProjectDescriptionModel(context.Object, facade);

            var text = "b";
            model.Summary = text;

            project.VerifySet(p => p.Summary = text, Times.Once());
            mockChangeSet.Verify(h => h.StoreChanges(), Times.Once());
        }
    }
}
