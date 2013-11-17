#load AutomationId.Explorer.cs
#load AutomationId.Wpf.cs
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

public static ITabPage GetProjectPageTabItem(Application application)
{
    var tab = GetMainTab(application);
    var startPage = tab.Pages
        .Where(p => string.Equals("Project", p.Name))
        .FirstOrDefault();
    return startPage;
}

public static void CloseProjectPageTab(Application application)
{
    var startPage = GetProjectPageTabItem(application);

    var closeTabSearchCriteria = SearchCriteria
        .ByAutomationId(ProjectViewAutomationIds.CloseTabButton)
        .AndControlType(ControlType.Button);
    var closeTabButton = (Button)startPage.Get(closeTabSearchCriteria);
    closeTabButton.Click();
}

public static void CloseStartPageTab(Application application)
{
    var startPage = GetStartPageTabItem(application);

    var container = (TabPage)startPage;
    foreach(var item in container.Items)
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

    var closeTabSearchCriteria = SearchCriteria
        .ByAutomationId(WelcomeViewAutomationIds.CloseTabButton)
        .AndControlType(ControlType.Button);
    var closeTabButton = (Button)startPage.Get(closeTabSearchCriteria);
    closeTabButton.Click();
}

