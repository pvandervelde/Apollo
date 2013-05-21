//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utilities;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// The presenter for the <see cref="MachineSelectorModel"/>.
    /// </summary>
    public sealed class MachineSelectorPresenter : Presenter<IMachineSelectorView, MachineSelectorModel, MachineSelectorParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineSelectorPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public MachineSelectorPresenter(IDependencyInjectionProxy container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            View.Model = new MachineSelectorModel(context, Parameter.Suggestions);
        }
    }
}
