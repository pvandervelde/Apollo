//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Common.Commands;
using Apollo.Utilities;
using Autofac;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// The presenter for the <see cref="DatasetModel"/>.
    /// </summary>
    public sealed class DatasetDetailPresenter : Presenter<IDatasetDetailView, DatasetDetailModel, DatasetDetailParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetDetailPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public DatasetDetailPresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            var projectFacade = m_Container.Resolve<ILinkToProjects>();
            var progressTracker = m_Container.Resolve<ITrackSteppingProgress>();
            var timer = m_Container.Resolve<Func<string, IDisposable>>();
            var eventAggregator = m_Container.Resolve<IEventAggregator>();

            var model = new DatasetDetailModel(context, progressTracker, projectFacade, Parameter.Dataset) 
                {
                    // ShowDatasetAdvancedViewCommand = new ShowDatasetAdvancedViewCommand(context, eventAggregator, Parameter.Dataset),
                    SwitchDatasetToEditModeCommand = new SwitchDatasetToEditModeCommand(Parameter.Dataset),
                    SwitchDatasetToExecutingModeCommand = new SwitchDatasetToExecutingModeCommand(Parameter.Dataset),
                };

            View.Model = model;
        }
    }
}
