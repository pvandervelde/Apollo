//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Wpf;
using Apollo.UI.Wpf.Events;
using Apollo.UI.Wpf.Views.Projects;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Explorer.Commands
{
    /// <summary>
    /// Handles the showing of the project tab.
    /// </summary>
    internal sealed class ShowProjectsTabCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the project tab can be opened.
        /// </summary>
        /// <param name="projectFacade">
        /// The object that contains the methods that allow interaction
        /// with the project system.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the tab can be opened; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanShowTab(ILinkToProjects projectFacade)
        {
            // If there is no facade then we're in design mode or something
            // else weird.
            if (projectFacade == null)
            {
                return false;
            }

            return projectFacade.HasActiveProject();
        }

        /// <summary>
        /// Called when the command is executed.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        private static void ShowTab(IContextAware context, IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ShowViewEvent>().Publish(
                new ShowViewRequest(
                    typeof(ProjectPresenter),
                    CommonRegionNames.Content,
                    new ProjectParameter(context)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowProjectsTabCommand"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="projectFacade">
        ///     The object that contains the methods that allow interaction
        ///     with the project system.
        /// </param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ShowProjectsTabCommand(IContextAware context, ILinkToProjects projectFacade, IEventAggregator eventAggregator)
            : base(obj => ShowTab(context, eventAggregator), obj => CanShowTab(projectFacade))
        {
        }
    }
}
