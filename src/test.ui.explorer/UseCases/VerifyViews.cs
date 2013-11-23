using System;
using System.Globalization;
using Test.UI.Explorer.Controls;

namespace Test.UI.Explorer.UseCases
{
    public static partial class VerifyViews
    {
        public static void Verify()
        {
            Log.Info("Starting test ...");

            Initialize.InitializeWhite();

            var applicationPath = ApplicationProxies.GetApolloExplorerPath();
            if (string.IsNullOrEmpty(applicationPath))
            {
                throw new RegressionTestFailedException("Could not find application path.");
            }

            var application = ApplicationProxies.StartApplication(applicationPath);
            try
            {
                var text = string.Format(
                    CultureInfo.InvariantCulture,
                    "Started [{0}] - PID: [{1}]",
                    application.Name,
                    application.Process.Id);
                Log.Info(text);

                VerifyTabBehaviour(application);

                VerifyWelcomeTab(application);

                VerifyFileMenu(application);

                VerifyEditMenu(application);

                VerifyViewMenu(application);

                VerifyRunMenu(application);

                VerifyHelpMenu(application);

                MenuProxies.CloseApplicationViaFileExitMenuItem(application);
                if (application.HasExited)
                {
                    application = null;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                throw;
            }
            finally
            {
                if (application != null)
                {
                    ApplicationProxies.ExitApplication(application);
                }

                Log.Info("Test finished");
            }

            if (Log.HasErrors())
            {
                throw new RegressionTestFailedException("One or more errors were logged.");
            }
        }
    }
}


