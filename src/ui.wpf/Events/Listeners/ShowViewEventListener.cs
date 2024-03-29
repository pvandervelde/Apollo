﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using Apollo.Utilities;
using Microsoft.Practices.Prism.Regions;

namespace Apollo.UI.Wpf.Events.Listeners
{
    /// <summary>
    /// An <see cref="EventListener"/> which is responsible for showing views.
    /// </summary>
    public class ShowViewEventListener : EventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowViewEventListener"/> class.
        /// </summary>
        /// <param name="container">The IOC container.</param>
        /// <param name="dispatcherContext">The dispatcher context.</param>
        public ShowViewEventListener(IDependencyInjectionProxy container, IContextAware dispatcherContext)
            : base(container, dispatcherContext)
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
            Action action = () => ShowViewInternal(request);
            if (DispatcherContext.IsSynchronized)
            {
                action();
            }
            else
            {
                DispatcherContext.Invoke(action);
            }
        }

        private void ShowViewInternal(ShowViewRequest request)
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

            var presenter = (IPresenter)Container.Resolve(request.PresenterType);
            var view = Container.Resolve(presenter.ViewType);
            presenter.Initialize(view, request.Parameter);
            RegionManager.SetRegionManager((DependencyObject)view, regionManager);

            var viewWindow = view as Window;
            if (viewWindow != null)
            {
                Action showWindow = 
                    () =>
                    {
                        var mainWindow = Application.Current.MainWindow;
                        if (!ReferenceEquals(mainWindow, viewWindow))
                        {
                            if (request.IsModal)
                            {
                                viewWindow.Owner = Application.Current.MainWindow;
                                viewWindow.ShowDialog();
                                viewWindow.Owner = null;
                            }
                            else
                            {
                                viewWindow.Show();
                            }
                        }
                        else
                        {
                            viewWindow.Show();
                        }
                    };
                if (viewWindow.Dispatcher.CheckAccess())
                {
                    showWindow();
                }
                else
                {
                    viewWindow.Dispatcher.Invoke(showWindow);
                }
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

                region.AddAndActivateWithParameter(view, request.Parameter);
            }
        }
    }
}
