//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utilities;
using Nuclei.Progress;

namespace Apollo.UI.Wpf.Views.Progress
{
    /// <summary>
    /// Defines a presenter that creates <see cref="ProgressModel"/> objects and connects them to
    /// <see cref="IProgressView"/> objects.
    /// </summary>
    public sealed class ProgressPresenter : Presenter<IProgressView, ProgressModel, ProgressParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the error reports presenter.</param>
        public ProgressPresenter(IDependencyInjectionProxy container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            var reportCollector = m_Container.Resolve<ICollectProgressReports>();
            View.Model = new ProgressModel(context, reportCollector);
        }
    }
}
