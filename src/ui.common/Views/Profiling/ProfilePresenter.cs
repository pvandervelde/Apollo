//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Common.Profiling;
using Autofac;

namespace Apollo.UI.Common.Views.Profiling
{
    /// <summary>
    /// Defines a presenter that creates <see cref="ProfileModel"/> objects and connects them to
    /// <see cref="IProfileView"/> objects.
    /// </summary>
    public sealed class ProfilePresenter : Presenter<IProfileView, ProfileModel, ProfileParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the error reports presenter.</param>
        public ProfilePresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            var storage = m_Container.Resolve<TimingReportCollection>();
            View.Model = new ProfileModel(context, storage);
        }
    }
}
