//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Automation;
using Apollo.UI.Explorer;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;

namespace Test.Regression.Explorer.Controls
{
    /// <summary>
    /// Provides helper methods for dealing with the controls on the welcome page.
    /// </summary>
    internal static class WelcomePageControlProxies
    {
        /// <summary>
        /// Opens the project page tab via the start page 'New project' button.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'New project' button could not be invoked for some reason.
        /// </exception>
        public static void OpenProjectPageViaWelcomePageButton(Application application)
        {
            var startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                throw new RegressionTestFailedException();
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
                throw new RegressionTestFailedException("Failed to select the start page.", e);
            }

            var newProjectSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.NewProject)
                .AndControlType(ControlType.Button);
            var newProjectButton = Retry.Times(() => (Button)startPage.Get(newProjectSearchCriteria));
            if (newProjectButton == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                newProjectButton.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to click the 'New project' button.", e);
            }
        }

        /// <summary>
        /// Checks the 'Keep welcome page open' checkbox.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'Keep welcome page open' checkbox could not be checked for some reason.
        /// </exception>
        public static void CheckKeepWelcomePageOpen(Application application)
        {
            var startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
            }

            startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                throw new RegressionTestFailedException();
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
                throw new RegressionTestFailedException("Failed to select the start page", e);
            }

            // Check 'keep open' flag
            var keepOpenFlagSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ClosePageAfterLoad);
            var keepOpenCheckBox = Retry.Times(() => (CheckBox)startPage.Get(keepOpenFlagSearchCriteria));
            if (keepOpenCheckBox == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                if (!keepOpenCheckBox.Checked)
                {
                    keepOpenCheckBox.Checked = true;
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to check the 'Keep start page open' checkbox.", e);
            }
        }

        /// <summary>
        /// Unchecks the 'Keep welcome page open' checkbox.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'Keep welcome page open' checkbox could not be unchecked for some reason.
        /// </exception>
        public static void UncheckKeepWelcomePageOpen(Application application)
        {
            var startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
            }

            startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                throw new RegressionTestFailedException();
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
                throw new RegressionTestFailedException("Failed to select the start page", e);
            }

            // Check 'keep open' flag
            var keepOpenFlagSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ClosePageAfterLoad);
            var keepOpenCheckBox = Retry.Times(() => (CheckBox)startPage.Get(keepOpenFlagSearchCriteria));
            if (keepOpenCheckBox == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                if (!keepOpenCheckBox.Checked)
                {
                    keepOpenCheckBox.Checked = false;
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to uncheck the 'Keep start page open' checkbox.", e);
            }
        }

        /// <summary>
        /// Checks the 'Show welcome page on application start' checkbox.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'Show welcome page on application start' checkbox could not be checked for some reason.
        /// </exception>
        public static void CheckShowWelcomePageOnApplicationStart(Application application)
        {
            var startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
            }

            startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                throw new RegressionTestFailedException();
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
                throw new RegressionTestFailedException("Failed to select the start page", e);
            }

            // Check 'Show welcome page on application start' flag
            var showStartPageSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ShowPageOnStartup);
            var showStartPageCheckBox = Retry.Times(() => (CheckBox)startPage.Get(showStartPageSearchCriteria));
            if (showStartPageCheckBox == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                if (!showStartPageCheckBox.Checked)
                {
                    showStartPageCheckBox.Checked = true;
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to check the 'Show welcome page on application start.", e);
            }
        }

        /// <summary>
        /// Unchecks the 'Show welcome page on application start' checkbox.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'Show welcome page on application start' checkbox could not be unchecked for some reason.
        /// </exception>
        public static void UncheckShowWelcomePageOnApplicationStart(Application application)
        {
            var startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application);
            }

            startPage = TabProxies.GetStartPageTabItem(application);
            if (startPage == null)
            {
                throw new RegressionTestFailedException();
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
                throw new RegressionTestFailedException("Failed to select the start page", e);
            }

            // Check 'Show welcome page on application start' flag
            var showStartPageSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ShowPageOnStartup);
            var showStartPageCheckBox = Retry.Times(() => (CheckBox)startPage.Get(showStartPageSearchCriteria));
            if (showStartPageCheckBox == null)
            {
                throw new RegressionTestFailedException();
            }

            try
            {
                if (showStartPageCheckBox.Checked)
                {
                    showStartPageCheckBox.Checked = false;
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException("Failed to uncheck the 'Show welcome page on application start' checkbox.", e);
            }
        }
    }
}
