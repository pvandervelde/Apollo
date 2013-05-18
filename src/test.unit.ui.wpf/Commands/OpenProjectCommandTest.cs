//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class OpenProjectCommandTest
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
        public void CanLoadProjectWithNullFacade()
        {
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new OpenProjectCommand(null, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanLoadProjectWhileUnableToLoad()
        {
            var project = new Mock<ILinkToProjects>();
            {
                project.Setup(p => p.CanLoadProject())
                    .Returns(false);
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new OpenProjectCommand(project.Object, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void LoadProject()
        {
            var project = new Mock<ILinkToProjects>();
            {
                project.Setup(p => p.LoadProject(It.IsAny<IPersistenceInformation>()))
                    .Verifiable();
            }

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new OpenProjectCommand(project.Object, timerFunc);

            var persistence = new Mock<IPersistenceInformation>();
            command.Execute(persistence.Object);

            project.Verify(p => p.LoadProject(It.IsAny<IPersistenceInformation>()), Times.Once());
        }
    }
}
