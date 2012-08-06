//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Input;
using Apollo.UI.Common;
using Apollo.UI.Common.Commands;
using Autofac;
using Microsoft.Practices.Prism.Events;

namespace Apollo.ProjectExplorer.Views.Welcome
{
    /// <summary>
    /// The presenter for the <see cref="WelcomeModel"/>.
    /// </summary>
    internal sealed class WelcomePresenter : Presenter<IWelcomeView, WelcomeModel, WelcomeParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomePresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public WelcomePresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            View.Model = new WelcomeModel(context) 
                { 
                    CloseCommand = CreateCloseCommand(context),
                    NewProjectCommand = CompositeCommandBuilder.CloseWelcomeViewAndInvokeCommand<NewProjectCommand>(m_Container),
                    OpenProjectCommand = CompositeCommandBuilder.CloseWelcomeViewAndInvokeCommand<OpenProjectCommand>(m_Container),
                };
        }

        private ICommand CreateCloseCommand(IContextAware context)
        {
            return m_Container.Resolve<CloseViewCommand>(
                new TypedParameter(typeof(IEventAggregator), m_Container.Resolve<IEventAggregator>()),
                new TypedParameter(typeof(string), CommonRegionNames.Content),
                new TypedParameter(typeof(Parameter), new WelcomeParameter(context)));
        }
    }
}
