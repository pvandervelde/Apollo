﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectFacadeTest
    {
        [Test]
        public void Name()
        {
            var project = new Mock<IProject>();
            {
                project.SetupProperty(p => p.Name);
            }

            var facade = new ProjectFacade(project.Object);
            
            var name = "name";
            facade.Name = name;
            Assert.AreEqual(name, facade.Name);
        }

        [Test]
        public void OnNameUpdate()
        {
            var project = new Mock<IProject>();
            var facade = new ProjectFacade(project.Object);
            
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

            var facade = new ProjectFacade(project.Object);

            var summary = "text";
            facade.Summary = summary;
            Assert.AreEqual(summary, facade.Summary);
        }

        [Test]
        public void OnSummaryUpdate()
        {
            var project = new Mock<IProject>();
            var facade = new ProjectFacade(project.Object);

            bool eventRaised = false;
            facade.OnSummaryChanged += (s, e) => { eventRaised = true; };

            project.Raise(p => p.OnSummaryChanged += null, new ValueChangedEventArgs<string>("newSummary"));
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void OnDatasetCreated()
        {
            var project = new Mock<IProject>();
            var facade = new ProjectFacade(project.Object);

            bool eventRaised = false;
            facade.OnDatasetCreated += (s, e) => { eventRaised = true; };

            project.Raise(p => p.OnDatasetCreated += null, EventArgs.Empty);
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void OnDatasetDeleted()
        {
            var project = new Mock<IProject>();
            var facade = new ProjectFacade(project.Object);

            bool eventRaised = false;
            facade.OnDatasetDeleted += (s, e) => { eventRaised = true; };

            project.Raise(p => p.OnDatasetDeleted += null, EventArgs.Empty);
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void SaveProject()
        {
            var persistence = new Mock<IPersistenceInformation>();
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.Save(It.IsAny<IPersistenceInformation>()))
                    .Callback<IPersistenceInformation>(info => Assert.AreSame(persistence.Object, info));
            }

            var facade = new ProjectFacade(project.Object);
            facade.SaveProject(persistence.Object);
        }

        [Test]
        public void Root()
        {
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.BaseDataset())
                    .Returns(new Mock<IProxyDataset>().Object);
            }

            var facade = new ProjectFacade(project.Object);
            var root = facade.Root();
            Assert.IsNotNull(root);
        }
    }
}
