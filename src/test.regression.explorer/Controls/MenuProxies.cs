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
        /// <param name="log">The log object.</param>
        /// <returns>The main menu of the application.</returns>
        public static MenuBar GetMainMenu(Application application, Log log)
        {
            const string prefix = "Menus - Get main menu";
            var mainWindow = DialogProxies.MainWindow(application, log);
            if (mainWindow == null)
            {
                return null;
            }

            var menuSearchCriteria = SearchCriteria
                .ByAutomationId(MainMenuAutomationIds.Menu)
                .AndControlType(ControlType.Menu);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get main menu.");

                    var menu = (MenuBar)mainWindow.Get(menuSearchCriteria);
                    if (menu == null)
                    {
                        log.Error(prefix, "Failed to get the main menu.");
                    }

                    return menu;
                });
        }

        /// <summary>
        /// Returns the 'File' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The 'File' menu item.</returns>
        public static Menu GetFileMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - Get 'File' menu";
            var menu = GetMainMenu(application, log);
            if (menu == null)
            {
                return null;
            }

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the 'File' menu item.");

                    var menuItem = menu.MenuItemBy(fileMenuSearchCriteria);
                    if (menuItem == null)
                    {
                        log.Error(prefix, "Failed to get the 'File' menu item.");
                    }

                    return menuItem;
                });
        }

        /// <summary>
        /// Returns the 'File- New' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The 'File - New' menu item.</returns>
        public static Menu GetFileNewMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - Get 'File - New' menu";
            var menu = GetMainMenu(application, log);
            if (menu == null)
            {
                return null;
            }

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var newSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileNew);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the 'File - New' menu item.");

                    var menuItem = menu.MenuItemBy(fileMenuSearchCriteria, newSearchCriteria);
                    if (menuItem == null)
                    {
                        log.Error(prefix, "Failed to find the 'File - New' menu item.");
                    }

                    return menuItem;
                });
        }

        /// <summary>
        /// Returns the 'File - Close' menu item.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The 'File - Close' menu item.</returns>
        public static Menu GetFileCloseMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - Get 'File - Close' menu";
            var menu = GetMainMenu(application, log);
            if (menu == null)
            {
                return null;
            }

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var closeSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileClose);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the 'File - Close' menu item.");

                    var menuItem = menu.MenuItemBy(fileMenuSearchCriteria, closeSearchCriteria);
                    if (menuItem == null)
                    {
                        log.Error(prefix, "Failed to find the 'File - Close' menu item.");
                    }

                    return menuItem;
                });
        }

        /// <summary>
        /// Creates a new project via the 'File - New' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'File - New' menu could not be invoked for some reason.
        /// </exception>
        public static void CreateNewProjectViaFileNewMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - New project via File menu";
            var newMenu = GetFileNewMenuItem(application, log);
            if (newMenu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the 'File - New' menu item.");
            }

            try
            {
                newMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to click the 'File - New' menu item.", e);
            }
        }

        /// <summary>
        /// Closes a project via the 'File - Close' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'File - Close' menu could not be invoked for some reason.
        /// </exception>
        public static void CloseProjectViaFileCloseMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - Close project via File menu";
            var closeMenu = GetFileCloseMenuItem(application, log);
            if (closeMenu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the 'File - Close' menu item.");
            }

            try
            {
                closeMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to click the 'File - Close' menu item.", e);
            }
        }

        /// <summary>
        /// Closes the application via the 'File - Exit' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'File - Exit' menu could not be invoked for some reason.
        /// </exception>
        public static void CloseApplicationViaFileExitMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - Close application via File menu";
            var menu = GetMainMenu(application, log);
            if (menu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the main menu.");
            }

            var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
            var exitSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileExit);
            var exitMenu = Retry.Times(() => menu.MenuItemBy(fileMenuSearchCriteria, exitSearchCriteria));
            if (exitMenu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the 'File - Exit' menu.");
            }

            try
            {
                exitMenu.Click();
                application.Process.WaitForExit(Constants.ShutdownWaitTimeInMilliSeconds());
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to click the 'File - Exit' menu item.", e);
            }
        }

        /// <summary>
        /// Switches to the start page tab item via the 'View - Start page' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        public static void SwitchToStartPageViaViewStartPageMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - Open start page via View menu";
            var menu = GetMainMenu(application, log);
            if (menu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the main menu.");
            }

            var viewMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.View);
            var startSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.ViewStartPage);
            var startMenu = Retry.Times(() => menu.MenuItemBy(viewMenuSearchCriteria, startSearchCriteria));
            if (startMenu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the 'View' menu.");
            }

            try
            {
                startMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(
                    prefix + ": Failed to click the 'View - Start page' menu item.", 
                    e);
            }
        }

        /// <summary>
        /// Switches to the project tab item via the 'View - Start page' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        public static void SwitchToProjectPageViaViewStartPageMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - Open project page via View menu";
            var menu = GetMainMenu(application, log);
            if (menu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the main menu.");
            }

            var viewMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.View);
            var projectSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.ViewProjects);
            var projectMenu = Retry.Times(() => menu.MenuItemBy(viewMenuSearchCriteria, projectSearchCriteria));
            if (projectMenu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the 'View - Project' menu.");
            }

            try
            {
                projectMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(
                    prefix + ": Failed to click the 'View - Project' menu item.",
                    e);
            }
        }

        /// <summary>
        /// Opens the about dialog via the 'Help - About' menu.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        public static void OpenAboutDialogViaHelpAboutMenuItem(Application application, Log log)
        {
            const string prefix = "Menus - Open about dialog via Help menu";
            var menu = GetMainMenu(application, log);
            if (menu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the main menu.");
            }

            var helpMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.Help);
            var aboutSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.HelpAbout);
            var aboutMenu = Retry.Times(() => menu.MenuItemBy(helpMenuSearchCriteria, aboutSearchCriteria));
            if (aboutMenu == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the 'Help' menu.");
            }

            try
            {
                aboutMenu.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to click the 'Help - About' menu item.", e);
            }
        }
    }
}
