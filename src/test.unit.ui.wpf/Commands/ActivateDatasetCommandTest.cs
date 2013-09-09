//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.Core.Base.Activation;
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
    public sealed class ActivateDatasetCommandTest
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
        public void CanActivateWithNullDataset()
        {
            var project = new Mock<ILinkToProjects>();

            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selectionFunc = suggestions => null;
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new ActivateDatasetCommand(project.Object, null, selectionFunc, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanActivateWithActivatedDataset()
        {
            var project = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsActivated)
                    .Returns(true);
            }

            var dataset = new DatasetFacade(proxy.Object);

            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selectionFunc = suggestions => null;
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new ActivateDatasetCommand(project.Object, dataset, selectionFunc, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void CanActivateWithNonActivatableDataset()
        {
            var project = new Mock<ILinkToProjects>();

            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsActivated)
                    .Returns(false);
                proxy.Setup(p => p.CanActivate)
                    .Returns(false);
            }

            var dataset = new DatasetFacade(proxy.Object);

            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selectionFunc = suggestions => null;
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new ActivateDatasetCommand(project.Object, dataset, selectionFunc, timerFunc);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void ActivateDataset()
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
                proxy.Setup(p => p.IsActivated)
                    .Returns(false);
                proxy.Setup(p => p.CanActivate)
                    .Returns(true);
                proxy.Setup(
                    p => p.Activate(
                        It.IsAny<DistributionLocations>(), 
                        It.IsAny<Func<IEnumerable<DistributionSuggestion>, SelectedProposal>>(),
                        It.IsAny<CancellationToken>()))
                    .Verifiable();
            }

            var dataset = new DatasetFacade(proxy.Object);

            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selectionFunc = suggestions => null;
            Func<string, IDisposable> timerFunc = s => new MockDisposable();

            var command = new ActivateDatasetCommand(projectLink.Object, dataset, selectionFunc, timerFunc);
            command.Execute(null);

            proxy.Verify(
                p => p.Activate(
                    It.IsAny<DistributionLocations>(),
                    It.IsAny<Func<IEnumerable<DistributionSuggestion>, SelectedProposal>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
            history.Verify(h => h.Mark(), Times.Once());
        }
    }
}
