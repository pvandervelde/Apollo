//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Explorer.Views.About;
using Apollo.UI.Wpf;
using Apollo.UI.Wpf.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Explorer.Commands
{
    /// <summary>
    /// Handles the showing of the About window.
    /// </summary>
    internal sealed class ShowAboutWindowCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Called when the command is executed.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        private static void OnExecute(IContextAware context, IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ShowViewEvent>().Publish(
                new ShowViewRequest(
                    typeof(AboutPresenter), 
                    "AboutWindow", 
                    new AboutParameter(context),
                    null,
                    true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAboutWindowCommand"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ShowAboutWindowCommand(IContextAware context, IEventAggregator eventAggregator)
            : base(obj => OnExecute(context, eventAggregator))
        {
        }
    }
}
