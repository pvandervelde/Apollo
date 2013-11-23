using System.Globalization;
using System.Linq;
using System.Windows.Automation;
using Apollo.UI.Explorer;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.MenuItems;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.UIItems.WindowStripControls;

namespace Test.UI.Explorer.Controls
{
    public static class TabProxies
    {
        public static Tab GetMainTab(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);

            var tabSearchCriteria = SearchCriteria
                .ByAutomationId(ShellAutomationIds.Tabs);
            var tab = (Tab)mainWindow.Get(tabSearchCriteria);
            return tab;
        }

        public static ITabPage GetStartPageTabItem(Application application)
        {
            var tab = GetMainTab(application);
            var startPage = tab.Pages
                .Where(p => string.Equals(WelcomeViewAutomationIds.Header, p.Id))
                .FirstOrDefault();
            return startPage;
        }

        public static ITabPage GetProjectPageTabItem(Application application)
        {
            var tab = GetMainTab(application);
            var projectPage = tab.Pages
                .Where(p => string.Equals(ProjectViewAutomationIds.Header, p.Id))
                .FirstOrDefault();
            return projectPage;
        }

        public static void OpenProjectPageViaStartPageButton(Application application)
        {
            var startPage = GetStartPageTabItem(application);

            // New button
            var newProjectSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.NewProject);
            var newProjectButton = (Button)startPage.Get(newProjectSearchCriteria);
            newProjectButton.Click();
        }

        public static void CloseProjectPageTab(Application application)
        {
            var projectPage = GetProjectPageTabItem(application);

            var closeTabSearchCriteria = SearchCriteria
                .ByAutomationId(ProjectViewAutomationIds.CloseTabButton)
                .AndControlType(ControlType.Button);
            var closeTabButton = (Button)projectPage.Get(closeTabSearchCriteria);
            closeTabButton.Click();
        }

        public static void CloseStartPageTab(Application application)
        {
            var startPage = GetStartPageTabItem(application);

            var closeTabSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.CloseTabButton)
                .AndControlType(ControlType.Button);
            var closeTabButton = (Button)startPage.Get(closeTabSearchCriteria);
            closeTabButton.Click();
        }

        public static void SwitchToProjectPageViaTabMenu(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);

            var tabMenuSearchCriteria = SearchCriteria
                .ByAutomationId(TabAutomationIds.TabItems)
                .AndControlType(ControlType.Menu);
            var menu = (MenuBar)mainWindow.Get(tabMenuSearchCriteria);

            var topLevelMenuSearchCriteria = SearchCriteria.ByText(string.Empty);
            var projectViewSearchCriteria = SearchCriteria.ByAutomationId(ProjectViewAutomationIds.Header);
            var projectViewMenu = menu.MenuItemBy(topLevelMenuSearchCriteria, projectViewSearchCriteria);
            projectViewMenu.Click();
        }

        public static void SwitchToStartPageViaTabMenu(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);

            var tabMenuSearchCriteria = SearchCriteria
                .ByAutomationId(TabAutomationIds.TabItems)
                .AndControlType(ControlType.Menu);
            var menu = (MenuBar)mainWindow.Get(tabMenuSearchCriteria);

            var topLevelMenuSearchCriteria = SearchCriteria.ByText(string.Empty);
            var welcomeViewSearchCriteria = SearchCriteria.ByAutomationId(WelcomeViewAutomationIds.Header);
            var welcomeViewMenu = menu.MenuItemBy(topLevelMenuSearchCriteria, welcomeViewSearchCriteria);
            welcomeViewMenu.Click();
        }
    }
}

