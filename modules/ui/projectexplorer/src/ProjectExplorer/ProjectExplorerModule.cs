﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Apollo.Core;
using Apollo.Core.UserInterfaces.Application;
using Apollo.ProjectExplorer.Events;
using Apollo.ProjectExplorer.Events.Listeners;
using Apollo.ProjectExplorer.Views.Menu;
using Apollo.ProjectExplorer.Views.Shell;
using Apollo.UI.Common;
using Autofac;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Modularity;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// Defines a <see cref="IModule"/> which handles the registrations for 
    /// the ProjectExplorer application.
    /// </summary>
    internal sealed class ProjectExplorerModule : IModule
    {
        /// <summary>
        /// The IOC container that holds the references.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorerModule"/> class.
        /// </summary>
        /// <param name="container">The IOC container that will hold the references.</param>
        public ProjectExplorerModule(IContainer container)
        {
            {
                Debug.Assert(container != null, "The container should exist.");
            }

            m_Container = container;
        }

        #region Implementation of IModule

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            // Get all the registrations from Apollo.UI.Common
            var commonUiAssembly = typeof(Observable).Assembly;
            m_Container.Configure(c => c.RegisterAssemblyTypes(commonUiAssembly)
                                           .Where(t => t.FullName.EndsWith("Command") && t.IsClass && !t.IsAbstract)
                                           .InstancePerDependency());

            var localAssembly = GetType().Assembly;
            m_Container.Configure(c => c.RegisterAssemblyTypes(localAssembly)
                                           .Where(t => t.FullName.EndsWith("Presenter") && t.IsClass && !t.IsAbstract)
                                           .InstancePerDependency()
                                           .PropertiesAutowired());
            m_Container.Configure(c => c.RegisterAssemblyTypes(localAssembly)
                                           .Where(t => (t.FullName.EndsWith("View") || t.FullName.EndsWith("Window")) && t.IsClass && !t.IsAbstract)
                                           .InstancePerDependency()
                                           .AsImplementedInterfaces());
            m_Container.Configure(c => c.RegisterAssemblyTypes(localAssembly)
                                           .Where(t => t.FullName.EndsWith("EventListener") && t.IsClass && !t.IsAbstract)
                                           .SingleInstance());
            m_Container.Configure(c => c.RegisterAssemblyTypes(localAssembly)
                                           .Where(t => t.FullName.EndsWith("Command") && t.IsClass && !t.IsAbstract)
                                           .InstancePerDependency());

            // Register the regions
            m_Container.Resolve<ShowViewEventListener>().Start();
            m_Container.Resolve<IEventAggregator>().GetEvent<ShowViewEvent>().Publish(new ShowViewRequest(typeof(ShellPresenter), "Shell", new ShellParameter()));
            m_Container.Resolve<IEventAggregator>().GetEvent<ShowViewEvent>().Publish(new ShowViewRequest(typeof(MenuPresenter), "Menu", new MenuParameter()));

            // Set the shutdown action
            var notificationNames = m_Container.Resolve<INotificationNameConstants>();
            var applicationFacade = m_Container.Resolve<IAbstractApplications>();
            {
                applicationFacade.RegisterNotification(
                    notificationNames.Shutdown, 
                    obj => 
                        {
                            var app = Application.Current;
                            if (app.Dispatcher.CheckAccess())
                            {
                                app.Shutdown();
                            }
                            else
                            {
                                app.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(app.Shutdown));
                            }
                        });
            }
        }

        #endregion
    }
}