﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using Apollo.Utilities;
using Apollo.Utilities.ExceptionHandling;
using Autofac;
using Autofac.Core;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal partial class App
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
        private const string DefaultErrorFileName = "projectexplorer.error.log";

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

                    functionReturnResult = 0;
                };

            var result = TopLevelExceptionHandler.RunGuarded(
                applicationAction, 
                Assembly.GetExecutingAssembly().GetName().Name, 
                DefaultErrorFileName);
            return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
        }

        /// <summary>
        /// Initializes the environment for use. Currently sets Environment Variables and 
        /// creates directories.
        /// </summary>
        private static void InitializeEnvironment()
        {
            // Set the internal logging for NLog
            Environment.SetEnvironmentVariable("NLOG_INTERNAL_LOG_FILE", @"d:\temp\nloginternal.log");
            Environment.SetEnvironmentVariable("NLOG_INTERNAL_LOG_LEVEL", "Debug");
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
                progressTracker,
                module => LoadUserInterface(module));

            // Load the core system. This will automatically
            // run the Prism bootstrapper which will then
            // load up the UI and display it.
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
    }
}
