//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Apollo.UI.Common.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> that closes a given view.
    /// </summary>
    public sealed class CloseViewCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines if the view should be closed.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the view should be closed; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanCloseView()
        {
            return true;
        }

        /// <summary>
        /// Closes the given view.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="regionName">The name of the region which must be closed.</param>
        /// <param name="viewParameter">The parameter which was used to create the view.</param>
        private static void OnCloseView(IEventAggregator eventAggregator, string regionName, Parameter viewParameter)
        {
            eventAggregator.GetEvent<CloseViewEvent>().Publish(
                new CloseViewRequest(
                    regionName,
                    viewParameter));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseViewCommand"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="regionName">The name of the region which must be closed.</param>
        /// <param name="viewParameter">The parameter which was used to create the view.</param>
        public CloseViewCommand(IEventAggregator eventAggregator, string regionName, Parameter viewParameter)
            : base(obj => OnCloseView(eventAggregator, regionName, viewParameter), obj => CanCloseView())
        { 
        }
    }
}
