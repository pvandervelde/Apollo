//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows.Automation;
using Apollo.UI.Explorer;
using Apollo.UI.Wpf;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;

namespace Test.Regression.Explorer.Controls
{
    /// <summary>
    /// Provides helper methods for dealing with tab controls.
    /// </summary>
    internal static class TabProxies
    {
        /// <summary>
        /// Returns the tab control in the main window of the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The tab control in the main window of the application.</returns>
        public static Tab GetMainTab(Application application, Log log)
        {
            const string prefix = "Tabs - Get main tab control";
            var mainWindow = DialogProxies.MainWindow(application, log);
            if (mainWindow == null)
            {
                return null;
            }

            var tabSearchCriteria = SearchCriteria
                .ByAutomationId(ShellAutomationIds.Tabs);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the main tab control.");

                    var tab = (Tab)mainWindow.Get(tabSearchCriteria);
                    if (tab == null)
                    {
                        log.Error(prefix, "Failed to get the main tab control.");
                    }

                    return tab;
                });
        }

        /// <summary>
        /// Returns the the start page tab item for the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The start page tab item.</returns>
        public static ITabPage GetStartPageTabItem(Application application, Log log)
        {
            const string prefix = "Tabs - Get start page";
            var tab = GetMainTab(application, log);
            if (tab == null)
            {
                return null;
            }

            try
            {
                log.Debug(prefix, "Trying to get the 'Start page' tab item.");

                var startPage = tab.Pages.FirstOrDefault(p => string.Equals(WelcomeViewAutomationIds.Header, p.Id));
                if (startPage == null)
                {
                    log.Error(prefix, "Failed to get the 'Start page' tab item.");
                }

                return startPage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the project page tab item for the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The project page tab item.</returns>
        public static ITabPage GetProjectPageTabItem(Application application, Log log)
        {
            const string prefix = "Tabs - Get project page";
            var tab = GetMainTab(application, log);
            if (tab == null)
            {
                return null;
            }

            try
            {
                log.Debug(prefix, "Trying to get the 'Project' tab item.");

                var projectPage = tab.Pages.FirstOrDefault(p => string.Equals(ProjectViewAutomationIds.Header, p.Id));
                if (projectPage == null)
                {
                    log.Error(prefix, "Failed to get the 'Project' tab item.");
                }

                return projectPage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Closes the project tab item via the close button on the tab item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the tab could not be closed via the close button for some reason.
        /// </exception>
        public static void CloseProjectPageTab(Application application, Log log)
        {
            const string prefix = "Tabs - Close project tab";
            var projectPage = GetProjectPageTabItem(application, log);
            if (projectPage == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the project tab page.");
            }

            var closeTabSearchCriteria = SearchCriteria
                .ByAutomationId(ProjectViewAutomationIds.CloseTabButton)
                .AndControlType(ControlType.Button);
            var closeTabButton = Retry.Times(() => (Button)projectPage.Get(closeTabSearchCriteria));
            if (closeTabButton == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to find the close project tab button.");
            }

            try
            {
                closeTabButton.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to close the project tab.", e);
            }
        }

        /// <summary>
        /// Closes the start page tab item via the close button on the tab item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the tab could not be closed via the close button for some reason.
        /// </exception>
        public static void CloseStartPageTab(Application application, Log log)
        {
            const string prefix = "Tabs - Close start page tab";
            var startPage = GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to find the start page tab.");
            }

            var closeTabSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.CloseTabButton)
                .AndControlType(ControlType.Button);
            var closeTabButton = Retry.Times(() => (Button)startPage.Get(closeTabSearchCriteria));
            if (closeTabButton == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to find the close start page tab button.");
            }

            try
            {
                closeTabButton.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to close the project tab.", e);
            }
        }
    }
}
