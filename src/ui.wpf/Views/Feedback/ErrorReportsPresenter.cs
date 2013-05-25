//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO.Abstractions;
using Apollo.UI.Wpf.Commands;
using Apollo.UI.Wpf.Feedback;
using Apollo.Utilities;

namespace Apollo.UI.Wpf.Views.Feedback
{
    /// <summary>
    /// Defines a presenter that creates <see cref="ErrorReportsModel"/> objects and connects them to
    /// <see cref="IErrorReportsView"/> objects.
    /// </summary>
    public sealed class ErrorReportsPresenter : Presenter<IErrorReportsView, ErrorReportsModel, ErrorReportsParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportsPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the error reports presenter.</param>
        public ErrorReportsPresenter(IDependencyInjectionProxy container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            var command = m_Container.Resolve<SendFeedbackReportCommand>();
            var reportCollector = m_Container.Resolve<ICollectFeedbackReports>();
            var fileSystem = m_Container.Resolve<IFileSystem>();
            View.Model = new ErrorReportsModel(context, command, reportCollector, fileSystem);
        }
    }
}
