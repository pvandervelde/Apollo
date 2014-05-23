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
        public IEnumerable<TestStep> TestsToExecute()
        {
            return new List<TestStep>
                {
                    new TestStep("Tab behaviour", VerifyTabBehaviour),
                    new TestStep("Welcome tab", VerifyWelcomeTab),
                    new TestStep("Close welcome page on project open", VerifyCloseOnProjectOpenCheckbox),
                    new TestStep("Initialize show welcome page", InitializeShowWelcomePageCheckbox),
                    new TestStep("Verify show welcome page", VerifyShowWelcomePageCheckbox),
                    new TestStep("File menu", VerifyFileMenu),
                    new TestStep("View menu", VerifyViewMenu),
                    new TestStep("Help menu", VerifyHelpMenu),
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
            const string prefix = "Tabs";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application, log);
                if (startPage == null)
                {
                    log.Info(prefix, "Opening start page.");
                    MenuProxies.SwitchToStartPageViaViewStartPageMenuItem(application, log);
                }

                // Make sure we don't close the welcome tab upon opening the project page
                WelcomePageControlProxies.UncheckCloseWelcomePageOnProjectOpen(application, log);

                var projectPage = TabProxies.GetProjectPageTabItem(application, log);
                if (projectPage == null)
                {
                    log.Info(prefix, "Opening project page.");
                    WelcomePageControlProxies.OpenProjectPageViaWelcomePageButton(application, log);
                }

                startPage = TabProxies.GetStartPageTabItem(application, log);
                if (startPage == null)
                {
                    var message = "Failed to open the start page.";
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);
                    return result;
                }

                projectPage = TabProxies.GetProjectPageTabItem(application, log);
                if (projectPage == null)
                {
                    var message = "Failed to open the project page.";
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);
                    return result;
                }

                try
                {
                    if (!startPage.IsSelected)
                    {
                        log.Info(prefix, "Setting focus to start page.");
                        startPage.Select();
                    }
                }
                catch (Exception e)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to select the start page tab. Error was: {0}",
                        e);
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);

                    return result;
                }

                assert.IsTrue(startPage.IsSelected, prefix + " - Start is selected");
                assert.IsFalse(projectPage.IsSelected, prefix + " - Project is not selected");

                MenuProxies.SwitchToProjectPageViaViewStartPageMenuItem(application, log);
                assert.IsFalse(startPage.IsSelected, prefix + " - Start is not selected");
                assert.IsTrue(projectPage.IsSelected, prefix + " - Project is selected");

                MenuProxies.SwitchToStartPageViaViewStartPageMenuItem(application, log);
                assert.IsTrue(startPage.IsSelected, prefix + " - Start is selected");
                assert.IsFalse(projectPage.IsSelected, prefix + " - Project is not selected");

                TabProxies.CloseProjectPageTab(application, log);
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed with exception. Error: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
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
            const string prefix = "Welcome tab";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application, log);
                if (startPage == null)
                {
                    log.Info(prefix, "Opening start page.");
                    MenuProxies.SwitchToStartPageViaViewStartPageMenuItem(application, log);
                }

                startPage = TabProxies.GetStartPageTabItem(application, log);
                if (startPage == null)
                {
                    var message = "Failed to get the start page.";
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);
                    return result;
                }

                try
                {
                    if (!startPage.IsSelected)
                    {
                        log.Info(prefix, "Setting focus to start page.");
                        startPage.Select();
                    }
                }
                catch (Exception e)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to select the start page tab. Error was: {0}",
                        e);
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);

                    return result;
                }

                var applicationNameSearchCiteria = SearchCriteria
                    .ByAutomationId(WelcomeViewAutomationIds.ApplicationName);
                var nameLabel = Retry.Times(
                    () =>
                    {
                        log.Debug(prefix, "Trying to get the application name label.");
                        var label = (Label)startPage.Get(applicationNameSearchCiteria);
                        if (label == null)
                        {
                            log.Error(prefix, "Failed to find the application name label.");
                        }

                        return label;
                    });
                if (nameLabel == null)
                {
                    var message = "Failed to get the application name label.";
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);
                    return result;
                }

                var nameText = nameLabel.Text;
                assert.AreEqual(ProductInformation.ProductName, nameText, prefix + " - Product Name");
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed with exception. Error: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
            }

            return result;
        }

        /// <summary>
        /// Verifies that the welcome tab page works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyCloseOnProjectOpenCheckbox(Application application, Log log)
        {
            const string prefix = "Close welcome tab on project open";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application, log);
                if (startPage == null)
                {
                    log.Info(prefix, "Opening start page.");
                    MenuProxies.SwitchToStartPageViaViewStartPageMenuItem(application, log);
                }

                startPage = TabProxies.GetStartPageTabItem(application, log);
                if (startPage == null)
                {
                    var message = "Failed to get the start page.";
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);
                    return result;
                }

                try
                {
                    if (!startPage.IsSelected)
                    {
                        log.Info(prefix, "Setting focus to start page.");
                        startPage.Select();
                    }
                }
                catch (Exception e)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to select the start page tab. Error was: {0}",
                        e);
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);

                    return result;
                }

                // Check 'keep open' flag
                WelcomePageControlProxies.UncheckCloseWelcomePageOnProjectOpen(application, log);

                // New button
                var newProjectSearchCriteria = SearchCriteria
                    .ByAutomationId(WelcomeViewAutomationIds.NewProject);
                var newProjectButton = (Button)startPage.Get(newProjectSearchCriteria);
                if (newProjectButton == null)
                {
                    var message = "Failed to get the 'New Project' button.";
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);
                    return result;
                }

                newProjectButton.Click();

                // Check that the start page hasn't been closed
                var currentStartPage = TabProxies.GetStartPageTabItem(application, log);
                assert.IsNotNull(currentStartPage, prefix + " - Start page does not exist after opening project");
                assert.IsFalse(currentStartPage.IsSelected, prefix + " - Start page is selected after opening project");

                var currentProjectPage = TabProxies.GetProjectPageTabItem(application, log);
                assert.IsNotNull(currentProjectPage, prefix + " - Project page does not exist after opening project");
                assert.IsTrue(currentProjectPage.IsSelected, prefix + " - Project page is not selected after opening project");

                // Check that File - close has been enabled
                var fileCloseMenu = MenuProxies.GetFileCloseMenuItem(application, log);
                assert.IsTrue(fileCloseMenu.Enabled, prefix + " - File - Close menu is not enabled");

                // HACK: It seems that the File menu stays open when we check the File - close menu item
                var fileMenu = MenuProxies.GetFileMenuItem(application, log);
                if (fileMenu == null)
                {
                    var message = "Failed to get the file menu.";
                    log.Error(prefix, message);
                    result.AddError(prefix + " - " + message);
                    return result;
                }

                if (fileMenu.IsFocussed)
                {
                    fileMenu.Click();
                }

                // Close the project via the close button on the tab page
                TabProxies.CloseProjectPageTab(application, log);

                WelcomePageControlProxies.CheckCloseWelcomePageOnProjectOpen(application, log);

                // New button
                newProjectButton.Click();

                // Check that the start page has been closed
                currentStartPage = TabProxies.GetStartPageTabItem(application, log);
                assert.IsNull(currentStartPage, prefix + " - Start page exists after opening project");

                // Close the project via the close button on the tab page
                TabProxies.CloseProjectPageTab(application, log);
                WelcomePageControlProxies.UncheckCloseWelcomePageOnProjectOpen(application, log);
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed with exception. Error: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
            }

            return result;
        }

        public TestResult InitializeShowWelcomePageCheckbox(Application application, Log log)
        {
            const string prefix = "Initialize show welcome page";
            var result = new TestResult();
            try
            {
                WelcomePageControlProxies.UncheckShowWelcomePageOnApplicationStart(application, log);
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed with exception. Error: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
            }

            return result;
        }

        public TestResult VerifyShowWelcomePageCheckbox(Application application, Log log)
        {
            const string prefix = "Verify show welcome page";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application, log);
                assert.IsNull(startPage, prefix + " - Start page was open on application start.");

                WelcomePageControlProxies.CheckShowWelcomePageOnApplicationStart(application, log);
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed with exception. Error: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
            }

            return result;
        }

        /// <summary>
        /// Verifies that the 'File' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyFileMenu(Application application, Log log)
        {
            const string prefix = "File menu";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var projectPage = TabProxies.GetProjectPageTabItem(application, log);
                if (projectPage != null)
                {
                    TabProxies.CloseProjectPageTab(application, log);
                }

                projectPage = TabProxies.GetProjectPageTabItem(application, log);
                assert.IsNull(projectPage, prefix + " - The project page was not closed.");

                MenuProxies.CreateNewProjectViaFileNewMenuItem(application, log);

                projectPage = TabProxies.GetProjectPageTabItem(application, log);
                assert.IsNotNull(projectPage, prefix + " - A new project was not created.");

                var fileCloseMenu = MenuProxies.GetFileCloseMenuItem(application, log);
                assert.IsTrue(fileCloseMenu.Enabled, prefix + " - File - Close menu is not enabled");

                MenuProxies.CloseProjectViaFileCloseMenuItem(application, log);
                
                projectPage = TabProxies.GetProjectPageTabItem(application, log);
                assert.IsNull(projectPage, prefix + " - The project page was not closed.");
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed with exception. Error: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
            }

            return result;
        }

        /// <summary>
        /// Verifies that the 'View' menu works as expected.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The test result for the current test case.</returns>
        public TestResult VerifyViewMenu(Application application, Log log)
        {
            const string prefix = "View menu";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var startPage = TabProxies.GetStartPageTabItem(application, log);
                if (startPage != null)
                {
                    log.Info(prefix, "Closing start page.");
                    TabProxies.CloseStartPageTab(application, log);
                }

                // Make sure we don't close the welcome tab upon opening the project page
                WelcomePageControlProxies.UncheckCloseWelcomePageOnProjectOpen(application, log);

                var projectPage = TabProxies.GetProjectPageTabItem(application, log);
                if (projectPage != null)
                {
                    log.Info(prefix, "Closing project page.");
                    TabProxies.CloseProjectPageTab(application, log);
                }

                // Open start page via view menu
                MenuProxies.SwitchToStartPageViaViewStartPageMenuItem(application, log);

                startPage = TabProxies.GetStartPageTabItem(application, log);
                assert.IsNotNull(startPage, prefix + " - Check start page exists after clicking start page menu item");
                assert.IsTrue(startPage.IsSelected, prefix + " - Check start page is focussed after clicking start page menu item");
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed with exception. Error: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
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
            const string prefix = "Help menu";
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
                    "Failed with exception. Error: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
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
            const string prefix = "About dialog";
            var assert = new Assert(result, log);
            log.Info(prefix, "Verifying content ...");

            MenuProxies.OpenAboutDialogViaHelpAboutMenuItem(application, log);
            var dialog = DialogProxies.AboutWindow(application, log);
            if (dialog == null)
            {
                var message = "Failed to get dialog.";
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
                return;
            }

            // Check application name
            var applicationNameSearchCiteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.ProductName);
            var nameLabel = Retry.Times(() => (Label)dialog.Get(applicationNameSearchCiteria));
            if (nameLabel == null)
            {
                var message = "Failed to get name label.";
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
                return;
            }

            var nameText = nameLabel.Text;
            assert.AreEqual(ProductInformation.ProductName, nameText, prefix + " - Product name");

            // Check application version
            var applicationVersionSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.ProductVersion);
            var versionLabel = Retry.Times(() => (Label)dialog.Get(applicationVersionSearchCriteria));
            if (versionLabel == null)
            {
                var message = "Failed to get version label.";
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
                return;
            }

            var versionText = versionLabel.Text;
            var versionAttribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            var version = (versionAttribute[0] as AssemblyInformationalVersionAttribute).InformationalVersion;
            assert.AreEqual(version, versionText, prefix + " - Product version");

            // Check company name
            var companyNameSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.CompanyName);
            var companyLabel = Retry.Times(() => (Label)dialog.Get(companyNameSearchCriteria));
            if (companyLabel == null)
            {
                var message = "Failed to get company label.";
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
                return;
            }

            var companyText = companyLabel.Text;
            assert.AreEqual(CompanyInformation.CompanyName, companyText, prefix + " - Company name");

            // Check copyright
            var copyrightSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.Copyright);
            var copyrightLabel = Retry.Times(() => (Label)dialog.Get(copyrightSearchCriteria));
            if (copyrightLabel == null)
            {
                var message = "Failed to get copyright label.";
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
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
                prefix + " - Copyright");

            try
            {
                dialog.Close();
            }
            catch (Exception e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed to close the dialog. Error was: {0}",
                    e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
            }
        }
    }
}
