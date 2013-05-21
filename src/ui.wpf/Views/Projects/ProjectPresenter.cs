//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;

namespace Apollo.UI.Wpf.Views.Projects
{
    /// <summary>
    /// The presenter for the <see cref="ProjectModel"/>.
    /// </summary>
    public sealed class ProjectPresenter : Presenter<IProjectView, ProjectModel, ProjectParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public ProjectPresenter(IDependencyInjectionProxy container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            var command = m_Container.Resolve<CloseProjectCommand>();
            View.Model = new ProjectModel(context, command);
        }
    }
}
