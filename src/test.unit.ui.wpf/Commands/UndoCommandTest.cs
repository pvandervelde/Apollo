//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities.History;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class UndoCommandTest
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
        public void CanUndoWithNullFacade()
        {
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UndoCommand(null, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanUndoWithoutActiveProject()
        {
            var projectLink = new Mock<ILinkToProjects>();
            {
                projectLink.Setup(p => p.HasActiveProject())
                    .Returns(false);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UndoCommand(projectLink.Object, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanUndoWithHistoryWithoutRollBack()
        {
            var history = new Mock<ITimeline>();
            {
                history.Setup(h => h.CanRollBack)
                    .Returns(false);
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

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UndoCommand(projectLink.Object, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanUndoWithoutMarkers()
        {
            var currentMark = new TimeMarker(10);
            var markers = new List<TimeMarker>
                {
                    currentMark,
                };
            var history = new Mock<ITimeline>();
            {
                history.Setup(h => h.CanRollBack)
                    .Returns(true);
                history.Setup(h => h.MarkersInThePast())
                    .Returns(markers);
                history.Setup(h => h.Current)
                    .Returns(currentMark);
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
                projectLink.Setup(p => p.HasActiveProject())
                    .Returns(true);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UndoCommand(projectLink.Object, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanUndoWithMarkers()
        {
            var currentMark = new TimeMarker(10);
            var markers = new List<TimeMarker>
                {
                    currentMark,
                    new TimeMarker(1),
                };
            var history = new Mock<ITimeline>();
            {
                history.Setup(h => h.CanRollBack)
                    .Returns(true);
                history.Setup(h => h.MarkersInThePast())
                    .Returns(markers);
                history.Setup(h => h.Current)
                    .Returns(currentMark);
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
                projectLink.Setup(p => p.HasActiveProject())
                    .Returns(true);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UndoCommand(projectLink.Object, timerFunc);
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void Undo()
        {
            var currentMark = new TimeMarker(10);
            var markers = new List<TimeMarker>
                {
                    currentMark,
                    new TimeMarker(1),
                };
            var history = new Mock<ITimeline>();
            {
                history.Setup(h => h.CanRollBack)
                    .Returns(true);
                history.Setup(h => h.MarkersInThePast())
                    .Returns(markers);
                history.Setup(h => h.Current)
                    .Returns(currentMark);
                history.Setup(h => h.RollBackTo(It.IsAny<TimeMarker>(), It.IsAny<TimelineTraveller>()))
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
                projectLink.Setup(p => p.HasActiveProject())
                    .Returns(true);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UndoCommand(projectLink.Object, timerFunc);
            command.Execute(null);

            history.Verify(h => h.RollBackTo(It.IsAny<TimeMarker>(), It.IsAny<TimelineTraveller>()), Times.Once());
        }
    }
}
