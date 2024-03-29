﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;
using NSarrac.Framework;

namespace Apollo.UI.Wpf.Views.Feedback
{
    /// <summary>
    /// Defines a presenter that creates <see cref="FeedbackModel"/> objects and connects them to
    /// <see cref="IFeedbackView"/> objects.
    /// </summary>
    public sealed class FeedbackPresenter : Presenter<IFeedbackView, FeedbackModel, FeedbackParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the error reports presenter.</param>
        public FeedbackPresenter(IDependencyInjectionProxy container)
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
            var reportBuilder = m_Container.Resolve<IBuildReports>();
            View.Model = new FeedbackModel(context, command, () => reportBuilder);
        }
    }
}
