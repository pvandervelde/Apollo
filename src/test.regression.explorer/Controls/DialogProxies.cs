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
        /// <returns>The main window of the application.</returns>
        public static Window MainWindow(Application application)
        {
            // Note that the windows can't be found through an Automation ID for some reason, hence
            // using the title of the window.
            var window = application.GetWindow("Project explorer", InitializeOption.NoCache);
            return window;
        }

        /// <summary>
        /// Returns the about window of the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The about window of the application.</returns>
        public static Window AboutWindow(Application application)
        {
            // Note that the windows can't be found through an Automation ID for some reason, hence
            // using the title of the window.
            var mainWindow = MainWindow(application);
            var window = mainWindow.ModalWindow("About");
            return window;
        }
    }
}
