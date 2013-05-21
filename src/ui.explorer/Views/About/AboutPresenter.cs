//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Wpf;
using Apollo.Utilities;

namespace Apollo.UI.Explorer.Views.About
{
    /// <summary>
    /// The presenter for the <see cref="AboutModel"/>.
    /// </summary>
    internal sealed class AboutPresenter : Presenter<IAboutView, AboutModel, AboutParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the menu.</param>
        public AboutPresenter(IDependencyInjectionProxy container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            View.Model = new AboutModel(context);
        }
    }
}
