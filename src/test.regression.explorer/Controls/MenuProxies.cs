//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Automation;
using Apollo.UI.Explorer;
using TestStack.White;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.MenuItems;
using TestStack.White.UIItems.WindowStripControls;

namespace Test.Regression.Explorer.Controls
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
            if (mainWindow == null)
            {
                return null;
            }

            var menuSearchCriteria = SearchCriteria
                .ByAutomationId(MainMenuAutomationIds.Menu)
                .AndControlType(ControlType.Menu);
            return Retry.Times(() => (MenuBar)mainWindow.Get(menuSearchCriteria));
        }

        /// <summary>
        /// Returns the 'File' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The 'File' menu item.</returns>
        public static Menu GetFileMenuItem(Application application)
        {
            var menu = GetMainMenu(application);
            if (menu == null)
            {
                return null;
            }

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            return Retry.Times(() => menu.MenuItemBy(fileMenuSearchCriteria));
        }

        /// <summary>
        /// Returns the 'File- New' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The 'File - New' menu item.</returns>
        public static Menu GetFileNewMenuItem(Application application)
        {
            var menu = GetMainMenu(application);
            if (menu == null)
            {
                return null;
            }

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var newSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileNew);
            return Retry.Times(() => menu.MenuItemBy(fileMenuSearchCriteria, newSearchCriteria));
        }

        /// <summary>
        /// Returns the 'File - Close' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The 'File - Close' menu item.</returns>
        public static Menu GetFileCloseMenuItem(Application application)
        {
            var menu = GetMainMenu(application);
            if (menu == null)
            {
                return null;
            }

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var closeSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileClose);
            return Retry.Times(() => menu.MenuItemBy(fileMenuSearchCriteria, closeSearchCriteria));
        }

        /// <summary>
        /// Creates a new project via the 'File - New' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'File - New' menu could not be invoked for some reason.
        /// </exception>
        public static void CreateNewProjectViaFileNewMenuItem(Application application)
        {
            var newMenu = GetFileNewMenuItem(application);
            if (newMenu == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                newMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to click the 'File - New' menu item.", e);
            }
        }

        /// <summary>
        /// Closes the application via the 'File - Exit' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'File - Exit' menu could not be invoked for some reason.
        /// </exception>
        public static void CloseApplicationViaFileExitMenuItem(Application application)
        {
            var menu = GetMainMenu(application);
            if (menu == null)
            {
                throw new RegressionTestFailedException();
            }

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var exitSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileExit);
            var exitMenu = Retry.Times(() => menu.MenuItemBy(fileMenuSearchCriteria, exitSearchCriteria));
            if (exitMenu == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                exitMenu.Click();
                application.Process.WaitForExit(Constants.ShutdownWaitTimeInMilliSeconds());
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to click the 'File - Exit' menu item.", e);
            }
        }

        /// <summary>
        /// Opens the start page tab item via the 'View - Start page' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void OpenStartPageViaViewStartPageMenuItem(Application application)
        {
            var menu = GetMainMenu(application);
            if (menu == null)
            {
                throw new RegressionTestFailedException();
            }

            var viewMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.View);
            var startSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.ViewStartPage);
            var startMenu = Retry.Times(() => menu.MenuItemBy(viewMenuSearchCriteria, startSearchCriteria));
            if (startMenu == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                startMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to click the 'View - Start page' menu item.", e);
            }
        }

        /// <summary>
        /// Opens the about dialog via the 'Help - About' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        public static void OpenAboutDialogViaHelpAboutMenuItem(Application application)
        {
            var menu = GetMainMenu(application);
            if (menu == null)
            {
                throw new RegressionTestFailedException();
            }

            var helpMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.Help);
            var aboutSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.HelpAbout);
            var aboutMenu = Retry.Times(() => menu.MenuItemBy(helpMenuSearchCriteria, aboutSearchCriteria));
            if (aboutMenu == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                aboutMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to click the 'Help - About' menu item.", e);
            }
        }
    }
}
