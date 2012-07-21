//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Common.Events;
using Apollo.UI.Common.Views.Scenes;
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
        /// <param name="scene">The scene data.</param>
        /// <returns>
        /// <see langword="true" /> if the advanced view can be shown; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanShow(SceneFacade scene)
        {
            return false;
        }

        /// <summary>
        /// Shows the advanced view for the dataset.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="scene">The scene data.</param>
        private static void OnShow(IContextAware context, IEventAggregator eventAggregator, SceneFacade scene)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (scene == null)
            {
                return;
            }

            eventAggregator.GetEvent<ShowViewEvent>().Publish(
                new ShowViewRequest(
                    typeof(ScenePresenter),
                    CommonRegionNames.Content,
                    new SceneParameter(context, scene)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowDatasetAdvancedViewCommand"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="scene">The scene data.</param>
        public ShowDatasetAdvancedViewCommand(IContextAware context, IEventAggregator eventAggregator, SceneFacade scene)
            : base(obj => OnShow(context, eventAggregator, scene), obj => CanShow(scene))
        {
        }
    }
}
