

using System;
using System.Globalization;
using Apollo.UI.Explorer;
using Test.UI.Explorer.Controls;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;

namespace Test.UI.Explorer.UseCases
{
    public static partial class VerifyViews
    {
        // Execute Window verifications
        public static void VerifyTabBehaviour(Application application)
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

            Assert.IsTrue(startPage.IsSelected, "Tabs - Start is selected");
            Assert.IsFalse(projectPage.IsSelected, "Tabs - Project is not selected");

            TabProxies.SwitchToProjectPageViaTabMenu(application);
            Assert.IsFalse(startPage.IsSelected, "Tabs - Start is not selected");
            Assert.IsTrue(projectPage.IsSelected, "Tabs - Project is selected");

            TabProxies.SwitchToStartPageViaTabMenu(application);
            Assert.IsTrue(startPage.IsSelected, "Tabs - Start is selected");
            Assert.IsFalse(projectPage.IsSelected, "Tabs - Project is not selected");

            TabProxies.CloseProjectPageTab(application);
        }

        public static void VerifyWelcomeTab(Application application)
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
            Assert.AreEqual(Constants.GetProductName(), nameText, "Welcome tab - Product Name");

            // Check 'keep open' flag
            var keepOpenFlagSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ClosePageAfterLoad);
            var keepOpenCheckBox = (CheckBox)startPage.Get(keepOpenFlagSearchCriteria);
            Assert.IsFalse(keepOpenCheckBox.Checked, "Welcome tab - Keep open not checked");
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
            Assert.IsNotNull(currentStartPage, "Welcome tab - Start page exists after opening project");
            Assert.IsFalse(currentStartPage.IsSelected, "Welcome tab - Start page is not selected after opening project");

            var currentProjectPage = TabProxies.GetProjectPageTabItem(application);
            Assert.IsNotNull(currentProjectPage, "Welcome tab - Project page exists after opening project");
            Assert.IsTrue(currentProjectPage.IsSelected, "Welcome tab - Project page is selected after opening project");

            // Check that File - close has been enabled
            var fileCloseMenu = MenuProxies.GetFileCloseMenuItem(application);
            Assert.IsTrue(fileCloseMenu.Enabled, "Welcome tab - File - Close menu is enabled");

            // HACK: It seems that the File menu stays open when we check the File - close menu item
            var fileMenu = MenuProxies.GetFileMenuItem(application);
            if (fileMenu.IsFocussed)
            {
                fileMenu.Click();
            }

            // Close the project via the close button on the tab page
            TabProxies.CloseProjectPageTab(application);
        }

        public static void VerifyFileMenu(Application application)
        {
            // If a project is open, then close it

            // New (check that start page has closed, project page has opened, and close has been enabled)
            // Open
            // Exit
        }

        public static void VerifyEditMenu(Application application)
        {
            // Undo
            // Redo
        }

        public static void VerifyViewMenu(Application application)
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
            Assert.IsNotNull(startPage, "View menu - Check start page exists after clicking start page menu item");
            Assert.IsTrue(startPage.IsSelected, "View menu - Check start page is focussed after clicking start page menu item");

            // Open new project
            // Check that project enables
        }

        public static void VerifyRunMenu(Application application)
        {
            // Do nothing for now
        }

        public static void VerifyHelpMenu(Application application)
        {
            // VerifyHelpItem();
            VerifyAboutDialog(application);
        }

        private static void VerifyAboutDialog(Application application)
        {
            Log.Info("Verifying content of about dialog ...");

            MenuProxies.OpenAboutDialogViaHelpAboutMenuItem(application);
            var dialog = DialogProxies.AboutWindow(application);

            // Check application name
            var applicationNameSearchCiteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.ProductName);
            var nameLabel = (Label)dialog.Get(applicationNameSearchCiteria);
            var nameText = nameLabel.Text;
            Assert.AreEqual(Constants.GetProductName(), nameText, "About dialog - Product name");

            // Check application version
            var applicationVersionSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.ProductVersion);
            var versionLabel = (Label)dialog.Get(applicationVersionSearchCriteria);
            var versionText = versionLabel.Text;
            Assert.AreEqual(Constants.GetFullApplicationVersion(), versionText, "About dialog - Product version");

            // Check company name
            var companyNameSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.CompanyName);
            var companyLabel = (Label)dialog.Get(companyNameSearchCriteria);
            var companyText = companyLabel.Text;
            Assert.AreEqual(Constants.GetCompanyName(), companyText, "About dialog - Company name");

            // Check copyright
            var copyrightSearchCriteria = SearchCriteria
                .ByAutomationId(AboutAutomationIds.Copyright);
            var copyrightLabel = (Label)dialog.Get(copyrightSearchCriteria);
            var copyrightText = copyrightLabel.Text;
            Assert.AreEqual(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Copyright {0} 2009 - {1}",
                    Constants.GetCompanyName(),
                    DateTimeOffset.Now.Year),
                copyrightText,
                "About dialog - Copyright");

            dialog.Close();
        }
    }
}

