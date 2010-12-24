//-----------------------------------------------------------------------
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
using Apollo.ProjectExplorer.Views.Content;
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
            var builder = new ContainerBuilder();
            {
                var commonUiAssembly = typeof(Observable).Assembly;
                builder.RegisterAssemblyTypes(commonUiAssembly)
                    .Where(t => t.FullName.EndsWith("Command", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .InstancePerDependency();

                var localAssembly = GetType().Assembly;
                builder.RegisterAssemblyTypes(localAssembly)
                    .Where(t => t.FullName.EndsWith("Presenter", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .InstancePerDependency()
                    .PropertiesAutowired();
                builder.RegisterAssemblyTypes(localAssembly)
                    .Where(t => (t.FullName.EndsWith("View", StringComparison.Ordinal) || t.FullName.EndsWith("Window", StringComparison.Ordinal)) && t.IsClass && !t.IsAbstract)
                    .InstancePerDependency()
                    .AsImplementedInterfaces();
                builder.RegisterAssemblyTypes(localAssembly)
                    .Where(t => t.FullName.EndsWith("EventListener", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .SingleInstance();
                builder.RegisterAssemblyTypes(localAssembly)
                    .Where(t => t.FullName.EndsWith("Command", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .InstancePerDependency();
            }

            builder.Update(m_Container);

            // Register the regions
            m_Container.Resolve<ShowViewEventListener>().Start();
            m_Container.Resolve<IEventAggregator>().GetEvent<ShowViewEvent>().Publish(new ShowViewRequest(typeof(ShellPresenter), RegionNames.Shell, new ShellParameter()));
            m_Container.Resolve<IEventAggregator>().GetEvent<ShowViewEvent>().Publish(new ShowViewRequest(typeof(MenuPresenter), RegionNames.MainMenu, new MenuParameter()));

            // Set the shutdown action
            var notificationNames = m_Container.Resolve<INotificationNameConstants>();
            var applicationFacade = m_Container.Resolve<IAbstractApplications>();
            {
                applicationFacade.RegisterNotification(
                    notificationNames.SystemShuttingDown,
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
