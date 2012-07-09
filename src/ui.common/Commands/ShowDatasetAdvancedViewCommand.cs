//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the showing of the dataset advanced view.
    /// </summary>
    public sealed class ShowDatasetAdvancedViewCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the advanced view can be shown for the dataset.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <returns>
        /// <see langword="true" /> if the advanced view can be shown; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanShow(DatasetFacade dataset)
        {
            return false;
        }

        /// <summary>
        /// Shows the advanced view for the dataset.
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

            // For now we do nothing. At some point we'll have to figure out what comes next.
            return;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowDatasetAdvancedViewCommand"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="dataset">The dataset.</param>
        public ShowDatasetAdvancedViewCommand(IContextAware context, IEventAggregator eventAggregator, DatasetFacade dataset)
            : base(obj => OnShow(context, eventAggregator, dataset), obj => CanShow(dataset))
        {
        }
    }
}
