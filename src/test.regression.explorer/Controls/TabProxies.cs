//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows.Automation;
using Apollo.UI.Explorer;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.WindowStripControls;

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
        /// <returns>The tab control in the main window of the application.</returns>
        public static Tab GetMainTab(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);
            if (mainWindow == null)
            {
                return null;
            }

            var tabSearchCriteria = SearchCriteria
                .ByAutomationId(ShellAutomationIds.Tabs);
            return Retry.Times(() => (Tab)mainWindow.Get(tabSearchCriteria));
        }

        /// <summary>
        /// Returns the the start page tab item for the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The start page tab item.</returns>
        public static ITabPage GetStartPageTabItem(Application application)
        {
            var tab = GetMainTab(application);
            if (tab == null)
            {
                return null;
            }

            try
            {
                var startPage = tab.Pages.FirstOrDefault(p => string.Equals(WelcomeViewAutomationIds.Header, p.Id));
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
        /// <returns>The project page tab item.</returns>
        public static ITabPage GetProjectPageTabItem(Application application)
        {
            var tab = GetMainTab(application);
            if (tab == null)
            {
                return null;
            }

            try
            {
                var projectPage = tab.Pages.FirstOrDefault(p => string.Equals(ProjectViewAutomationIds.Header, p.Id));
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
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the tab could not be closed via the close button for some reason.
        /// </exception>
        public static void CloseProjectPageTab(Application application)
        {
            var projectPage = GetProjectPageTabItem(application);
            if (projectPage == null)
            {
                throw new RegressionTestFailedException();
            }

            var closeTabSearchCriteria = SearchCriteria
                .ByAutomationId(ProjectViewAutomationIds.CloseTabButton)
                .AndControlType(ControlType.Button);
            var closeTabButton = Retry.Times(() => (Button)projectPage.Get(closeTabSearchCriteria));
            if (closeTabButton == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                closeTabButton.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to close the project tab.", e);
            }
        }

        /// <summary>
        /// Closes the start page tab item via the close button on the tab item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the tab could not be closed via the close button for some reason.
        /// </exception>
        public static void CloseStartPageTab(Application application)
        {
            var startPage = GetStartPageTabItem(application);
            if (startPage == null)
            {
                throw new RegressionTestFailedException();
            }

            var closeTabSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.CloseTabButton)
                .AndControlType(ControlType.Button);
            var closeTabButton = Retry.Times(() => (Button)startPage.Get(closeTabSearchCriteria));
            if (closeTabButton == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                closeTabButton.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to close the project tab.", e);
            }
        }
        
        /// <summary>
        /// Switches to the project tab item via the menu on the tab control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the project page could not be given focus via the tab menu.
        /// </exception>
        public static void SwitchToProjectPageViaTabMenu(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);
            if (mainWindow == null)
            {
                throw new RegressionTestFailedException();
            }

            var tabMenuSearchCriteria = SearchCriteria
                .ByAutomationId(TabAutomationIds.TabItems)
                .AndControlType(ControlType.Menu);
            var menu = Retry.Times(() => (MenuBar)mainWindow.Get(tabMenuSearchCriteria));
            if (menu == null)
            {
                throw new RegressionTestFailedException();
            }

            var topLevelMenuSearchCriteria = SearchCriteria.ByText(string.Empty);
            var projectViewSearchCriteria = SearchCriteria.ByText("Project");
            var projectViewMenu = Retry.Times(() => menu.MenuItemBy(topLevelMenuSearchCriteria, projectViewSearchCriteria));
            if (projectViewMenu == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                projectViewMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to switch to project tab via the tab menu.", e);
            }
        }

        /// <summary>
        /// Switches to the start page tab item via the menu on the tab control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the start page could not be given focus via the tab menu.
        /// </exception>
        public static void SwitchToStartPageViaTabMenu(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);
            if (mainWindow == null)
            {
                throw new RegressionTestFailedException();
            }

            var tabMenuSearchCriteria = SearchCriteria
                .ByAutomationId(TabAutomationIds.TabItems)
                .AndControlType(ControlType.Menu);
            var menu = Retry.Times(() => (MenuBar)mainWindow.Get(tabMenuSearchCriteria));
            if (menu == null)
            {
                throw new RegressionTestFailedException();
            }

            var topLevelMenuSearchCriteria = SearchCriteria.ByText(string.Empty);
            var welcomeViewSearchCriteria = SearchCriteria.ByText("Start page");
            var welcomeViewMenu = Retry.Times(() => menu.MenuItemBy(topLevelMenuSearchCriteria, welcomeViewSearchCriteria));
            if (welcomeViewMenu == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                welcomeViewMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to switch to the start page tab via the tab menu.", e);
            }
        }
    }
}
