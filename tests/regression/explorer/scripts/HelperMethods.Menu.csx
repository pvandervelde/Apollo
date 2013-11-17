#load AutomationId.Explorer.cs
#load HelperMethods.Constants.csx
#load HelperMethods.Logging.csx

#r UIAutomationTypes, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
#r UIAutomationClient, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35

using System.Globalization;
using System.Windows.Automation;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.MenuItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.UIItems.WindowStripControls;

// Menu
public static MenuBar GetMainMenu(Application application)
{
    var mainWindow = MainWindow(application);
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

// Menu - Help
public static void OpenAboutDialogViaHelpAboutMenuItem(Application application)
{
    var menu = GetMainMenu(application);

    var helpMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.Help);
    var aboutSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.HelpAbout);
    var aboutMenu = menu.MenuItemBy(helpMenuSearchCriteria, aboutSearchCriteria);

    aboutMenu.Click();
}

// Menu - View