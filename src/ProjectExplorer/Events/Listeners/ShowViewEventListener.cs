//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using Apollo.UI.Common;
using Apollo.UI.Common.Eventing;
using Autofac;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Regions;

namespace Apollo.ProjectExplorer.Events.Listeners
{
    /// <summary>
    /// An <see cref="EventListener"/> which is responsible for showing views.
    /// </summary>
    internal class ShowViewEventListener : EventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowViewEventListener"/> class.
        /// </summary>
        /// <param name="container">The IOC container.</param>
        public ShowViewEventListener(IContainer container) : base(container)
        {
        }

        /// <summary>
        /// Allows derivative classes to subscribe to different events.
        /// </summary>
        protected override void Subscribe()
        {
            EventAggregator.GetEvent<ShowViewEvent>().Subscribe(ShowView);
        }

        /// <summary>
        /// Shows the selected view.
        /// </summary>
        /// <param name="request">The request which indicates which view to show.</param>
        private void ShowView(ShowViewRequest request)
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
                    region.ActivateByParameter(request.Parameter);
                    return;
                }
            }

            // Resolve the presenter and view
            var presenter = (IPresenter)Container.Resolve(request.PresenterType);
            var view = Container.Resolve(presenter.ViewType);
            presenter.Initialize(view, request.Parameter);
            RegionManager.SetRegionManager((DependencyObject) view, regionManager);

            // Show the view
            var viewWindow = view as Window;
            if (viewWindow != null)
            {
                viewWindow.Show();
            }
            else
            {
                if (region == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The region '{0}' does not exist.", request.RegionName));
                }

                region.AddAndActivateWithParameter(view, request.Parameter);
            }
        }
    }
}
