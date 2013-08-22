//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Apollo.Utilities.History;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class AddChildDatasetCommandTest
    {
        private sealed class MockDisposable : IDisposable
        {
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // Do nothing.
            }
        }

        [Test]
        public void CanAddNewChildWithNullDatasetFacade()
        {
            var project = new Mock<ILinkToProjects>();
            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new AddChildDatasetCommand(project.Object, null, func);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanAddNewChildWithInvalidDatasetFacade()
        {
            var project = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsValid)
                    .Returns(false);
            }

            var dataset = new DatasetFacade(proxy.Object);
            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new AddChildDatasetCommand(project.Object, dataset, func);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanAddNewChildWithDatasetNotAllowedToBecomeParent()
        {
            var project = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsValid)
                    .Returns(true);
                proxy.Setup(p => p.CanBecomeParent)
                    .Returns(false);
            }

            var dataset = new DatasetFacade(proxy.Object);
            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new AddChildDatasetCommand(project.Object, dataset, func);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void AddNewChild()
        {
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

            var childProxy = new Mock<IProxyDataset>();
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsValid)
                    .Returns(true);
                proxy.Setup(p => p.CanBecomeParent)
                    .Returns(true);
                proxy.Setup(p => p.StoredAt)
                    .Returns(new NullPersistenceInformation());
                proxy.Setup(p => p.CreateNewChild(It.IsAny<DatasetCreationInformation>()))
                    .Returns(childProxy.Object)
                    .Verifiable();
            }

            var dataset = new DatasetFacade(proxy.Object);
            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new AddChildDatasetCommand(projectLink.Object, dataset, func);
            Assert.IsTrue(command.CanExecute(null));

            command.Execute(null);
            proxy.Verify(p => p.CreateNewChild(It.IsAny<DatasetCreationInformation>()), Times.Once());
            history.Verify(h => h.Mark(), Times.Once());
        }
    }
}
