//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities.History;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DeactivateDatasetCommandTest
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
        public void CanDeactivateWithNullDataset()
        {
            var project = new Mock<ILinkToProjects>();

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new DeactivateDatasetCommand(project.Object, null, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanDeactivateWithDeactivatedDataset()
        {
            var project = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsActivated)
                    .Returns(false);
            }

            var dataset = new DatasetFacade(proxy.Object);

            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new DeactivateDatasetCommand(project.Object, dataset, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void DeactivateDataset()
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

            using (var source = new CancellationTokenSource())
            {
                var proxy = new Mock<IProxyDataset>();
                {
                    proxy.Setup(p => p.IsActivated)
                        .Returns(true);
                    proxy.Setup(p => p.Deactivate())
                        .Returns(() => Task.Factory.StartNew(
                            () =>
                            {
                            },
                            source.Token,
                            TaskCreationOptions.None,
                            new CurrentThreadTaskScheduler()))
                        .Verifiable();
                }

                var dataset = new DatasetFacade(proxy.Object);

                Func<string, IDisposable> timerFunc = s => new MockDisposable();

                var command = new DeactivateDatasetCommand(projectLink.Object, dataset, timerFunc);
                command.Execute(null);

                proxy.Verify(p => p.Deactivate(), Times.Once());
                history.Verify(h => h.Mark(), Times.Once());
            }
        }
    }
}
