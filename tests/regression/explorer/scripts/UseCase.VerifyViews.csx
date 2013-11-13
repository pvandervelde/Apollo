#load HelperMethods.Application.csx
#load HelperMethods.Dialog.csx
#load HelperMethods.Initialization.csx
#load HelperMethods.Logging.csx
#load HelperMethods.Menu.csx
#load HelperMethods.Testing.csx
#load UseCase.VerifyViews.Cases.csx

LogInfo("Starting test ...");

InitializeWhite();

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
        "Started [{0}] - PID: [{1}]",
        application.Name,
        application.Process.Id);
    LogInfo(text);

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

    if (HasLoggedErrors())
    {
        throw new RegressionTestFailedException("One or more errors were logged.");
    }
}
catch(Exception e)
{
    LogError(e.ToString());
    throw;
}
finally
{
    if (application != null)
    {
        ExitApplication(application);
    }

    LogInfo("Test finished");
}

