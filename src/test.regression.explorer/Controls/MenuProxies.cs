//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Automation;
using Apollo.UI.Explorer;
using TestStack.White;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.MenuItems;
using TestStack.White.UIItems.WindowStripControls;

namespace Test.UI.Explorer.Controls
{
    /// <summary>
    /// Provides helper methods for dealing with menus.
    /// </summary>
    internal static class MenuProxies
    {
        /// <summary>
        /// Returns the main menu of the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The main menu of the application.</returns>
        public static MenuBar GetMainMenu(Application application)
        {
            var mainWindow = DialogProxies.MainWindow(application);
            var menuSearchCriteria = SearchCriteria
                .ByAutomationId(MainMenuAutomationIds.Menu)
                .AndControlType(ControlType.Menu);
            var menu = (MenuBar)mainWindow.Get(menuSearchCriteria);

            return menu;
        }

        /// <summary>
        /// Returns the 'File' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The 'File' menu item.</returns>
        public static Menu GetFileMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var fileMenu = menu.MenuItemBy(fileMenuSearchCriteria);
            return fileMenu;
        }

        /// <summary>
        /// Returns the 'File- New' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The 'File - New' menu item.</returns>
        public static Menu GetFileNewMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var newSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileNew);
            var newMenu = menu.MenuItemBy(fileMenuSearchCriteria, newSearchCriteria);
            return newMenu;
        }

        /// <summary>
        /// Returns the 'File - Close' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The 'File - Close' menu item.</returns>
        public static Menu GetFileCloseMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var closeSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileClose);
            var closeMenu = menu.MenuItemBy(fileMenuSearchCriteria, closeSearchCriteria);
            return closeMenu;
        }

        /// <summary>
        /// Creates a new project via the 'File - New' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void CreateNewProjectViaFileNewMenuItem(Application application)
        {
            var newMenu = GetFileNewMenuItem(application);
            newMenu.Click();
        }

        /// <summary>
        /// Closes the application via the 'File - Exit' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void CloseApplicationViaFileExitMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var exitSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileExit);
            var exitMenu = menu.MenuItemBy(fileMenuSearchCriteria, exitSearchCriteria);

            exitMenu.Click();

            application.Process.WaitForExit(Constants.ShutdownWaitTimeInMilliSeconds());
        }

        /// <summary>
        /// Opens the start page tab item via the 'View - Start page' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void OpenStartPageViaViewStartPageMenuItem(Application application)
        {
            var menu = GetMainMenu(application);

            var viewMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.View);
            var startSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.ViewStartPage);
            var startMenu = menu.MenuItemBy(viewMenuSearchCriteria, startSearchCriteria);

            startMenu.Click();
        }

        /// <summary>
        /// Opens the about dialog via the 'Help - About' menu.
        /// </summary>
        /// <param name="application">The application.</param>
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
