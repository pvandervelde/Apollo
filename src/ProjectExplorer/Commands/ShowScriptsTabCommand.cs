//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.ProjectExplorer.Events;
using Apollo.UI.Common.Views.Scripting;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.ProjectExplorer.Commands
{
    /// <summary>
    /// Handles the showing of the project tab.
    /// </summary>
    internal sealed class ShowScriptsTabCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the script tab can be opened.
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
            // Can always open a new script
            return true;
        }

        /// <summary>
        /// Called when the command is executed.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        private static void ShowTab(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ShowViewEvent>().Publish(
                new ShowViewRequest(
                    typeof(ScriptPresenter),
                    RegionNames.Content,
                    new ScriptParameter()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowScriptsTabCommand"/> class.
        /// </summary>
        /// <param name="projectFacade">
        ///     The object that contains the methods that allow interaction
        ///     with the project system.
        /// </param>
        /// <param name="eventAggregator">The event aggregator.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "The constructor is called via the IOC container.")]
        public ShowScriptsTabCommand(ILinkToProjects projectFacade, IEventAggregator eventAggregator)
            : base(obj => ShowTab(eventAggregator), obj => CanShowTab(projectFacade))
        {
        }
    }
}
