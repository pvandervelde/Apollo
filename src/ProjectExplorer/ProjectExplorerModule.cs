//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using Apollo.Core.Host;
using Apollo.Core.Host.UserInterfaces.Application;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.ProjectExplorer.Views.Menu;
using Apollo.ProjectExplorer.Views.Shell;
using Apollo.ProjectExplorer.Views.StatusBar;
using Apollo.UI.Common;
using Apollo.UI.Common.Events;
using Apollo.UI.Common.Listeners;
using Apollo.UI.Common.Views.Datasets;
using Apollo.UI.Common.Views.Feedback;
using Apollo.UI.Common.Views.Progress;
using Apollo.UI.Common.Views.Projects;
using Apollo.UI.Common.Views.Scripting;
using Apollo.Utilities;
using Apollo.Utilities.ExceptionHandling;
using Autofac;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using NSarrac.Framework;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// Defines a <see cref="IModule"/> which handles the registrations for 
    /// the UI portion of the ProjectExplorer application.
    /// </summary>
    /// <remarks>
    /// Note that this module only contains the UI controls and UI related classes,
    /// e.g. ViewModels etc.. All core related elements need to be injected via 
    /// the <see cref="KernelBootstrapper"/>.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    internal sealed class ProjectExplorerModule : IModule
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "projectexplorer.error.log";

        /// <summary>
        /// The IOC container that holds the references.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// The reset event that is used to signal the application that it is safe to shut down.
        /// </summary>
        private readonly AutoResetEvent m_ResetEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorerModule"/> class.
        /// </summary>
        /// <param name="container">The IOC container that will hold the references.</param>
        /// <param name="resetEvent">The reset event that is used to signal the application that it is safe to shut down.</param>
        public ProjectExplorerModule(IContainer container, AutoResetEvent resetEvent)
        {
            {
                Debug.Assert(container != null, "The container should exist.");
                Debug.Assert(resetEvent != null, "The reset event should exist.");
            }

            m_Container = container;
            m_ResetEvent = resetEvent;
        }

        #region Implementation of IModule

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            UpdateContainer();

            InitializeModels();

            RegisterNotifications();

            ActivateRegions();
        }

        /// <summary>
        /// Updates the existing container with the additional UI references.
        /// </summary>
        /// <remarks>
        /// Note that this method only adds references to UI controls and UI
        /// related classes / interfaces. Additional references for the 
        /// core needs to be added via a different path.
        /// </remarks>
        /// <seealso cref="UserInterfaceBootstrapper"/>
        /// <seealso cref="KernelBootstrapper"/>
        private void UpdateContainer()
        {
            var builder = new ContainerBuilder();
            {
                // Register the utilities elements. These are 'shared' with the core
                builder.RegisterModule(new UtilitiesModule());
                builder.RegisterModule(new CommonUIModule());

                // Get all the registrations from Apollo.UI.Common
                var commonUiAssembly = typeof(Observable).Assembly;
                builder.RegisterAssemblyTypes(commonUiAssembly)
                   .Where(t => t.FullName.EndsWith("Presenter", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                   .InstancePerDependency()
                   .PropertiesAutowired();
                builder.RegisterAssemblyTypes(commonUiAssembly)
                    .Where(
                        t => (t.FullName.EndsWith("View", StringComparison.Ordinal) || t.FullName.EndsWith("Window", StringComparison.Ordinal))
                            && t.IsClass 
                            && !t.IsAbstract)
                    .InstancePerDependency()
                    .AsImplementedInterfaces();
                builder.RegisterAssemblyTypes(commonUiAssembly)
                    .Where(t => t.FullName.EndsWith("Command", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .InstancePerDependency();
                builder.RegisterAssemblyTypes(commonUiAssembly)
                    .Where(t => t.FullName.EndsWith("EventListener", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .SingleInstance();
                builder.RegisterAssemblyTypes(commonUiAssembly)
                    .Where(t => t.FullName.EndsWith("Command", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .InstancePerDependency();

                // Get the registrations from the current assembly
                var localAssembly = GetType().Assembly;
                builder.RegisterAssemblyTypes(localAssembly)
                    .Where(t => t.FullName.EndsWith("Presenter", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .InstancePerDependency()
                    .PropertiesAutowired();
                builder.RegisterAssemblyTypes(localAssembly)
                    .Where(
                        t => (t.FullName.EndsWith("View", StringComparison.Ordinal) || t.FullName.EndsWith("Window", StringComparison.Ordinal)) 
                            && t.IsClass 
                            && !t.IsAbstract)
                    .InstancePerDependency()
                    .AsImplementedInterfaces();
                builder.RegisterAssemblyTypes(localAssembly)
                    .Where(t => t.FullName.EndsWith("Command", StringComparison.Ordinal) && t.IsClass && !t.IsAbstract)
                    .InstancePerDependency();

                builder.Register(c => new DispatcherContextWrapper(Application.Current.Dispatcher))
                    .As<IContextAware>();
                
                var key = SrcOnlyExceptionHandlingUtillities.ReportingPublicKey();
                builder.RegisterModule(new FeedbackReportingModule(() => key));
            }

            builder.Update(m_Container);
        }

        private void InitializeModels()
        {
            SelectScriptLanguageModel.StoreKnownLanguages(m_Container.Resolve<IContextAware>());
        }

        private void RegisterNotifications()
        {
            // Set the shutdown action
            var notificationNames = m_Container.Resolve<INotificationNameConstants>();
            var applicationFacade = m_Container.Resolve<IAbstractApplications>();
            {
                applicationFacade.RegisterNotification(
                    notificationNames.SystemShuttingDown,
                    obj =>
                    {
                        // Shut down is rather yucky. It turns out that
                        // app.Shutdown() is async (the documentation doesn't tell you that)
                        // so we have to find a way to wait for it to be done. Enter yuckiness.
                        var app = (App)Application.Current;
                        Action action =
                            () =>
                            {
                                m_ResetEvent.WaitOne();
                                app.Shutdown();
                            };

                        app.Dispatcher.BeginInvoke(action);
                    });
            }
        }

        private void ActivateRegions()
        {
            m_Container.Resolve<ShowViewEventListener>().Start();
            m_Container.Resolve<CloseViewEventListener>().Start();

            m_Container.Resolve<IEventAggregator>()
                .GetEvent<ShowViewEvent>()
                .Publish(
                    new ShowViewRequest(
                        typeof(ShellPresenter), 
                        RegionNames.Shell, 
                        new ShellParameter(m_Container.Resolve<IContextAware>())));
            m_Container.Resolve<IEventAggregator>()
                .GetEvent<ShowViewEvent>()
                .Publish(
                    new ShowViewRequest(
                        typeof(MenuPresenter), 
                        RegionNames.MainMenu, 
                        new MenuParameter(m_Container.Resolve<IContextAware>())));
            m_Container.Resolve<IEventAggregator>()
                .GetEvent<ShowViewEvent>()
                .Publish(
                    new ShowViewRequest(
                        typeof(StatusBarPresenter),
                        RegionNames.StatusBar,
                        new StatusBarParameter(m_Container.Resolve<IContextAware>())));

            m_Container.Resolve<IEventAggregator>()
               .GetEvent<ShowViewEvent>()
               .Publish(
                   new ShowViewRequest(
                       typeof(ErrorReportsPresenter),
                       CommonRegionNames.StatusBarErrorReport,
                       new ErrorReportsParameter(m_Container.Resolve<IContextAware>())));

            m_Container.Resolve<IEventAggregator>()
              .GetEvent<ShowViewEvent>()
              .Publish(
                  new ShowViewRequest(
                      typeof(FeedbackPresenter),
                      CommonRegionNames.StatusBarFeedback,
                      new FeedbackParameter(m_Container.Resolve<IContextAware>())));

            m_Container.Resolve<IEventAggregator>()
              .GetEvent<ShowViewEvent>()
              .Publish(
                  new ShowViewRequest(
                      typeof(ProgressPresenter),
                      CommonRegionNames.StatusBarProgressReport,
                      new ProgressParameter(m_Container.Resolve<IContextAware>())));

            ActivateProjectRegions();
        }

        private void ActivateProjectRegions()
        {
            var context = m_Container.Resolve<IContextAware>();
            var projectFacade = m_Container.Resolve<ILinkToProjects>();
            projectFacade.OnNewProjectLoaded +=
                (s, e) =>
                {
                    m_Container.Resolve<IEventAggregator>().GetEvent<ShowViewEvent>().Publish(
                        new ShowViewRequest(
                            typeof(ProjectPresenter),
                            CommonRegionNames.Content,
                            new ProjectParameter(context)));

                    m_Container.Resolve<IEventAggregator>().GetEvent<ShowViewEvent>().Publish(
                        new ShowViewRequest(
                            typeof(ProjectDescriptionPresenter),
                            CommonRegionNames.ProjectViewTopPane,
                            new ProjectDescriptionParameter(context)));

                    m_Container.Resolve<IEventAggregator>().GetEvent<ShowViewEvent>().Publish(
                        new ShowViewRequest(
                            typeof(DatasetGraphPresenter),
                            CommonRegionNames.ProjectViewContent,
                            new DatasetGraphParameter(context)));
                };

            projectFacade.OnProjectUnloaded +=
                (s, e) =>
                {
                    m_Container.Resolve<IEventAggregator>().GetEvent<CloseViewEvent>().Publish(
                        new CloseViewRequest(
                            CommonRegionNames.ProjectViewTopPane,
                            new ProjectDescriptionParameter(context)));

                    m_Container.Resolve<IEventAggregator>().GetEvent<CloseViewEvent>().Publish(
                        new CloseViewRequest(
                            CommonRegionNames.ProjectViewContent,
                            new DatasetGraphParameter(context)));

                    m_Container.Resolve<IEventAggregator>().GetEvent<CloseViewEvent>().Publish(
                        new CloseViewRequest(
                            CommonRegionNames.Content,
                            new ProjectParameter(context)));
                };
        }

        #endregion
    }
}
