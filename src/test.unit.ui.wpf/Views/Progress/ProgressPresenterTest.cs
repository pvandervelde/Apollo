//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Progress
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ProgressPresenterTest
    {
        [Test]
        public void Initialize()
        {
            var context = new Mock<IContextAware>();

            var view = new Mock<IProgressView>();
            {
                view.SetupSet(v => v.Model = It.IsAny<ProgressModel>())
                    .Verifiable();
            }

            var parameter = new ProgressParameter(context.Object);
            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
                container.Setup(c => c.Resolve<ICollectProgressReports>())
                    .Returns(new Mock<ICollectProgressReports>().Object);
            }

            var presenter = new ProgressPresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
            view.VerifySet(v => v.Model = It.IsAny<ProgressModel>(), Times.Once());
        }
    }
}
