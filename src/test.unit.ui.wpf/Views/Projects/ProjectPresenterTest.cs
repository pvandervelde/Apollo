//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectPresenterTest
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
        public void Initialize()
        {
            var context = new Mock<IContextAware>();

            var view = new Mock<IProjectView>();
            {
                view.SetupSet(v => v.Model = It.IsAny<ProjectModel>())
                    .Verifiable();
            }

            var parameter = new ProjectParameter(context.Object);

            var projectLink = new Mock<ILinkToProjects>();
            Func<string, IDisposable> func = s => new MockDisposable();

            var command = new CloseProjectCommand(projectLink.Object, func);
            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
                container.Setup(c => c.Resolve<CloseProjectCommand>())
                    .Returns(command);
            }

            var presenter = new ProjectPresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
            view.VerifySet(v => v.Model = It.IsAny<ProjectModel>(), Times.Once());
        }
    }
}
