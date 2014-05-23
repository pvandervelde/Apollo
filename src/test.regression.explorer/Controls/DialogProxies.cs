//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems.WindowItems;

namespace Test.Regression.Explorer.Controls
{
    /// <summary>
    /// Provides helper methods for dealing with dialogs.
    /// </summary>
    internal static class DialogProxies
    {
        /// <summary>
        /// Returns the main window of the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The main window of the application.</returns>
        public static Window MainWindow(Application application, Log log)
        {
            const string prefix = "Dialogs - MainWindow";
            if ((application == null) || application.HasExited)
            {
                throw new RegressionTestFailedException(prefix + ": Application does not exists or has already exited.");
            }

            // Note that the windows can't be found through an Automation ID for some reason, hence
            // using the title of the window.
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get main window.");
                    var window = application.GetWindow("Project explorer", InitializeOption.NoCache);

                    if (window == null)
                    {
                        log.Error(prefix, "Failed to get main window.");
                    }

                    return window;
                });
        }

        /// <summary>
        /// Returns the about window of the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The about window of the application.</returns>
        public static Window AboutWindow(Application application, Log log)
        {
            const string prefix = "Dialogs - AboutWindow";
            if ((application == null) || application.HasExited)
            {
                throw new RegressionTestFailedException(prefix + ": Application does not exist or has already exited.");
            }

            // Note that the windows can't be found through an Automation ID for some reason, hence
            // using the title of the window.
            var mainWindow = MainWindow(application, log);
            if (mainWindow == null)
            {
                return null;
            }

            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the about window.");

                    var window = mainWindow.ModalWindow("About");
                    if (window == null)
                    {
                        log.Error(prefix, "Failed to get the about window.");
                    }

                    return window;
                });
        }

        public static Window DatasetMachineSelectionWindow(Application application, Log log)
        {
            const string prefix = "Dialogs - MachineSelectionWindow";
            if ((application == null) || application.HasExited)
            {
                throw new RegressionTestFailedException(prefix + ": Application does not exist or has already exited.");
            }

            // Note that the windows can't be found through an Automation ID for some reason, hence
            // using the title of the window.
            var mainWindow = MainWindow(application, log);
            if (mainWindow == null)
            {
                return null;
            }

            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the machine selection window.");

                    var window = mainWindow.ModalWindow("Select a machine for the dataset");
                    if (window == null)
                    {
                        log.Error(prefix, "Failed to get the machine selection window.");
                    }

                    return window;
                });
        }
    }
}
