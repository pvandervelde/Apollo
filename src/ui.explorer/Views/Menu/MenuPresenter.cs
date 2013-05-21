//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Explorer.Commands;
using Apollo.UI.Wpf;
using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;

namespace Apollo.UI.Explorer.Views.Menu
{
    /// <summary>
    /// Defines the presenter for the <see cref="MenuModel"/>.
    /// </summary>
    internal sealed class MenuPresenter : Presenter<IMenuView, MenuModel, MenuParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the menu.</param>
        public MenuPresenter(IDependencyInjectionProxy container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            View.Model = new MenuModel(context);
            View.Model.NewProjectCommand = CompositeCommandBuilder.CloseWelcomeViewAndInvokeCommand<NewProjectCommand>(m_Container);
            View.Model.OpenProjectCommand = CompositeCommandBuilder.CloseWelcomeViewAndInvokeCommand<OpenProjectCommand>(m_Container);
            View.Model.SaveProjectCommand = m_Container.Resolve<SaveProjectCommand>();
            View.Model.CloseProjectCommand = m_Container.Resolve<CloseProjectCommand>();
            View.Model.ExitCommand = m_Container.Resolve<ExitCommand>();

            View.Model.UndoCommand = m_Container.Resolve<UndoCommand>();
            View.Model.RedoCommand = m_Container.Resolve<RedoCommand>();

            View.Model.ShowStartPageCommand = m_Container.Resolve<ShowWelcomeViewTabCommand>();
            View.Model.ShowProjectsCommand = m_Container.Resolve<ShowProjectsTabCommand>();
            View.Model.ShowScriptsCommand = m_Container.Resolve<ShowScriptsTabCommand>();
            View.Model.AboutCommand = m_Container.Resolve<ShowAboutWindowCommand>();
        }
    }
}
