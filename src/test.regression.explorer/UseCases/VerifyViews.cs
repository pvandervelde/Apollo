using System;
using System.Globalization;
using System.Reflection;
using Apollo.Internals;
using Apollo.UI.Explorer;
using Test.UI.Explorer.Controls;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;

namespace Test.UI.Explorer.UseCases
{
    /// <summary>
    /// Verifies that the controls in the different views of the application work as they are supposed to.
    /// </summary>
    internal class VerifyViews : IUserInterfaceVerifier
    {
        /// <summary>
        /// Verifies a part of the user interface.
        /// </summary>
        /// <param name="testLog">The log object used for the current test.</param>
        public void Verify(Log testLog)
        {
            testLog.Info("Starting test ...");
            var assert = new Assert(testLog);

            var applicationPath = ApplicationProxies.GetApolloExplorerPath(testLog);
            if (string.IsNullOrEmpty(applicationPath))
            {
                throw new RegressionTestFailedException("Could not find application path.");
            }

            var application = ApplicationProxies.StartApplication(applicationPath, testLog);
            try
            {
                var text = string.Format(
                    CultureInfo.InvariantCulture,
                    "Started [{0}] - PID: [{1}]",
                    application.Name,
                    application.Process.Id);
                testLog.Info(text);

                VerifyTabBehaviour(application, testLog, assert);

                VerifyWelcomeTab(application, testLog, assert);

                VerifyFileMenu(application, testLog, assert);

                VerifyEditMenu(application, testLog, assert);

                VerifyViewMenu(application, testLog, assert);

                VerifyRunMenu(application, testLog, assert);

                VerifyHelpMenu(application, testLog, assert);

                MenuProxies.CloseApplicationViaFileExitMenuItem(application);
                if (application.HasExited)
                {
                    application = null;
                }
            }
            catch (Exception e)
            {
                testLog.Error(e.ToString());
            }
            finally
            {
                if (application != null)
                {
                    ApplicationProxies.ExitApplication(application, testLog);
                }

                testLog.Info("Test finished");
            }
        }

        /// <summary>
        /// Verifies that the main window tab control works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        private void VerifyTabBehaviour(Application application, Log log, Assert assert)
        {
            var startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
            }

            var projectPage = TabProxies.GetProjectPageTabItem(application);
            if (projectPage == null)
            {
                TabProxies.OpenProjectPageViaStartPageButton(application);
            }

            startPage = TabProxies.GetStartPageTabItem(application);
            projectPage = TabProxies.GetProjectPageTabItem(application);

            if (!startPage.IsSelected)
            {
                startPage.Select();
            }

            assert.IsTrue(startPage.IsSelected, "Tabs - Start is selected");
            assert.IsFalse(projectPage.IsSelected, "Tabs - Project is not selected");

            TabProxies.SwitchToProjectPageViaTabMenu(application);
            assert.IsFalse(startPage.IsSelected, "Tabs - Start is not selected");
            assert.IsTrue(projectPage.IsSelected, "Tabs - Project is selected");

            TabProxies.SwitchToStartPageViaTabMenu(application);
            assert.IsTrue(startPage.IsSelected, "Tabs - Start is selected");
            assert.IsFalse(projectPage.IsSelected, "Tabs - Project is not selected");

            TabProxies.CloseProjectPageTab(application);
        }

        /// <summary>
        /// Verifies that the welcome tab page works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        public void VerifyWelcomeTab(Application application, Log log, Assert assert)
        {
            var startPage = TabProxies.GetStartPageTabItem(application);
            if (!startPage.IsSelected)
            {
                startPage.Select();
            }

            var applicationNameSearchCiteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ApplicationName);
            var nameLabel = (Label)startPage.Get(applicationNameSearchCiteria);
            var nameText = nameLabel.Text;
            assert.AreEqual(ProductInformation.ProductName, nameText, "Welcome tab - Product Name");

            // Check 'keep open' flag
            var keepOpenFlagSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ClosePageAfterLoad);
            var keepOpenCheckBox = (CheckBox)startPage.Get(keepOpenFlagSearchCriteria);
            assert.IsFalse(keepOpenCheckBox.Checked, "Welcome tab - Keep open not checked");
            if (keepOpenCheckBox.Checked)
            {
                keepOpenCheckBox.Checked = false;
            }

            // New button
            var newProjectSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.NewProject);
            var newProjectButton = (Button)startPage.Get(newProjectSearchCriteria);
            newProjectButton.Click();

            // Check that the start page hasn't been closed
            var currentStartPage = TabProxies.GetStartPageTabItem(application);
            assert.IsNotNull(currentStartPage, "Welcome tab - Start page exists after opening project");
            assert.IsFalse(currentStartPage.IsSelected, "Welcome tab - Start page is not selected after opening project");

            var currentProjectPage = TabProxies.GetProjectPageTabItem(application);
            assert.IsNotNull(currentProjectPage, "Welcome tab - Project page exists after opening project");
            assert.IsTrue(currentProjectPage.IsSelected, "Welcome tab - Project page is selected after opening project");

            // Check that File - close has been enabled
            var fileCloseMenu = MenuProxies.GetFileCloseMenuItem(application);
            assert.IsTrue(fileCloseMenu.Enabled, "Welcome tab - File - Close menu is enabled");

            // HACK: It seems that the File menu stays open when we check the File - close menu item
            var fileMenu = MenuProxies.GetFileMenuItem(application);
            if (fileMenu.IsFocussed)
            {
                fileMenu.Click();
            }

            // Close the project via the close button on the tab page
            TabProxies.CloseProjectPageTab(application);
        }

        /// <summary>
        /// Verifies that the 'File' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        public void VerifyFileMenu(Application application, Log log, Assert assert)
        {
            // If a project is open, then close it

            // New (check that start page has closed, project page has opened, and close has been enabled)
            // Open
            // Exit
        }

        /// <summary>
        /// Verifies that the 'Edit' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        public void VerifyEditMenu(Application application, Log log, Assert assert)
        {
            // Undo
            // Redo
        }

        /// <summary>
        /// Verifies that the 'View' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        public void VerifyViewMenu(Application application, Log log, Assert assert)
        {
            var startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage != null)
            {
                TabProxies.CloseStartPageTab(application);
            }

            var projectPage = TabProxies.GetProjectPageTabItem(application);
            if (projectPage != null)
            {
                TabProxies.CloseProjectPageTab(application);
            }

            // Open start page via view menu
            MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
            startPage = TabProxies.GetStartPageTabItem(application);
            assert.IsNotNull(startPage, "View menu - Check start page exists after clicking start page menu item");
            assert.IsTrue(startPage.IsSelected, "View menu - Check start page is focussed after clicking start page menu item");

            // Open new project
            // Check that project enables
        }

        /// <summary>
        /// Verifies that the 'Run' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        public void VerifyRunMenu(Application application, Log log, Assert assert)
        {
            // Do nothing for now
        }

        /// <summary>
        /// Verifies that the 'Help' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        public void VerifyHelpMenu(Application application, Log log, Assert assert)
        {
            // VerifyHelpItem();
            VerifyAboutDialog(application, log, assert);
        }

        /// <summary>
        /// Verifies that the about dialog works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        private void VerifyAboutDialog(Application application, Log log, Assert assert)
        {
            log.Info("Verifying content of about dialog ...");

            MenuProxies.OpenAboutDialogViaHelpAboutMenuItem(application);
            var dialog = DialogProxies.AboutWindow(application);

            // Check application name
            var applicationNameSearchCiteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.ProductName);
            var nameLabel = (Label)dialog.Get(applicationNameSearchCiteria);
            var nameText = nameLabel.Text;
            assert.AreEqual(ProductInformation.ProductName, nameText, "About dialog - Product name");

            // Check application version
            var applicationVersionSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.ProductVersion);
            var versionLabel = (Label)dialog.Get(applicationVersionSearchCriteria);
            var versionText = versionLabel.Text;
            assert.AreEqual(Assembly.GetExecutingAssembly().GetName().Version.ToString(4), versionText, "About dialog - Product version");

            // Check company name
            var companyNameSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.CompanyName);
            var companyLabel = (Label)dialog.Get(companyNameSearchCriteria);
            var companyText = companyLabel.Text;
            assert.AreEqual(CompanyInformation.CompanyName, companyText, "About dialog - Company name");

            // Check copyright
            var copyrightSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.Copyright);
            var copyrightLabel = (Label)dialog.Get(copyrightSearchCriteria);
            var copyrightText = copyrightLabel.Text;
            assert.AreEqual(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Copyright {0} 2009 - {1}",
                    CompanyInformation.CompanyName,
                    DateTimeOffset.Now.Year),
                copyrightText,
                "About dialog - Copyright");

            dialog.Close();
        }
    }
}


