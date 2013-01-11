//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Host.Scripting.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectFacadeForScriptsTest
    {
        [Test]
        public void Name()
        {
            var project = new Mock<IProject>();
            {
                project.SetupProperty(p => p.Name);
            }

            var facade = new ProjectFacadeForScripts(new ProjectFacade(project.Object));

            var name = "name";
            facade.Name = name;
            Assert.AreEqual(name, facade.Name);
        }

        [Test]
        public void OnNameUpdate()
        {
            var project = new Mock<IProject>();
            var facade = new ProjectFacadeForScripts(new ProjectFacade(project.Object));

            bool eventRaised = false;
            facade.OnNameChanged += (s, e) => { eventRaised = true; };

            project.Raise(p => p.OnNameChanged += null, new ValueChangedEventArgs<string>("newName"));
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void Summary()
        {
            var project = new Mock<IProject>();
            {
                project.SetupProperty(p => p.Summary);
            }

            var facade = new ProjectFacadeForScripts(new ProjectFacade(project.Object));

            var summary = "text";
            facade.Summary = summary;
            Assert.AreEqual(summary, facade.Summary);
        }

        [Test]
        public void OnSummaryUpdate()
        {
            var project = new Mock<IProject>();
            var facade = new ProjectFacadeForScripts(new ProjectFacade(project.Object));

            bool eventRaised = false;
            facade.OnSummaryChanged += (s, e) => { eventRaised = true; };

            project.Raise(p => p.OnSummaryChanged += null, new ValueChangedEventArgs<string>("newSummary"));
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void OnDatasetCreated()
        {
            var project = new Mock<IProject>();
            var facade = new ProjectFacadeForScripts(new ProjectFacade(project.Object));

            bool eventRaised = false;
            facade.OnDatasetCreated += (s, e) => { eventRaised = true; };

            project.Raise(p => p.OnDatasetCreated += null, EventArgs.Empty);
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void OnDatasetDeleted()
        {
            var project = new Mock<IProject>();
            var facade = new ProjectFacadeForScripts(new ProjectFacade(project.Object));

            bool eventRaised = false;
            facade.OnDatasetDeleted += (s, e) => { eventRaised = true; };

            project.Raise(p => p.OnDatasetDeleted += null, EventArgs.Empty);
            Assert.IsTrue(eventRaised);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void SaveProject()
        {
        }

        [Test]
        public void Root()
        {
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.BaseDataset())
                    .Returns(new Mock<IProxyDataset>().Object);
            }

            var facade = new ProjectFacadeForScripts(new ProjectFacade(project.Object));
            var root = facade.Root();
            Assert.IsNotNull(root);
        }
    }
}
