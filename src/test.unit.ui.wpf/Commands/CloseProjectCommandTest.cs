//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CloseProjectCommandTest
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
        public void CanCloseWithNullProjectFacade()
        {
            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new CloseProjectCommand(null, func);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanCloseWithProjectThatCannotBeUnloaded()
        {
            var project = new Mock<ILinkToProjects>();
            {
                project.Setup(p => p.CanUnloadProject())
                    .Returns(false);
            }

            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new CloseProjectCommand(project.Object, func);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CloseProject()
        {
            var project = new Mock<ILinkToProjects>();
            {
                project.Setup(p => p.CanUnloadProject())
                    .Returns(true);
                project.Setup(p => p.UnloadProject())
                    .Verifiable();
            }

            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new CloseProjectCommand(project.Object, func);
            Assert.IsTrue(command.CanExecute(null));

            command.Execute(null);
            project.Verify(p => p.UnloadProject(), Times.Once());
        }
    }
}
