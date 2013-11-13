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

    foreach(var item in mainWindow.Items)
    {
        Console.WriteLine(
            string.Format(
                CultureInfo.InvariantCulture,
                "UIItem: Id = {0}; Name={1}; Type={2}; AutomationId = {3}; ControlType = {4}",
                item.Id,
                item.Name,
                item.GetType(),
                item.AutomationElement.Current.AutomationId,
                item.AutomationElement.Current.ControlType.ProgrammaticName));

        var itemCollection = item as UIItemContainer;
        if (itemCollection != null)
        {
            foreach(var subItem in itemCollection.Items)
            {
                Console.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "    UIItem: Id = {0}; Name={1}; AutomationId = {2}; ControlType = {3}",
                        subItem.Id,
                        subItem.Name,
                        subItem.AutomationElement.Current.AutomationId,
                        subItem.AutomationElement.Current.ControlType.ProgrammaticName));
            }
        }
    }

    var menuSearchCriteria = SearchCriteria
        .ByAutomationId(MainMenuAutomationIds.Menu)
        .AndControlType(ControlType.Menu);
    var menu = (MenuBar)mainWindow.Get(menuSearchCriteria);

    return menu;
}

// Menu - Edit

// Menu - About

// Menu - View

public static

public static void CloseApplicationViaFileExitMenuItem(Application application)
{
    var menu = GetMainMenu(application);

    var fileMenuSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.File);
    var exitSearchCriteria = SearchCriteria.ByAutomationId(MainMenuAutomationIds.FileExit);
    var exitMenu = menu.MenuItemBy(fileMenuSearchCriteria, exitSearchCriteria);

    exitMenu.Click();

    application.Process.WaitForExit(ShutdownWaitTimeInMilliSeconds());
}