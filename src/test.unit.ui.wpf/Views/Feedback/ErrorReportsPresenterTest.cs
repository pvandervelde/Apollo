//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Apollo.UI.Wpf.Commands;
using Apollo.UI.Wpf.Feedback;
using Apollo.Utilities;
using Moq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ErrorReportsPresenterTest
    {
        [Test]
        public void Initialize()
        {
            var context = new Mock<IContextAware>();
            var collector = new Mock<ICollectFeedbackReports>();
            {
                collector.Setup(c => c.LocateFeedbackReports())
                    .Returns(new List<FileInfo>());
            }

            var fileSystem = new Mock<IFileSystem>();
            var sender = new Mock<ISendFeedbackReports>();
            var command = new SendFeedbackReportCommand(sender.Object);

            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
                container.Setup(c => c.Resolve<ICollectFeedbackReports>())
                    .Returns(collector.Object);
                container.Setup(c => c.Resolve<IFileSystem>())
                    .Returns(fileSystem.Object);
                container.Setup(c => c.Resolve<SendFeedbackReportCommand>())
                    .Returns(command);
            }

            var view = new Mock<IErrorReportsView>();
            {
                view.SetupSet(v => v.Model = It.IsAny<ErrorReportsModel>())
                    .Verifiable();
            }

            var parameter = new ErrorReportsParameter(context.Object);

            var presenter = new ErrorReportsPresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
            view.VerifySet(v => v.Model = It.IsAny<ErrorReportsModel>(), Times.Once());
        }
    }
}
