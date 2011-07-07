//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Common;
using Apollo.UI.Common.Events;
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
        /// <returns>
        /// <see langword="true" /> if the tab can be opened; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanShowTab()
        {
            // Can always open a new script
            return true;
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
                    typeof(ScriptPresenter),
                    CommonRegionNames.Content,
                    new ScriptParameter(context)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowScriptsTabCommand"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ShowScriptsTabCommand(IContextAware context, IEventAggregator eventAggregator)
            : base(obj => ShowTab(context, eventAggregator), obj => CanShowTab())
        {
        }
    }
}
