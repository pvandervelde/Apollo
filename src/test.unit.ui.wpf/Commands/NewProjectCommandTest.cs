//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class NewProjectCommandTest
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
        public void CanCreateNewProjectWithNullFacade()
        {
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new NewProjectCommand(null, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanCreateNewProjectWithFacadeUnableToCreate()
        {
            var project = new Mock<ILinkToProjects>();
            {
                project.Setup(p => p.CanCreateNewProject())
                    .Returns(false);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new NewProjectCommand(project.Object, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CreateNewProject()
        {
            var project = new Mock<ILinkToProjects>();
            {
                project.Setup(p => p.NewProject())
                    .Verifiable();
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new NewProjectCommand(project.Object, timerFunc);
            command.Execute(null);

            project.Verify(p => p.NewProject(), Times.Once());
        }
    }
}
