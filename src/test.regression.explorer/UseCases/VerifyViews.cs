//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Apollo.Internals;
using Apollo.UI.Explorer;
using Test.Regression.Explorer.Controls;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;

namespace Test.Regression.Explorer.UseCases
{
    /// <summary>
    /// Verifies that the controls in the different views of the application work as they are supposed to.
    /// </summary>
    internal sealed class VerifyViews : IUserInterfaceVerifier
    {
        /// <summary>
        /// Returns a collection of tests that should be executed.
        /// </summary>
        /// <returns>The list of test cases that should be executed for the current verifier.</returns>
        public IEnumerable<TestCase> TestsToExecute()
        {
            return new List<TestCase>
                {
                    new TestCase("Tab behaviour", VerifyTabBehaviour),
                    new TestCase("Welcome tab", VerifyWelcomeTab),
                    new TestCase("Keep welcome tab open", VerifyKeepOpenCheckbox),
                    new TestCase("Initialize show welcome page", InitializeShowWelcomePageCheckbox),
                    new TestCase("Verify show welcome page", VerifyShowWelcomePageCheckbox),
                    new TestCase("View menu", VerifyViewMenu),
                    new TestCase("Help menu", VerifyHelpMenu),
                };
        }

        /// <summary>
        /// Verifies that the main window tab control works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The test result for the current test case.</returns>
        private TestResult VerifyTabBehaviour(Application application, Log log)
        {
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application);
                if (startPage == null)
                {
                    MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
                }

                var projectPage = TabProxies.GetProjectPageTabItem(application);
                if (projectPage == null)
                {
                    WelcomePageControlProxies.OpenProjectPageViaWelcomePageButton(application);
                }

                startPage = TabProxies.GetStartPageTabItem(application);
                if (startPage == null)
                {
                    result.AddError("Tabs - Failed to open the start page.");
                    return result;
                }

                projectPage = TabProxies.GetProjectPageTabItem(application);
                if (projectPage == null)
                {
                    result.AddError("Tabs - Failed to open the project page.");
                    return result;
                }

                try
                {
                    if (!startPage.IsSelected)
                    {
                        startPage.Select();
                    }
                }
                catch (Exception e)
                {
                    result.AddError(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Tabs - Failed to select the start page tab. Error was: {0}",
                            e));

                    return result;
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
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Tabs - Failed with exception. Error: {0}",
                    e);
                log.Error(message);
                result.AddError(message);
            }

            return result;
        }

        /// <summary>
        /// Verifies that the welcome tab page works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyWelcomeTab(Application application, Log log)
        {
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application);
                if (startPage == null)
                {
                    MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
                }

                startPage = TabProxies.GetStartPageTabItem(application);
                if (startPage == null)
                {
                    result.AddError("Welcome tab - Failed to get the start page.");
                    return result;
                }

                try
                {
                    if (!startPage.IsSelected)
                    {
                        startPage.Select();
                    }
                }
                catch (Exception e)
                {
                    result.AddError(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Welcome tab - Failed to select the start page tab. Error was: {0}",
                            e));

                    return result;
                }

                var applicationNameSearchCiteria = SearchCriteria
                    .ByAutomationId(WelcomeViewAutomationIds.ApplicationName);
                var nameLabel = Retry.Times(() => (Label)startPage.Get(applicationNameSearchCiteria));
                if (nameLabel == null)
                {
                    result.AddError("Welcome tab - Failed to get the application name label.");
                    return result;
                }

                var nameText = nameLabel.Text;
                assert.AreEqual(ProductInformation.ProductName, nameText, "Welcome tab - Product Name");
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Welcome tab - Failed with exception. Error: {0}",
                    e);
                log.Error(message);
                result.AddError(message);
            }

            return result;
        }

        /// <summary>
        /// Verifies that the welcome tab page works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyKeepOpenCheckbox(Application application, Log log)
        {
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application);
                if (startPage == null)
                {
                    MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
                }

                startPage = TabProxies.GetStartPageTabItem(application);
                if (startPage == null)
                {
                    result.AddError("Keep welcome tab open - Failed to get the start page.");
                    return result;
                }

                try
                {
                    if (!startPage.IsSelected)
                    {
                        startPage.Select();
                    }
                }
                catch (Exception e)
                {
                    result.AddError(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Keep welcome tab open - Failed to select the start page tab. Error was: {0}",
                            e));

                    return result;
                }

                // Check 'keep open' flag
                WelcomePageControlProxies.CheckKeepWelcomePageOpen(application);

                // New button
                var newProjectSearchCriteria = SearchCriteria
                    .ByAutomationId(WelcomeViewAutomationIds.NewProject);
                var newProjectButton = (Button)startPage.Get(newProjectSearchCriteria);
                newProjectButton.Click();

                // Check that the start page hasn't been closed
                var currentStartPage = TabProxies.GetStartPageTabItem(application);
                assert.IsNotNull(currentStartPage, "Keep welcome tab open - Start page exists after opening project");
                assert.IsFalse(currentStartPage.IsSelected, "Keep welcome tab open - Start page is not selected after opening project");

                var currentProjectPage = TabProxies.GetProjectPageTabItem(application);
                assert.IsNotNull(currentProjectPage, "Keep welcome tab open - Project page exists after opening project");
                assert.IsTrue(currentProjectPage.IsSelected, "Keep welcome tab open - Project page is selected after opening project");

                // Check that File - close has been enabled
                var fileCloseMenu = MenuProxies.GetFileCloseMenuItem(application);
                assert.IsTrue(fileCloseMenu.Enabled, "Keep welcome tab open - File - Close menu is enabled");

                // HACK: It seems that the File menu stays open when we check the File - close menu item
                var fileMenu = MenuProxies.GetFileMenuItem(application);
                if (fileMenu.IsFocussed)
                {
                    fileMenu.Click();
                }

                // Close the project via the close button on the tab page
                TabProxies.CloseProjectPageTab(application);

                WelcomePageControlProxies.UncheckKeepWelcomePageOpen(application);

                // New button
                newProjectButton.Click();

                // Check that the start page has been closed
                currentStartPage = TabProxies.GetStartPageTabItem(application);
                assert.IsNull(currentStartPage, "Keep welcome tab open - Start page exists after opening project");

                // Close the project via the close button on the tab page
                TabProxies.CloseProjectPageTab(application);
                WelcomePageControlProxies.CheckKeepWelcomePageOpen(application);
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Keep welcome tab open - Failed with exception. Error: {0}",
                    e);
                log.Error(message);
                result.AddError(message);
            }

            return result;
        }

        public TestResult InitializeShowWelcomePageCheckbox(Application application, Log log)
        {
            var result = new TestResult();
            try
            {
                WelcomePageControlProxies.UncheckShowWelcomePageOnApplicationStart(application);
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Initialize show welcome page - Failed with exception. Error: {0}",
                    e);
                log.Error(message);
                result.AddError(message);
            }

            return result;
        }

        public TestResult VerifyShowWelcomePageCheckbox(Application application, Log log)
        {
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application);
                assert.IsNull(startPage, "Verify show welcome page - Start page was open on application start.");

                WelcomePageControlProxies.CheckShowWelcomePageOnApplicationStart(application);
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify show welcome page - Failed with exception. Error: {0}",
                    e);
                log.Error(message);
                result.AddError(message);
            }

            return result;
        }

        /// <summary>
        /// Verifies that the 'File' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyFileMenu(Application application, Log log, Assert assert)
        {
            // If a project is open, then close it

            // New (check that start page has closed, project page has opened, and close has been enabled)
            // Open
            // Exit
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the 'Edit' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyEditMenu(Application application, Log log, Assert assert)
        {
            // Undo
            // Redo
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the 'View' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyViewMenu(Application application, Log log)
        {
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
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
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "View menu - Failed with exception. Error: {0}",
                    e);
                log.Error(message);
                result.AddError(message);
            }

            return result;
        }

        /// <summary>
        /// Verifies that the 'Run' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="assert">The object used to verify the test conditions.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyRunMenu(Application application, Log log, Assert assert)
        {
            // Do nothing for now
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the 'Help' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyHelpMenu(Application application, Log log)
        {
            var result = new TestResult();
            try
            {
                // VerifyHelpItem();
                VerifyAboutDialog(application, log, result);
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Help menu - Failed with exception. Error: {0}",
                    e);
                log.Error(message);
                result.AddError(message);
            }

            return result;
        }

        /// <summary>
        /// Verifies that the about dialog works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="result">The test result for the current test.</param>
        private void VerifyAboutDialog(Application application, Log log, TestResult result)
        {
            var assert = new Assert(result, log);
            log.Info("Verifying content of about dialog ...");

            MenuProxies.OpenAboutDialogViaHelpAboutMenuItem(application);
            var dialog = DialogProxies.AboutWindow(application);
            if (dialog == null)
            {
                result.AddError("About dialog - Failed to get dialog.");
                return;
            }

            // Check application name
            var applicationNameSearchCiteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.ProductName);
            var nameLabel = Retry.Times(() => (Label)dialog.Get(applicationNameSearchCiteria));
            if (nameLabel == null)
            {
                result.AddError("About dialog - Failed to get name label.");
                return;
            }

            var nameText = nameLabel.Text;
            assert.AreEqual(ProductInformation.ProductName, nameText, "About dialog - Product name");

            // Check application version
            var applicationVersionSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.ProductVersion);
            var versionLabel = Retry.Times(() => (Label)dialog.Get(applicationVersionSearchCriteria));
            if (versionLabel == null)
            {
                result.AddError("About dialog - Failed to get version label.");
                return;
            }

            var versionText = versionLabel.Text;
            assert.AreEqual(Assembly.GetExecutingAssembly().GetName().Version.ToString(4), versionText, "About dialog - Product version");

            // Check company name
            var companyNameSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.CompanyName);
            var companyLabel = Retry.Times(() => (Label)dialog.Get(companyNameSearchCriteria));
            if (companyLabel == null)
            {
                result.AddError("About dialog - Failed to get company label.");
                return;
            }

            var companyText = companyLabel.Text;
            assert.AreEqual(CompanyInformation.CompanyName, companyText, "About dialog - Company name");

            // Check copyright
            var copyrightSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.Copyright);
            var copyrightLabel = Retry.Times(() => (Label)dialog.Get(copyrightSearchCriteria));
            if (copyrightLabel == null)
            {
                result.AddError("About dialog - Failed to get copyright label.");
                return;
            }

            var copyrightText = copyrightLabel.Text;
            assert.AreEqual(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Copyright {0} 2009 - {1}",
                    CompanyInformation.CompanyName,
                    DateTimeOffset.Now.Year),
                copyrightText,
                "About dialog - Copyright");

            try
            {
                dialog.Close();
            }
            catch (Exception e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "About dialog - Failed to close the dialog. Error was: {0}",
                    e);
                log.Error(message);
                result.AddError(message);
            }
        }
    }
}
