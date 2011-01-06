//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.UserInterfaces.Project;
using Autofac;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// The presenter for the <see cref="DatasetGraphModel"/>.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DatasetGraphPresenter : Presenter<IDatasetGraphView, DatasetGraphModel, DatasetGraphParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetGraphPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the menu.</param>
        public DatasetGraphPresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var serviceFacade = m_Container.Resolve<ILinkToProjects>();
            var project = serviceFacade.ActiveProject();

            var context = m_Container.Resolve<IContextAware>();

            Debug.Assert(project != null, "There should be an active project.");
            View.Model = new DatasetGraphModel(project, context);
        }
    }
}
