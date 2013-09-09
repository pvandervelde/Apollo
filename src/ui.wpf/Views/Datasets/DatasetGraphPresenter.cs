//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Apollo.Core.Base.Activation;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;
using Autofac;
using Microsoft.Practices.Prism.Events;
using Nuclei.Diagnostics;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// The presenter for the <see cref="DatasetGraphModel"/>.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DatasetGraphPresenter : Presenter<IDatasetGraphView, DatasetGraphModel, DatasetGraphParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the dataset presenter.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetGraphPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the dataset presenter.</param>
        public DatasetGraphPresenter(IDependencyInjectionProxy container)
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
            View.Model = new DatasetGraphModel(context, project, CreateModel);
        }

        /// <summary>
        /// Creates a new dataset model.
        /// </summary>
        /// <param name="dataset">The dataset for the new model.</param>
        /// <returns>
        /// The dataset model for the given facade.
        /// </returns>
        private DatasetModel CreateModel(DatasetFacade dataset)
        {
            var context = m_Container.Resolve<IContextAware>();
            var projectFacade = m_Container.Resolve<ILinkToProjects>();
            var progressTracker = m_Container.Resolve<ITrackSteppingProgress>();
            var timer = m_Container.Resolve<Func<string, IDisposable>>();
            var eventAggregator = m_Container.Resolve<IEventAggregator>();

            var result = new DatasetModel(context, progressTracker, projectFacade, dataset)
            {
                NewChildDatasetCommand = new AddChildDatasetCommand(projectFacade, dataset, timer),
                DeleteDatasetCommand = new DeleteDatasetCommand(projectFacade, dataset, timer),
                ActivateDatasetCommand = CreateActivateDatasetCommand(projectFacade, dataset, timer),
                DeactivateDatasetCommand = new DeactivateDatasetCommand(projectFacade, dataset, timer),
                ShowDetailViewCommand = new ShowDatasetDetailViewCommand(context, eventAggregator, dataset),
            };

            return result;
        }

        private ActivateDatasetCommand CreateActivateDatasetCommand(
            ILinkToProjects projectFacade, 
            DatasetFacade dataset, 
            Func<string, IDisposable> timer)
        {
            var context = m_Container.Resolve<IContextAware>();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                c =>
                {
                    var presenter = (IPresenter)m_Container.Resolve(typeof(MachineSelectorPresenter));
                    var view = m_Container.Resolve(presenter.ViewType) as IMachineSelectorView;
                    presenter.Initialize(view, new MachineSelectorParameter(context, c));

                    var window = view as Window;
                    window.Owner = Application.Current.MainWindow;
                    if (window.ShowDialog() ?? false)
                    {
                        return new SelectedProposal(view.Model.SelectedPlan);
                    }
                    
                    return new SelectedProposal();
                };

            var command = m_Container.Resolve<ActivateDatasetCommand>(
                new TypedParameter(typeof(ILinkToProjects), projectFacade),
                new TypedParameter(typeof(DatasetFacade), dataset),
                new TypedParameter(typeof(Func<IEnumerable<DistributionSuggestion>, SelectedProposal>), selector),
                new TypedParameter(typeof(SystemDiagnostics), timer));
            return command;
        }
    }
}
