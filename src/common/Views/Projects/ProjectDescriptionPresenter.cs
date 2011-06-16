//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Apollo.Core.UserInterfaces.Projects;
using Autofac;

namespace Apollo.UI.Common.Views.Projects
{
    /// <summary>
    /// The presenter for the <see cref="ProjectDescriptionModel"/>.
    /// </summary>
    public sealed class ProjectDescriptionPresenter : Presenter<IProjectDescriptionView, ProjectDescriptionModel, ProjectDescriptionParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDescriptionPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public ProjectDescriptionPresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();

            var serviceFacade = m_Container.Resolve<ILinkToProjects>();
            var project = serviceFacade.ActiveProject();

            Debug.Assert(project != null, "There should be an active project.");
            View.Model = new ProjectDescriptionModel(context, project);
        }
    }
}
