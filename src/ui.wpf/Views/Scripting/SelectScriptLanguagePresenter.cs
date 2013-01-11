//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Autofac;

namespace Apollo.UI.Wpf.Views.Scripting
{
    /// <summary>
    /// The presenter for the <see cref="SelectScriptLanguageModel"/>.
    /// </summary>
    public sealed class SelectScriptLanguagePresenter : Presenter<ISelectScriptLanguageView, SelectScriptLanguageModel, SelectScriptLanguageParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectScriptLanguagePresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public SelectScriptLanguagePresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            View.Model = new SelectScriptLanguageModel(context);
        }
    }
}
