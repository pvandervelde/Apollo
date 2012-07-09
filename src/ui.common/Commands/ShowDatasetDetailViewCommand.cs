//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Common.Events;
using Apollo.UI.Common.Views.Datasets;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the showing of the dataset detail view.
    /// </summary>
    public sealed class ShowDatasetDetailViewCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the script host can be closed.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>
        /// <see langword="true" /> if the script host can be closed; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanShow(DatasetFacade dataset)
        {
            return dataset.IsLoaded;
        }

        /// <summary>
        /// Closes the script host.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="dataset">The dataset.</param>
        private static void OnShow(IContextAware context, IEventAggregator eventAggregator, DatasetFacade dataset)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (dataset == null)
            {
                return;
            }

            eventAggregator.GetEvent<ShowViewEvent>().Publish(
                new ShowViewRequest(
                    typeof(DatasetDetailPresenter),
                    CommonRegionNames.Content,
                    new DatasetDetailParameter(context, dataset)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowDatasetDetailViewCommand"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="dataset">The dataset.</param>
        public ShowDatasetDetailViewCommand(IContextAware context, IEventAggregator eventAggregator, DatasetFacade dataset)
            : base(obj => OnShow(context, eventAggregator, dataset), obj => CanShow(dataset))
        {
        }
    }
}
