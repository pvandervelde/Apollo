//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Apollo.Core.Host;
using Apollo.Core.Host.UserInterfaces.Application;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Explorer.Nuclei;
using Apollo.UI.Explorer.Nuclei.ExceptionHandling;
using Apollo.UI.Explorer.Properties;
using Apollo.UI.Explorer.Views.Menu;
using Apollo.UI.Explorer.Views.Shell;
using Apollo.UI.Explorer.Views.StatusBar;
using Apollo.UI.Explorer.Views.Welcome;
using Apollo.UI.Wpf;
using Apollo.UI.Wpf.Events;
using Apollo.UI.Wpf.Events.Listeners;
using Apollo.UI.Wpf.Views.Datasets;
using Apollo.UI.Wpf.Views.Feedback;
using Apollo.UI.Wpf.Views.Notification;
using Apollo.UI.Wpf.Views.Profiling;
using Apollo.UI.Wpf.Views.Progress;
using Apollo.UI.Wpf.Views.Projects;
using Apollo.UI.Wpf.Views.Scripting;
using Autofac;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using NSarrac.Framework;
using Nuclei.Configuration;

namespace Apollo.UI.Explorer
{
    /// <summary>
    /// Defines a <see cref="IModule"/> which handles the registrations for 
    /// the UI portion of the Explorer application.
    /// </summary>
    /// <remarks>
    /// Note that this module only contains the UI controls and UI related classes,
    /// e.g. ViewModels etc.. All core related elements need to be injected via 
    /// the <see cref="KernelBootstrapper"/>.
    /// </remarks>
    internal sealed class ExplorerModule : IModule
    {
        /// <summary>
        /// The IOC container that holds the references.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// The reset event that is used to signal the application that it is safe to shut down.
        /// </summary>
        private readonly AutoResetEvent m_ResetEvent;

        /// <summary>
        /// A flag that indicates if the application is profiling itself.
        /// </summary>
        private readonly bool m_IsProfiling;

        /// <summary>
        /// A flag that indicates if the application should show the welcome page on start-up.
        /// </summary>
        private readonly bool m_ShowWelcomePage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorerModule"/> class.
        /// </summary>
        /// <param name="container">The IOC container that will hold the references.</param>
        /// <param name="resetEvent">The reset event that is used to signal the application that it is safe to shut down.</param>
        public ExplorerModule(IContainer container, AutoResetEvent resetEvent)
        {
            {
                Debug.Assert(container != null, "The container should exist.");
                Debug.Assert(resetEvent != null, "The reset event should exist.");
            }

            m_Container = container;
            m_ResetEvent = resetEvent;
            m_IsProfiling = ConfigurationHelpers.ShouldBeProfiling();
            m_ShowWelcomePage = Settings.Default.ShowWelcomePageOnStartup;
        }

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

                // Get all the registrations from Apollo.UI.Wpf
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

                var key = SrcOnlyExceptionHandlingUtilities.ReportingPublicKey();
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
                                // First nuke all the UI stuff
                                m_Container.Dispose();

                                // Now wait for the kernel to shut down.
                                m_ResetEvent.WaitOne();

                                // And then kill the app.
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

            // Get the aggregator and the show view event once so that
            // we don't keep hitting the container over and over.
            var aggregator = m_Container.Resolve<IEventAggregator>();
            var showViewEvent = aggregator.GetEvent<ShowViewEvent>();
            showViewEvent.Publish(
                new ShowViewRequest(
                    typeof(ShellPresenter),
                    RegionNames.Shell,
                    new ShellParameter(m_Container.Resolve<IContextAware>())));

            showViewEvent.Publish(
                new ShowViewRequest(
                    typeof(MenuPresenter),
                    RegionNames.MainMenu,
                    new MenuParameter(m_Container.Resolve<IContextAware>())));

            showViewEvent.Publish(
                new ShowViewRequest(
                    typeof(StatusBarPresenter),
                    RegionNames.StatusBar,
                    new StatusBarParameter(m_Container.Resolve<IContextAware>())));

            showViewEvent.Publish(
                new ShowViewRequest(
                    typeof(ErrorReportsPresenter),
                    CommonRegionNames.StatusBarErrorReport,
                    new ErrorReportsParameter(m_Container.Resolve<IContextAware>())));

            showViewEvent.Publish(
                new ShowViewRequest(
                    typeof(FeedbackPresenter),
                    CommonRegionNames.StatusBarFeedback,
                    new FeedbackParameter(m_Container.Resolve<IContextAware>())));

            showViewEvent.Publish(
                new ShowViewRequest(
                    typeof(NotificationPresenter),
                    CommonRegionNames.StatusBarStatusText,
                    new NotificationParameter(m_Container.Resolve<IContextAware>())));

            showViewEvent.Publish(
                new ShowViewRequest(
                    typeof(ProgressPresenter),
                    CommonRegionNames.StatusBarProgressReport,
                    new ProgressParameter(m_Container.Resolve<IContextAware>())));

            // Only add the necessary controls if we are actually profiling. If we are not
            // profiling then we don't add any controls and anything that we did allocate
            // just goes the way of the dodo
            if (m_IsProfiling)
            {
                showViewEvent.Publish(
                    new ShowViewRequest(
                        typeof(ProfilePresenter),
                        CommonRegionNames.StatusBarProfilerReport,
                        new ProfileParameter(m_Container.Resolve<IContextAware>())));
            }

            if (m_ShowWelcomePage)
            {
                showViewEvent.Publish(
                    new ShowViewRequest(
                        typeof(WelcomePresenter),
                        CommonRegionNames.Content,
                        new WelcomeParameter(m_Container.Resolve<IContextAware>())));
            }

            ActivateProjectRegions();
        }

        private void ActivateProjectRegions()
        {
            var context = m_Container.Resolve<IContextAware>();
            var projectFacade = m_Container.Resolve<ILinkToProjects>();
            projectFacade.OnNewProjectLoaded +=
                (s, e) =>
                {
                    // Get the aggregator and the show view event once so that
                    // we don't keep hitting the container over and over.
                    var aggregator = m_Container.Resolve<IEventAggregator>();
                    var showViewEvent = aggregator.GetEvent<ShowViewEvent>();
                    showViewEvent.Publish(
                        new ShowViewRequest(
                            typeof(ProjectPresenter),
                            CommonRegionNames.Content,
                            new ProjectParameter(context)));

                    showViewEvent.Publish(
                        new ShowViewRequest(
                            typeof(ProjectDescriptionPresenter),
                            CommonRegionNames.ProjectViewTopPane,
                            new ProjectDescriptionParameter(context)));

                    showViewEvent.Publish(
                        new ShowViewRequest(
                            typeof(DatasetGraphPresenter),
                            CommonRegionNames.ProjectViewContent,
                            new DatasetGraphParameter(context)));
                };

            projectFacade.OnProjectUnloaded +=
                (s, e) =>
                {
                    // Get the aggregator and the close view event once so that
                    // we don't keep hitting the container over and over.
                    var aggregator = m_Container.Resolve<IEventAggregator>();
                    var closeViewEvent = aggregator.GetEvent<CloseViewEvent>();

                    closeViewEvent.Publish(
                        new CloseViewRequest(
                            CommonRegionNames.ProjectViewTopPane,
                            new ProjectDescriptionParameter(context)));

                    closeViewEvent.Publish(
                        new CloseViewRequest(
                            CommonRegionNames.ProjectViewContent,
                            new DatasetGraphParameter(context)));

                    closeViewEvent.Publish(
                        new CloseViewRequest(
                            CommonRegionNames.Content,
                            new ProjectParameter(context)));
                };
        }
    }
}
