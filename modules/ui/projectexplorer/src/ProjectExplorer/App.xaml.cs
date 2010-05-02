//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Threading;
using Apollo.Utils;
using Autofac;
using Autofac.Core;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    internal partial class App
    {
        /// <summary>
        /// Initializes the environment for use. Currently sets Environment Variables and 
        /// creates directories.
        /// </summary>
        private static void InitializeEnvironment()
        {
            // Set the internal logging for NLog
            //Environment.SetEnvironmentVariable("NLOG_INTERNAL_LOG_FILE", @"d:\temp\nloginternal.log");
            //Environment.SetEnvironmentVariable("NLOG_INTERNAL_LOG_LEVEL", "Debug");
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.</param>
        private void HandleApplicationStartup(object sender, StartupEventArgs e)
        {
            InitializeEnvironment();

            var userInterfaceModule = LoadKernel();
            LoadUserInterface(userInterfaceModule);
        }

        /// <summary>
        /// Loads the kernel.
        /// </summary>
        /// <returns>
        /// The module which contains the Dependency Injection 
        /// registrations for the user interface connection to the
        /// kernel.
        /// </returns>
        private IModule LoadKernel()
        {
            IModule userInterfaceModule = null;

            // At a later stage we need to clean this up.
            // there are two constants and a DI reference.
            var progressTracker = new TimeBasedProgressTracker(
                -1, 
                new TimeSpan(0, 0, 0, 0, 500),
                new StartupTimeStorage());

            var bootstrapper = new KernelBootstrapper(
                new BootstrapperStartInfo(),
                () => new MockExceptionHandler(),
                progressTracker,
                module => { userInterfaceModule = module; });

            // Load the kernel
            bootstrapper.Load();

            // Return the DI module that holds the
            // kernel UI links
            return userInterfaceModule;
        }

        /// <summary>
        /// Loads the user interface.
        /// </summary>
        /// <param name="userInterfaceModule">The user interface module.</param>
        private void LoadUserInterface(IModule userInterfaceModule)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(userInterfaceModule);
            }

            var container = builder.Build();
            var bootstrapper = new UserInterfaceBootstrapper(container);
            bootstrapper.Run();

            // Start the user interface
            //var mainView = container.Resolve<IShellView>();
            //mainView.ShowView();

            // Set the main window and the shutdown method
            // In theory the main window should already be set
            // correctly, however should we ever change the
            // order in which windows are created / loaded
            // then we still have the correct main window.
            //var mainWindow = mainView as Window;
            //{
                //Debug.Assert(mainWindow != null, "The shell should also be a window.");
            //}

            //MainWindow = mainWindow;

        }

        /// <summary>
        /// Called when an unhandled exception takes place.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Threading.DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void HandleUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Unhandled exceptions go here..
            // Refer to the unhandled exception handler --> nSarrac to the rescue

            // Indicate that we have handled the exception.
            e.Handled = true;

            // Bail!
            // Exit-Fast
        }

        /// <summary>
        /// Called when the application is about to shut down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.ExitEventArgs"/> instance containing the event data.</param>
        private void HandleApplicationShutdown(object sender, ExitEventArgs e)
        {
            // At this point we are committed to a shutdown. There is
            // no way to stop it anymore.
        }

        /// <summary>
        /// Called when the windows session is about to end.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.SessionEndingCancelEventArgs"/> instance containing the event data.</param>
        private void HandleSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            // Check if there is unsaved data
            // Check if anything is running
            //throw new NotImplementedException();
        }
    }
}
