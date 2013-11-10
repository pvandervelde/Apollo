#load HelperMethods.Application.csx
#load HelperMethods.Dialog.csx
#load HelperMethods.Initialization.csx
#load HelperMethods.Logging.csx
#load HelperMethods.Testing.csx
#load UseCase.VerifyViews.Cases.csx

Log("Starting test ...");
Console.WriteLine("Starting test ...");

InitializeWhite();

// Load application
var applicationPath = GetApolloExplorerPath();
if (string.IsNullOrEmpty(applicationPath))
{
    throw new RegressionTestFailedException("Could not find application path.");
}

var application = StartApplication(applicationPath);
try
{
    var text = string.Format(
        CultureInfo.InvariantCulture,
        "{0} - PID: {1}",
        application.Name,
        application.Process.Id);

    Log(text);
    Console.WriteLine(text);

    // find the main window
    var mainWindow = MainWindow(application);

    text = string.Format(
        CultureInfo.InvariantCulture,
        "MainWindow: {0}",
        mainWindow.Title);

    Log(text);
    Console.WriteLine(text);

    // Execute Window verifications
    VerifyWelcomeTab();

    VerifyFileMenu();

    VerifyEditMenu();

    VerifyViewMenu();

    VerifyRunMenu();

    VerifyHelpMenu();

    CloseApplicationViaFileExitMenuItem(application);
    if (application.HasExited)
    {
        application = null;
    }
}
catch(Exception e)
{
    Console.WriteLine(e);
    Log(e.ToString());
    throw;
}
finally
{
    if (application != null)
    {
        ExitApplication(application);
    }
}

Log("Completed test");
Console.WriteLine("Completed test");