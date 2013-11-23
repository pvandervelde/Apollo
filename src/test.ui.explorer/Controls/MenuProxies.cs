using System.Globalization;
using System.Windows.Automation;
using Apollo.UI.Explorer;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.MenuItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.UIItems.WindowStripControls;

namespace Test.UI.Explorer.Controls
{
    public static class MenuProxies
    {
        public static MenuBar GetMainMenu(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);
            var menuSearchCriteria = SearchCriteria
                .ByAutomationId(MainMenuAutomationIds.Menu)
                .AndControlType(ControlType.Menu);
            var menu = (MenuBar)mainWindow.Get(menuSearchCriteria);

            return menu;
        }

        public static Menu GetFileMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var fileMenu = menu.MenuItemBy(fileMenuSearchCriteria);
            return fileMenu;
        }

        public static Menu GetFileNewMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var newSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileNew);
            var newMenu = menu.MenuItemBy(fileMenuSearchCriteria, newSearchCriteria);
            return newMenu;
        }

        public static Menu GetFileCloseMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var closeSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileClose);
            var closeMenu = menu.MenuItemBy(fileMenuSearchCriteria, closeSearchCriteria);
            return closeMenu;
        }

        public static void CreateNewProjectViaFileNewMenuItem(Application application)
        {
            var newMenu = GetFileNewMenuItem(application);
            AssertIsTrue(newMenu.Enabled, "File - New - Menu item is enabled");

            newMenu.Click();
        }

        public static void CloseApplicationViaFileExitMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var exitSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileExit);
            var exitMenu = menu.MenuItemBy(fileMenuSearchCriteria, exitSearchCriteria);

            exitMenu.Click();

            application.Process.WaitForExit(ShutdownWaitTimeInMilliSeconds());
        }

        // Menu - Edit

        // Menu - View
        public static void OpenStartPageViaViewStartPageMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var viewMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.View);
            var startSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.ViewStartPage);
            var startMenu = menu.MenuItemBy(viewMenuSearchCriteria, startSearchCriteria);

            startMenu.Click();
        }

        // Menu - Help
        public static void OpenAboutDialogViaHelpAboutMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var helpMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.Help);
            var aboutSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.HelpAbout);
            var aboutMenu = menu.MenuItemBy(helpMenuSearchCriteria, aboutSearchCriteria);

            aboutMenu.Click();
        }
    }
}
