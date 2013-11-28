//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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

            var tabSearchCriteria = SearchCriteria
                .ByAutomationId(ShellAutomationIds.Tabs);
            var tab = (Tab)mainWindow.Get(tabSearchCriteria);
            return tab;
        }

        /// <summary>
        /// Returns the the start page tab item for the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The start page tab item.</returns>
        public static ITabPage GetStartPageTabItem(Application application)
        {
            var tab = GetMainTab(application);
            var startPage = tab.Pages.FirstOrDefault(p => string.Equals(WelcomeViewAutomationIds.Header, p.Id));
            return startPage;
        }

        /// <summary>
        /// Returns the project page tab item for the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The project page tab item.</returns>
        public static ITabPage GetProjectPageTabItem(Application application)
        {
            var tab = GetMainTab(application);
            var projectPage = tab.Pages.FirstOrDefault(p => string.Equals(ProjectViewAutomationIds.Header, p.Id));
            return projectPage;
        }

        /// <summary>
        /// Opens the project page tab via the start page 'New project' button.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void OpenProjectPageViaStartPageButton(Application application)
        {
            var startPage = GetStartPageTabItem(application);
            if (!startPage.IsSelected)
            {
                startPage.Select();
            }

            var newProjectSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.NewProject)
                .AndControlType(ControlType.Button);
            var newProjectButton = (Button)startPage.Get(newProjectSearchCriteria);
            newProjectButton.Click();
        }

        /// <summary>
        /// Closes the project tab item via the close button on the tab item.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void CloseProjectPageTab(Application application)
        {
            var projectPage = GetProjectPageTabItem(application);

            var closeTabSearchCriteria = SearchCriteria
                .ByAutomationId(ProjectViewAutomationIds.CloseTabButton)
                .AndControlType(ControlType.Button);
            var closeTabButton = (Button)projectPage.Get(closeTabSearchCriteria);
            closeTabButton.Click();
        }

        /// <summary>
        /// Closes the start page tab item via the close button on the tab item.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void CloseStartPageTab(Application application)
        {
            var startPage = GetStartPageTabItem(application);

            var closeTabSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.CloseTabButton)
                .AndControlType(ControlType.Button);
            var closeTabButton = (Button)startPage.Get(closeTabSearchCriteria);
            closeTabButton.Click();
        }
        
        /// <summary>
        /// Switches to the project tab item via the menu on the tab control.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void SwitchToProjectPageViaTabMenu(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);

            var tabMenuSearchCriteria = SearchCriteria
                .ByAutomationId(TabAutomationIds.TabItems)
                .AndControlType(ControlType.Menu);
            var menu = (MenuBar)mainWindow.Get(tabMenuSearchCriteria);

            var topLevelMenuSearchCriteria = SearchCriteria.ByText(string.Empty);
            var projectViewSearchCriteria = SearchCriteria.ByText("Project");
            var projectViewMenu = menu.MenuItemBy(topLevelMenuSearchCriteria, projectViewSearchCriteria);
            projectViewMenu.Click();
        }

        /// <summary>
        /// Switches to the start page tab item via the menu on the tab control.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void SwitchToStartPageViaTabMenu(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);

            var tabMenuSearchCriteria = SearchCriteria
                .ByAutomationId(TabAutomationIds.TabItems)
                .AndControlType(ControlType.Menu);
            var menu = (MenuBar)mainWindow.Get(tabMenuSearchCriteria);
            
            var topLevelMenuSearchCriteria = SearchCriteria.ByText(string.Empty);
            var welcomeViewSearchCriteria = SearchCriteria.ByText("Start page");
            var welcomeViewMenu = menu.MenuItemBy(topLevelMenuSearchCriteria, welcomeViewSearchCriteria);
            welcomeViewMenu.Click();
        }
    }
}
