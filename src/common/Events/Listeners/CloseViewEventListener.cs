//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using Apollo.UI.Common.Events;
using Autofac;
using Microsoft.Practices.Prism.Regions;

namespace Apollo.UI.Common.Listeners
{
    /// <summary>
    /// An <see cref="EventListener"/> which is responsible for closing views.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class CloseViewEventListener : EventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseViewEventListener"/> class.
        /// </summary>
        /// <param name="container">The IOC container.</param>
        /// <param name="dispatcherContext">The dispatcher context.</param>
        public CloseViewEventListener(IContainer container, IContextAware dispatcherContext)
            : base(container, dispatcherContext)
        {
        }

        /// <summary>
        /// Allows derivative classes to subscribe to different events.
        /// </summary>
        protected override void Subscribe()
        {
            EventAggregator.GetEvent<CloseViewEvent>().Subscribe(CloseView);
        }

        /// <summary>
        /// Closes the selected view.
        /// </summary>
        /// <param name="request">The request which indicates which view to close.</param>
        private void CloseView(CloseViewRequest request)
        {
            Action action = () => CloseViewInternal(request);
            if (DispatcherContext.IsSynchronized)
            {
                action();
            }
            else
            {
                DispatcherContext.Invoke(action);
            }
        }

        private void CloseViewInternal(CloseViewRequest request)
        {
            var region = null as IRegion;
            var regionManager = request.RegionManager ?? MainRegionManager;

            // Only deactivate the view if it exists
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

                        region.Remove(view);
                    }
                }
            }
        }
    }
}
