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
        // WHERE IN THE HELL DOES ALL OF THIS COME FROM? PRISM?
        //
        // Note that it's going to be tricky to put a try..catch around the run-loop of this thing
        // 1) We don't have easy access to the code because that is generated
        // 2) There is the damn message loop which complicates things
        // Not entirely sure how to deal with that yet.
        // Eventually we want to hook up the exception handler that is also used for the 
        // command line apps.

        /// <summary>
        /// Initializes the environment for use. Currently sets Environment Variables and 
        /// creates directories.
        /// </summary>
        private static void InitializeEnvironment()
        {
            // Set the internal logging for NLog
            // Environment.SetEnvironmentVariable("NLOG_INTERNAL_LOG_FILE", @"d:\temp\nloginternal.log");
            // Environment.SetEnvironmentVariable("NLOG_INTERNAL_LOG_LEVEL", "Debug");
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.</param>
        private void HandleApplicationStartup(object sender, StartupEventArgs e)
        {
            InitializeEnvironment();

            LoadKernel();
        }

        /// <summary>
        /// Loads the kernel.
        /// </summary>
        private void LoadKernel()
        {
            // At a later stage we need to clean this up.
            // there are two constants and a DI reference.
            var progressTracker = new TimeBasedProgressTracker(
                new ProgressTimer(new TimeSpan(0, 0, 0, 0, 500)),
                -1,
                new StartupTimeStorage());

            var bootstrapper = new KernelBootstrapper(
                new BootstrapperStartInfo(),
                () => new MockExceptionHandler(),
                progressTracker,
                module => LoadUserInterface(module));

            // Load the kernel
            bootstrapper.Load();
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
            // throw new NotImplementedException();
        }
    }
}
