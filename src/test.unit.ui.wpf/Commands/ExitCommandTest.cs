//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Application;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ExitCommandTest
    {
        [Test]
        public void CanShutdownWithNullApplication()
        {
            var command = new ExitCommand(null);
            Assert.IsFalse(command.CanExecute(null));
        }

        [Test]
        public void Shutdown()
        {
            var application = new Mock<IAbstractApplications>();
            {
                application.Setup(a => a.Shutdown())
                    .Verifiable();
            }

            var command = new ExitCommand(application.Object);
            Assert.IsTrue(command.CanExecute(null));
            command.Execute(null);

            application.Verify(a => a.Shutdown(), Times.Once());
        }
    }
}
