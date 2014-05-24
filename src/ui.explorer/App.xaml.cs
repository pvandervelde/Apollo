//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Shell;
using Apollo.UI.Explorer.Nuclei.ExceptionHandling;
using Apollo.Utilities;
using Autofac;
using Autofac.Core;
using Nuclei.Configuration;
using Nuclei.Diagnostics.Logging;

namespace Apollo.UI.Explorer
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    internal partial class App : IDisposable
    {
        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        public const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        public const int UnhandledExceptionApplicationExitCode = 1;

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "explorer.error.log";

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        /// <returns>A value indicating if the process exited normally (0) or abnormally (&gt; 0).</returns>
        [STAThread]
        public static int Main()
        {
            int functionReturnResult = -1;
            Action applicationAction = 
                () => 
                {
                    var app = new App();
                    app.InitializeComponent();
                    app.Run();

                    functionReturnResult = NormalApplicationExitCode;
                };

            using (var processor = new LogBasedExceptionProcessor(
                LoggerBuilder.ForFile(
                    Path.Combine(FileConstants.LogPath(), DefaultErrorFileName),
                    new DebugLogTemplate(new NullConfiguration(), () => DateTimeOffset.Now))))
            {
                var result = TopLevelExceptionGuard.RunGuarded(
                    applicationAction,
                    new ExceptionProcessor[]
                        {
                            processor.Process,
                        });

                return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
            }
        }

        /// <summary>
        /// Initializes the environment for use. 
        /// </summary>
        private static void InitializeEnvironment()
        {
        }

        /// <summary>
        /// The event that is used to signal the application that it is safe to shut down.
        /// </summary>
        private readonly AutoResetEvent m_ShutdownEvent
            = new AutoResetEvent(false);

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
            var bootstrapper = new KernelBootstrapper(
                m_ShutdownEvent,
                LoadUserInterface);

            // Load the core system. This will automatically
            // run the Prism bootstrapper which will then
            // load up the UI and display it.
            bootstrapper.Load();
        }

        /// <summary>
        /// Loads the user interface.
        /// </summary>
        /// <param name="container">The dependency injection container for the application.</param>
        private void LoadUserInterface(IContainer container)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var bootstrapper = new UserInterfaceBootstrapper(container, m_ShutdownEvent);
            bootstrapper.Run();
        }

        /// <summary>
        /// Called when the application is about to shut down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.ExitEventArgs"/> instance containing the event data.
        /// </param>
        private void HandleApplicationShutdown(object sender, ExitEventArgs e)
        {
            // At this point we are committed to a shutdown. There is
            // no way to stop it anymore.
        }

        /// <summary>
        /// Called when the windows session is about to end.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.SessionEndingCancelEventArgs"/> instance containing the event data.
        /// </param>
        private void HandleSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            // Check if there is unsaved data
            // Check if anything is running
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_ShutdownEvent.Dispose();
        }

        private void OnJumpListItemRejectedByOperatingSystem(object sender, JumpItemsRejectedEventArgs e)
        {
            // item rejected
        }

        private void OnJumpListItemRemovedByUser(object sender, JumpItemsRemovedEventArgs e)
        {
            // Item removed by user
        }
    }
}
