﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.ProjectExplorer.Events;
using Apollo.ProjectExplorer.Views.About;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.ProjectExplorer.Commands
{
    /// <summary>
    /// Handles the showing of the About window.
    /// </summary>
    internal sealed class ShowAboutWindowCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Called when the command is executed.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        private static void OnExecute(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ShowViewEvent>().Publish(new ShowViewRequest(typeof(AboutPresenter), "AboutWindow", new AboutParameter()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAboutWindowCommand"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "The constructor is called via the IOC container.")]
        public ShowAboutWindowCommand(IEventAggregator eventAggregator)
            : base(obj => OnExecute(eventAggregator))
        {
        }
    }
}
