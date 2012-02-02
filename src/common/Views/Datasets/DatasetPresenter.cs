//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Common.Commands;
using Apollo.Utilities;
using Autofac;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// The presenter for the <see cref="DatasetModel"/>.
    /// </summary>
    public sealed class DatasetPresenter : Presenter<IDatasetView, DatasetModel, DatasetParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public DatasetPresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Creates a new dataset model.
        /// </summary>
        /// <param name="dataset">The dataset for the new model.</param>
        /// <returns>
        /// The dataset model for the given facade.
        /// </returns>
        public DatasetModel CreateModel(DatasetFacade dataset)
        {
            var context = m_Container.Resolve<IContextAware>();
            var projectFacade = m_Container.Resolve<ILinkToProjects>();
            var progressTracker = m_Container.Resolve<ITrackSteppingProgress>();

            var result = new DatasetModel(context, progressTracker, projectFacade, dataset)
            {
                NewChildDatasetCommand = new AddChildDatasetCommand(projectFacade, dataset),
                DeleteDatasetCommand = new DeleteDatasetCommand(projectFacade, dataset),
                LoadDatasetCommand = CreateLoadDatasetCommand(dataset),
                UnloadDatasetCommand = new UnloadDatasetFromMachineCommand(dataset),
            };

            return result;
        }

        private LoadDatasetOntoMachineCommand CreateLoadDatasetCommand(DatasetFacade dataset)
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
                    else
                    {
                        return new SelectedProposal();
                    }
                };

            var command = m_Container.Resolve<LoadDatasetOntoMachineCommand>(
                new TypedParameter(typeof(DatasetFacade), dataset),
                new TypedParameter(typeof(Func<IEnumerable<DistributionSuggestion>, SelectedProposal>), selector));
            return command;
        }
    }
}
