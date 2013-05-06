//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;
using Autofac;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Nuclei.Progress;

namespace Apollo.UI.Wpf.Views.Datasets
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

            var model = new DatasetDetailModel(context, progressTracker, projectFacade, Parameter.Dataset) 
                {
                    // ShowDatasetAdvancedViewCommand = new ShowDatasetAdvancedViewCommand(context, eventAggregator, Parameter.Dataset),
                    SwitchDatasetToEditModeCommand = new SwitchDatasetToEditModeCommand(Parameter.Dataset),
                    SwitchDatasetToExecutingModeCommand = new SwitchDatasetToExecutingModeCommand(Parameter.Dataset),
                    CloseCommand = CreateCloseCommand(),
                };

            View.Model = model;
        }

        private CompositeCommand CreateCloseCommand()
        {
            var closeViewCommand = m_Container.Resolve<CloseViewCommand>(
                new TypedParameter(typeof(IEventAggregator), m_Container.Resolve<IEventAggregator>()),
                new TypedParameter(typeof(string), CommonRegionNames.Content),
                new TypedParameter(typeof(Parameter), Parameter));
            var compositeCommand = new CompositeCommand();
            compositeCommand.RegisterCommand(closeViewCommand);
            return compositeCommand;
        }
    }
}
