#load AutomationId.Explorer.cs
#load HelperMethods.Constants.csx
#load HelperMethods.Logging.csx

using System.Globalization;
using System.Windows.Automation;
using TestStack.White;
using TestStack.White.UIItems.WindowItems;

public static Window MainWindow(Application application)
{
    // Note that the windows can't be found through an Automation ID for some reason, hence
    // using the title of the window.
    var window = application.GetWindow("Project explorer", InitializeOption.NoCache);
    return window;
}

public static Window AboutWindow(Application application)
{
    // Note that the windows can't be found through an Automation ID for some reason, hence
    // using the title of the window.
    var mainWindow = MainWindow(application);
    var window = mainWindow.ModalWindow("About");
    return window;
}