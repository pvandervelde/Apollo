//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using NUnit.Framework;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems.Finders;

namespace Test.Regression.UI.Explorer
{
    public static class ShellAutomation
    {
        public static Application StartApplication()
        {
            var processInfo = new ProcessStartInfo
                {
                    FileName = string.Empty,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Maximized,
                };

            var application = Application.Launch(processInfo);

            var searchCriteria = SearchCriteria.ByText(string.Empty);
            var mainWindow = application.GetWindow(searchCriteria, InitializeOption.NoCache);

            Assert.IsNotNull(mainWindow);
            Assert.IsTrue(mainWindow.IsCurrentlyActive);

            return application;
        }

        public static void ExitApplication(Application application)
        {
            application.Close();
            Assert.IsTrue(application.HasExited);
        }
    }
}
