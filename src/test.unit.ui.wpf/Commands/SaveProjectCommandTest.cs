//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Apollo.Utilities.History;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SaveProjectCommandTest
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
        public void CanSaveProjectWithNullFacade()
        {
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new SaveProjectCommand(null, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanSaveProjectWithoutActiveProject()
        {
            var project = new Mock<ILinkToProjects>();
            {
                project.Setup(p => p.HasActiveProject())
                    .Returns(false);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new SaveProjectCommand(project.Object, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanSaveProjectWithoutChanges()
        {
            var project = new Mock<IProject>();

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.HasActiveProject())
                    .Returns(true);
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new SaveProjectCommand(projectLink.Object, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void LoadProject()
        {
            var history = new Mock<ITimeline>();
            {
                history.Setup(h => h.Mark(It.IsAny<string>()))
                    .Verifiable();
            }

            var project = new Mock<IProject>();
            {
                project.Setup(p => p.History)
                    .Returns(history.Object);
                project.Setup(p => p.Save(It.IsAny<IPersistenceInformation>()))
                    .Verifiable();
            }

            var projectFacade = new ProjectFacade(project.Object);
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.HasActiveProject())
                    .Returns(true);
                projectLink.Setup(p => p.ActiveProject())
                    .Returns(projectFacade);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new SaveProjectCommand(projectLink.Object, timerFunc);

            var persistence = new Mock<IPersistenceInformation>();
            command.Execute(persistence.Object);

            project.Verify(p => p.Save(It.IsAny<IPersistenceInformation>()), Times.Once());
            history.Verify(h => h.Mark(It.IsAny<string>()), Times.Once());
        }
    }
}
