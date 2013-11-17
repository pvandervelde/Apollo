#load AutomationId.Explorer.cs
#load HelperMethods.Constants.csx
#load HelperMethods.Dialog.csx
#load HelperMethods.Logging.csx

#r UIAutomationTypes, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
#r UIAutomationClient, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35

using System.Globalization;
using System.Windows.Automation;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.WindowItems;

public static Tab GetMainTab(Application application)
{
    var mainWindow = MainWindow(application);

    var tabSearchCriteria = SearchCriteria
        .ByControlType(ControlType.Tab);
    var tab = (Tab)mainWindow.Get(tabSearchCriteria);
    return tab;
}

public static ITabPage GetStartPageTabItem(Application application)
{
    var tab = GetMainTab(application);
    var startPage = tab.Pages
        .Where(p => string.Equals("Start page", p.Name))
        .FirstOrDefault();
    return startPage;
}