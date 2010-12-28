//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.UserInterfaces.Project;
using Autofac;

namespace Apollo.UI.Common.Views.Projects
{
    /// <summary>
    /// The presenter for the <see cref="ProjectModel"/>.
    /// </summary>
    public sealed class ProjectPresenter : Presenter<IProjectView, ProjectModel, ProjectParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the menu.</param>
        public ProjectPresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            View.Model = new ProjectModel(m_Container.Resolve<ILinkToProjects>());
        }
    }
}
