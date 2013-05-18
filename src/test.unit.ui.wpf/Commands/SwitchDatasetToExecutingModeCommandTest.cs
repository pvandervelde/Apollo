//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SwitchDatasetToExecutingModeCommandTest
    {
        [Test]
        public void CanSwitchWhileDatasetIsInEditMode()
        {
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsEditMode)
                    .Returns(false);
            }

            var dataset = new DatasetFacade(proxy.Object);

            var command = new SwitchDatasetToExecutingModeCommand(dataset);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void SwitchToEditMode()
        {
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.SwitchToExecutingMode())
                    .Verifiable();
            }

            var dataset = new DatasetFacade(proxy.Object);

            var command = new SwitchDatasetToExecutingModeCommand(dataset);
            command.Execute(null);

            proxy.Verify(p => p.SwitchToExecutingMode(), Times.Once());
        }
    }
}
