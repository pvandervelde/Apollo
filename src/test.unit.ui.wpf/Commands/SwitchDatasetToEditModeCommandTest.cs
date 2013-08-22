//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SwitchDatasetToEditModeCommandTest
    {
        [Test]
        public void CanSwitchWhileDatasetIsInEditMode()
        {
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.IsEditMode)
                    .Returns(true);
            }

            var dataset = new DatasetFacade(proxy.Object);

            var command = new SwitchDatasetToEditModeCommand(dataset);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void SwitchToEditMode()
        {
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.SwitchToEditMode())
                    .Verifiable();
            }

            var dataset = new DatasetFacade(proxy.Object);

            var command = new SwitchDatasetToEditModeCommand(dataset);
            command.Execute(null);

            proxy.Verify(p => p.SwitchToEditMode(), Times.Once());
        }
    }
}
