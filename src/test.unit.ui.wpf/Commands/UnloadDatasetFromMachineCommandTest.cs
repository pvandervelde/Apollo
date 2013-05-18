//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
    public sealed class UnloadDatasetFromMachineCommandTest
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
        public void CanUnloadWithNullDataset()
        {
            var project = new Mock<ILinkToProjects>();

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UnloadDatasetFromMachineCommand(project.Object, null, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanUnloadWithNonLoadedDataset()
        {
            var project = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsLoaded)
                    .Returns(false);
            }

            var dataset = new DatasetFacade(proxy.Object);

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UnloadDatasetFromMachineCommand(project.Object, dataset, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void UnloadDataset()
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

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsLoaded)
                    .Returns(true);
                proxy.Setup(p => p.UnloadFromMachine())
                    .Verifiable();
            }

            var dataset = new DatasetFacade(proxy.Object);

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new UnloadDatasetFromMachineCommand(projectLink.Object, dataset, timerFunc);
            command.Execute(null);

            proxy.Verify(p => p.UnloadFromMachine(), Times.Once());
            history.Verify(h => h.Mark(), Times.Once());
        }
    }
}
