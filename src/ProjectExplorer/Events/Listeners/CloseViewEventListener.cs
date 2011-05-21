//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using Apollo.UI.Common;
using Autofac;
using Microsoft.Practices.Prism.Regions;

namespace Apollo.ProjectExplorer.Events.Listeners
{
    /// <summary>
    /// An <see cref="EventListener"/> which is responsible for closing views.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Instantiated by the IOC container.")]
    internal sealed class CloseViewEventListener : EventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseViewEventListener"/> class.
        /// </summary>
        /// <param name="container">The IOC container.</param>
        public CloseViewEventListener(IContainer container)
            : base(container)
        {
        }

        /// <summary>
        /// Allows derivative classes to subscribe to different events.
        /// </summary>
        protected override void Subscribe()
        {
            EventAggregator.GetEvent<CloseViewEvent>().Subscribe(ShowView);
        }

        /// <summary>
        /// Shows the selected view.
        /// </summary>
        /// <param name="request">The request which indicates which view to show.</param>
        private void ShowView(CloseViewRequest request)
        {
            var region = null as IRegion;
            var regionManager = request.RegionManager ?? MainRegionManager;
            
            // If the view already exists, re-activate it instead - this way we only resolve and initialise 
            // the presenter if we actually need it.
            if (regionManager.Regions.ContainsRegionWithName(request.RegionName))
            {
                region = regionManager.Regions[request.RegionName];
                if (region.HasViewByParameter(request.Parameter))
                {
                    var view = region.GetViewByParameter(request.Parameter);
                    var viewWindow = view as Window;
                    if (viewWindow != null)
                    {
                        viewWindow.Close();
                    }
                    else 
                    {
                        if (region == null)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.InvariantCulture, 
                                    "The region '{0}' does not exist.", 
                                    request.RegionName));
                        }

                        region.Deactivate(view);
                    }
                }
            }
        }
    }
}
