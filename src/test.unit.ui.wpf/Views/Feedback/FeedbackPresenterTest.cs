//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Wpf.Commands;
using Apollo.UI.Wpf.Feedback;
using Apollo.Utilities;
using Moq;
using NSarrac.Framework;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FeedbackPresenterTest
    {
        [Test]
        public void Initialize()
        {
            var context = new Mock<IContextAware>();

            var view = new Mock<IFeedbackView>();
            {
                view.SetupSet(v => v.Model = It.IsAny<FeedbackModel>())
                    .Verifiable();
            }

            var parameter = new FeedbackParameter(context.Object);

            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
                container.Setup(c => c.Resolve<SendFeedbackReportCommand>())
                    .Returns(new SendFeedbackReportCommand(new Mock<ISendFeedbackReports>().Object));
                container.Setup(c => c.Resolve<IBuildReports>())
                    .Returns(new Mock<IBuildReports>().Object);
            }

            var presenter = new FeedbackPresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
            view.VerifySet(v => v.Model = It.IsAny<FeedbackModel>(), Times.Once());
        }
    }
}
