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
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'New project' button could not be invoked for some reason.
        /// </exception>
        public static void OpenProjectPageViaWelcomePageButton(Application application, Log log)
        {
            const string prefix = "Welcome page - Open project";
            var startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get the start page.");
            }

            try
            {
                if (!startPage.IsSelected)
                {
                    log.Debug(prefix, "Selecting start page.");
                    startPage.Select();
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to select the start page.", e);
            }

            var newProjectSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.NewProject)
                .AndControlType(ControlType.Button);
            var newProjectButton = Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get 'New project' button.");
                    var button = (Button)startPage.Get(newProjectSearchCriteria);
                    if (button == null)
                    {
                        log.Error(prefix, "Failed to get 'New project' button.");
                    }

                    return button;
                });
            if (newProjectButton == null)
            {
                newProjectButton = (Button)ControlProxies.FindItemManuallyInUIContainer(
                    startPage as UIItemContainer,
                    WelcomeViewAutomationIds.NewProject,
                    log);
                if (newProjectButton == null)
                {
                    throw new RegressionTestFailedException(prefix + ": Failed to get the 'New project' button.");
                }
            }

            try
            {
                newProjectButton.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to click the 'New project' button.", e);
            }
        }

        /// <summary>
        /// Checks the 'Keep welcome page open' checkbox.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'Keep welcome page open' checkbox could not be checked for some reason.
        /// </exception>
        public static void CheckCloseWelcomePageOnProjectOpen(Application application, Log log)
        {
            const string prefix = "Welcome page - Check close welcome page on project open";
            var startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application, log);
            }

            startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get start page.");
            }

            try
            {
                if (!startPage.IsSelected)
                {
                    log.Debug(prefix, "Selecting start page.");
                    startPage.Select();
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(
                    prefix + ": Failed to select the start page", 
                    e);
            }

            // Check 'keep open' flag
            var closePageSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ClosePageAfterLoad)
                .AndControlType(ControlType.CheckBox);
            var closePageCheckBox = Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get checkbox.");
                    var checkBox = (CheckBox)startPage.Get(closePageSearchCriteria);
                    if (checkBox == null)
                    {
                        log.Error(prefix, "Failed to get checkbox.");
                    }

                    return checkBox;
                });
            if (closePageCheckBox == null)
            {
                closePageCheckBox = (CheckBox)ControlProxies.FindItemManuallyInUIContainer(
                    startPage as UIItemContainer, 
                    WelcomeViewAutomationIds.ClosePageAfterLoad,
                    log);
                if (closePageCheckBox == null)
                {
                    throw new RegressionTestFailedException(prefix + ": Failed to get checkbox.");
                }
            }

            try
            {
                if (!closePageCheckBox.Checked)
                {
                    log.Debug(prefix, "Checking 'Close welcome page on project open' checkbox.");
                    closePageCheckBox.Checked = true;
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(
                    prefix + ": Failed to check the 'Keep start page open' checkbox.", 
                    e);
            }
        }

        /// <summary>
        /// Unchecks the 'Keep welcome page open' checkbox.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'Keep welcome page open' checkbox could not be unchecked for some reason.
        /// </exception>
        public static void UncheckCloseWelcomePageOnProjectOpen(Application application, Log log)
        {
            const string prefix = "Welcome page - Uncheck close welcome page on project open";
            var startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application, log);
            }

            startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get start page.");
            }

            try
            {
                if (!startPage.IsSelected)
                {
                    log.Debug(prefix, "Selecting start page.");
                    startPage.Select();
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(
                    prefix + ": Failed to select the start page", 
                    e);
            }

            // Check 'keep open' flag
            var closePageSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ClosePageAfterLoad)
                .AndControlType(ControlType.CheckBox);
            var closePageCheckBox = Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get checkbox.");
                    var checkBox = (CheckBox)startPage.Get(closePageSearchCriteria);
                    if (checkBox == null)
                    {
                        log.Error(prefix, "Failed to get checkbox.");
                    }

                    return checkBox;
                });
            if (closePageCheckBox == null)
            {
                closePageCheckBox = (CheckBox)ControlProxies.FindItemManuallyInUIContainer(
                    startPage as UIItemContainer,
                    WelcomeViewAutomationIds.ClosePageAfterLoad,
                    log);
                if (closePageCheckBox == null)
                {
                    throw new RegressionTestFailedException(prefix + ": Failed to get checkbox.");
                }
            }

            try
            {
                if (closePageCheckBox.Checked)
                {
                    log.Debug(prefix, "Unchecking 'Keep start page open' checkbox.");
                    closePageCheckBox.Checked = false;
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(
                    prefix + ": Failed to uncheck the 'Keep start page open' checkbox.", 
                    e);
            }
        }

        /// <summary>
        /// Checks the 'Show welcome page on application start' checkbox.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'Show welcome page on application start' checkbox could not be checked for some reason.
        /// </exception>
        public static void CheckShowWelcomePageOnApplicationStart(Application application, Log log)
        {
            const string prefix = "Welcome page - Check show page on start";
            var startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application, log);
            }

            startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get start page");
            }

            try
            {
                if (!startPage.IsSelected)
                {
                    log.Debug(prefix, "Selecting start page.");
                    startPage.Select();
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to select the start page", e);
            }

            // Check 'Show welcome page on application start' flag
            var showStartPageSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ShowPageOnStartup)
                .AndControlType(ControlType.CheckBox);
            var showStartPageCheckBox = Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get checkbox.");
                    var checkBox = (CheckBox)startPage.Get(showStartPageSearchCriteria);
                    if (checkBox == null)
                    {
                        log.Error(prefix, "Failed to get checkbox.");
                    }

                    return checkBox;
                });
            if (showStartPageCheckBox == null)
            {
                showStartPageCheckBox = (CheckBox)ControlProxies.FindItemManuallyInUIContainer(
                    startPage as UIItemContainer,
                    WelcomeViewAutomationIds.ShowPageOnStartup,
                    log);
                if (showStartPageCheckBox == null)
                {
                    throw new RegressionTestFailedException(prefix + ": Failed to get checkbox.");
                }
            }

            try
            {
                if (!showStartPageCheckBox.Checked)
                {
                    log.Debug(prefix, "Checking 'Show welcome page on application start' checkbox.");
                    showStartPageCheckBox.Checked = true;
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(
                    prefix + ": Failed to check the 'Show welcome page on application start.", 
                    e);
            }
        }

        /// <summary>
        /// Unchecks the 'Show welcome page on application start' checkbox.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the 'Show welcome page on application start' checkbox could not be unchecked for some reason.
        /// </exception>
        public static void UncheckShowWelcomePageOnApplicationStart(Application application, Log log)
        {
            const string prefix = "Welcome page - Uncheck show page on start";
            var startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                MenuProxies.OpenStartPageViaViewStartPageMenuItem(application, log);
            }

            startPage = TabProxies.GetStartPageTabItem(application, log);
            if (startPage == null)
            {
                throw new RegressionTestFailedException(prefix + ": Failed to get start page.");
            }

            try
            {
                if (!startPage.IsSelected)
                {
                    log.Debug(prefix, "Selecting start page.");
                    startPage.Select();
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + "Failed to select the start page", e);
            }

            // Check 'Show welcome page on application start' flag
            var showStartPageSearchCriteria = SearchCriteria
                .ByAutomationId(WelcomeViewAutomationIds.ShowPageOnStartup)
                .AndControlType(ControlType.CheckBox);
            var showStartPageCheckBox = Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get checkbox.");
                    var checkBox = (CheckBox)startPage.Get(showStartPageSearchCriteria);
                    if (checkBox == null)
                    {
                        log.Error(prefix, "Failed to get checkbox.");
                    }

                    return checkBox;
                });
            if (showStartPageCheckBox == null)
            {
                showStartPageCheckBox = (CheckBox)ControlProxies.FindItemManuallyInUIContainer(
                    startPage as UIItemContainer,
                    WelcomeViewAutomationIds.ShowPageOnStartup,
                    log);
                if (showStartPageCheckBox == null)
                {
                    throw new RegressionTestFailedException(prefix + ": Failed to get checkbox.");
                }
            }

            try
            {
                if (showStartPageCheckBox.Checked)
                {
                    log.Debug(prefix, "Unchecking 'Show welcome page on application start' checkbox.");
                    showStartPageCheckBox.Checked = false;
                }
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(
                    prefix + "Failed to uncheck the 'Show welcome page on application start' checkbox.", 
                    e);
            }
        }
    }
}
