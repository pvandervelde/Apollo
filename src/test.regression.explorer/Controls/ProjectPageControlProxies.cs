//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Apollo.UI.Wpf;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.ListBoxItems;

namespace Test.Regression.Explorer.Controls
{
    /// <summary>
    /// Provides helper methods for dealing with the controls on the project page.
    /// </summary>
    internal static class ProjectPageControlProxies
    {
        /// <summary>
        /// The maximum time we expect the activation or deactivation of a dataset to take.
        /// </summary>
        private static readonly TimeSpan s_DatasetActivationTime = new TimeSpan(0, 0, 5, 0);

        /// <summary>
        /// The maximum time we expect the creation or deletion of a dataset to take.
        /// </summary>
        private static readonly TimeSpan s_DatasetCreationOrDeletionTime = new TimeSpan(0, 0, 1, 0);

        private static TextBox GetProjectNameTextControl(Application application, Log log)
        {
            const string prefix = "Project page - Get project name control";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                return null;
            }

            var projectNameSearchCriteria = SearchCriteria
                .ByAutomationId(ProjectViewAutomationIds.ProjectName);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the project name control.");

                    var textBox = (TextBox)tab.Get(projectNameSearchCriteria);
                    if (textBox == null)
                    {
                        log.Error(prefix, "Failed to get the project name control.");
                    }

                    return textBox;
                });
        }

        private static TextBox GetProjectSummaryTextControl(Application application, Log log)
        {
            const string prefix = "Project page - Get project summary control";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                return null;
            }

            var projectSummarySearchCriteria = SearchCriteria
                .ByAutomationId(ProjectViewAutomationIds.ProjectSummary);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the project summary control.");

                    var textBox = (TextBox)tab.Get(projectSummarySearchCriteria);
                    if (textBox == null)
                    {
                        log.Error(prefix, "Failed to get the project summary control.");
                    }

                    return textBox;
                });
        }

        private static Label GetProjectDatasetCountControl(Application application, Log log)
        {
            const string prefix = "Project page - Get dataset count control";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                return null;
            }

            var datasetCountSearchCriteria = SearchCriteria
                .ByAutomationId(ProjectViewAutomationIds.DatasetCount);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the dataset count control.");

                    var label = (Label)tab.Get(datasetCountSearchCriteria);
                    if (label == null)
                    {
                        log.Error(prefix, "Failed to get the dataset count control.");
                    }

                    return label;
                });
        }

        private static TextBox GetDatasetNameTextControl(Application application, Log log, int id)
        {
            const string prefix = "Project page - Get dataset name control";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                return null;
            }

            var textBoxId = string.Format(
                CultureInfo.InvariantCulture,
                "TextBox_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetName,
                id);
            var textBoxSearchCriteria = SearchCriteria
                .ByAutomationId(textBoxId);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the dataset name control.");

                    var textBox = (TextBox)tab.Get(textBoxSearchCriteria);
                    if (textBox == null)
                    {
                        log.Error(prefix, "Failed to get the dataset name control.");
                    }

                    return textBox;
                });
        }

        private static TextBox GetDatasetSummaryTextControl(Application application, Log log, int id)
        {
            const string prefix = "Project page - Get dataset summary control";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                return null;
            }

            var textBoxId = string.Format(
                CultureInfo.InvariantCulture,
                "TextBox_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetSummary,
                id);
            var textBoxSearchCriteria = SearchCriteria
                .ByAutomationId(textBoxId);
            return Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get the dataset summary control.");

                    var textBox = (TextBox)tab.Get(textBoxSearchCriteria);
                    if (textBox == null)
                    {
                        log.Error(prefix, "Failed to get the dataset summary control.");
                    }

                    return textBox;
                });
        }

        /// <summary>
        /// Writes the given text to the project name text control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="name">The text that should be written to the name text control.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the project name could not be written to for some reason.
        /// </exception>
        public static void ProjectName(Application application, Log log, string name)
        {
            const string prefix = "Project page - Set project name";
            var textBox = GetProjectNameTextControl(application, log);
            if (textBox == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project name text control.");
            }

            try
            {
                textBox.BulkText = name;
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to set new project name.", e);
            }
        }

        /// <summary>
        /// Reads the text from the project name text control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The text contained in the project name text control; or <see langword="null"/> if the read failed.</returns>
        public static string ProjectName(Application application, Log log)
        {
            const string prefix = "Project page - Get project name";
            var textBox = GetProjectNameTextControl(application, log);
            if (textBox == null)
            {
                return null;
            }

            try
            {
                return textBox.BulkText;
            }
            catch (Exception e)
            {
                log.Error(
                    prefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to read project name. Error was: {0}",
                        e));
                return null;
            }
        }

        /// <summary>
        /// Writes the given text to the project summary text control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="summary">The text that should be written to the summary text control.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the project summary could not be written to for some reason.
        /// </exception>
        public static void ProjectSummary(Application application, Log log, string summary)
        {
            const string prefix = "Project page - Set project summary";
            var textBox = GetProjectSummaryTextControl(application, log);
            if (textBox == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project summary text control.");
            }

            try
            {
                textBox.BulkText = summary;
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to set new project summary.", e);
            }
        }

        /// <summary>
        /// Reads the text from the project summary text control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The text contained in the project summary text control; or <see langword="null"/> if the read failed.</returns>
        public static string ProjectSummary(Application application, Log log)
        {
            const string prefix = "Project page - Get project summary";
            var textBox = GetProjectSummaryTextControl(application, log);
            if (textBox == null)
            {
                return null;
            }

            try
            {
                return textBox.BulkText;
            }
            catch (Exception e)
            {
                log.Error(
                    prefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to read project summary. Error was: {0}",
                        e));
                return null;
            }
        }

        /// <summary>
        /// Returns the number of datasets that exist according to the control on the project view.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The number of datasets that exist according to the control on the project view.</returns>
        public static int GetNumberOfDatasetsViaProjectControl(Application application, Log log)
        {
            const string prefix = "Project page - Get number of datasets via project view";
            var label = GetProjectDatasetCountControl(application, log);
            if (label == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get the dataset count control.");
            }

            try
            {
                return int.Parse(label.Text);
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get the number of datasets from the project view.", e);
            }
        }

        private static SortedList<int, IUIItem> GetDatasetControls(Application application, Log log)
        {
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                return new SortedList<int, IUIItem>();
            }

            var partialId = string.Format(
                CultureInfo.InvariantCulture,
                "Vertex_[{0}_[DatasetId: [",
                DatasetViewAutomationIds.GraphVertex);
            var controls = ControlProxies.FindItemsManuallyInUIContainerWithPartialId((UIItemContainer)tab, partialId, log);

            var result = new SortedList<int, IUIItem>();
            foreach (var control in controls)
            {
                var idText = control.Id.Substring(partialId.Length).TrimEnd(']');

                int id;
                if (int.TryParse(idText, out id))
                {
                    result.Add(id, control);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a new child dataset for the root dataset.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        public static void CreateChildDatasetForRoot(Application application, Log log)
        {
            const string prefix = "Project page - Create child dataset for root";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project tab.");
            }

            var datasets = GetDatasetControls(application, log);
            if (datasets.Count == 0)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get dataset controls.");
            }

            // The root is always the lowest number, i.e. the first entry
            var buttonId = string.Format(
                CultureInfo.InvariantCulture,
                "Button_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetCreateChild,
                datasets.Keys[0]);
            var buttonSearchCriteria = SearchCriteria
                .ByAutomationId(buttonId);
            var button = tab.Get<Button>(buttonSearchCriteria);
            if (button == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get create dataset button.");
            }

            try
            {
                var ids = GetDatasetIds(application, log);

                button.Click();

                WaitForDatasetCreationOrDeletion(application, log, ids.Count() + 1);
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to create new dataset.", e);
            }
        }

        /// <summary>
        /// Waits for a dataset to be created.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="expectedCount">The current number of datasets.</param>
        public static void WaitForDatasetCreationOrDeletion(Application application, Log log, int expectedCount)
        {
            var endTime = DateTimeOffset.Now + s_DatasetCreationOrDeletionTime;
            while (DateTimeOffset.Now < endTime)
            {
                var ids = GetDatasetIds(application, log);
                if (ids.Count() == expectedCount)
                {
                    break;
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Returns the ID numbers of all known datasets.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <returns>A collection containing all the ID numbers of the known datasets.</returns>
        public static IEnumerable<int> GetDatasetIds(Application application, Log log)
        {
            var datasets = GetDatasetControls(application, log);
            return datasets.Keys;
        }

        /// <summary>
        /// Writes the given text to the project name text control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="id">The ID of the dataset.</param>
        /// <param name="name">The text that should be written to the name text control.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the project name could not be written to for some reason.
        /// </exception>
        public static void DatasetName(Application application, Log log, int id, string name)
        {
            const string prefix = "Project page - Set dataset name";
            var textBox = GetDatasetNameTextControl(application, log, id);
            if (textBox == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get dataset name text control.");
            }

            try
            {
                textBox.BulkText = name;
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to set new dataset name.", e);
            }
        }

        /// <summary>
        /// Reads the text from the project name text control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="id">The ID of the dataset.</param>
        /// <returns>The text contained in the project name text control; or <see langword="null"/> if the read failed.</returns>
        public static string DatasetName(Application application, Log log, int id)
        {
            const string prefix = "Project page - Get dataset name";
            var textBox = GetDatasetNameTextControl(application, log, id);
            if (textBox == null)
            {
                return null;
            }

            try
            {
                return textBox.BulkText;
            }
            catch (Exception e)
            {
                log.Error(
                    prefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to read dataset name. Error was: {0}",
                        e));
                return null;
            }
        }

        /// <summary>
        /// Writes the given text to the project summary text control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="id">The ID of the dataset.</param>
        /// <param name="summary">The text that should be written to the summary text control.</param>
        /// <exception cref="RegressionTestFailedException">
        ///     Thrown if the project summary could not be written to for some reason.
        /// </exception>
        public static void DatasetSummary(Application application, Log log, int id, string summary)
        {
            const string prefix = "Project page - Set dataset summary";
            var textBox = GetDatasetSummaryTextControl(application, log, id);
            if (textBox == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get dataset summary text control.");
            }

            try
            {
                textBox.BulkText = summary;
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to set new dataset summary.", e);
            }
        }

        /// <summary>
        /// Reads the text from the project summary text control.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="id">The ID of the dataset.</param>
        /// <returns>The text contained in the project summary text control; or <see langword="null"/> if the read failed.</returns>
        public static string DatasetSummary(Application application, Log log, int id)
        {
            const string prefix = "Project page - Get dataset summary";
            var textBox = GetDatasetSummaryTextControl(application, log, id);
            if (textBox == null)
            {
                return null;
            }

            try
            {
                return textBox.BulkText;
            }
            catch (Exception e)
            {
                log.Error(
                    prefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to read dataset summary. Error was: {0}",
                        e));
                return null;
            }
        }

        /// <summary>
        /// Activates the dataset with the given ID.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="id">The ID of the dataset that should be activated.</param>
        public static void ActivateDataset(Application application, Log log, int id)
        {
            const string prefix = "Project page - Activate dataset";
            if (IsDatasetActivated(application, log, id))
            {
                log.Info(
                    prefix,
                    "Dataset is already activated.");
                return;
            }

            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project tab.");
            }

            var datasets = GetDatasetControls(application, log);
            if (datasets.Count == 0)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get dataset controls.");
            }

            if (!datasets.ContainsKey(id))
            {
                throw new RegressionTestFailedException(prefix + " - Failed to find dataset with id: " + id.ToString(CultureInfo.InvariantCulture));
            }

            var buttonId = string.Format(
                CultureInfo.InvariantCulture,
                "Button_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetActivateDeactivate,
                id);
            var buttonSearchCriteria = SearchCriteria
                .ByAutomationId(buttonId);
            var button = tab.Get<Button>(buttonSearchCriteria);
            if (button == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get activate dataset button.");
            }

            try
            {
                button.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to activate the dataset.", e);
            }

            // handle dialog
            SelectMachineForDatasetActivation(application, log);

            // Wait for the dataset to be activated
            WaitForDatasetActivation(application, log, id);
        }

        private static void SelectMachineForDatasetActivation(Application application, Log log)
        {
            const string prefix = "Project page - Machine selection";
            var dialog = DialogProxies.DatasetMachineSelectionWindow(application, log);
            if (dialog == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get the machine selection window.");
            }

            var listSearchCriteria = SearchCriteria
                .ByAutomationId(MachineSelectorViewAutomationIds.AvailableMachines);
            var list = Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get available machines list.");
                    var listBox = dialog.Get<ListBox>(listSearchCriteria);
                    if (listBox == null)
                    {
                        log.Error(prefix, "Failed to get the available machines list.");
                    }

                    return listBox;
                });
            if (list == null)
            {
                list = (ListBox)ControlProxies.FindItemManuallyInUIContainer(
                    dialog,
                    MachineSelectorViewAutomationIds.AvailableMachines,
                    log);
                if (list == null)
                {
                    throw new RegressionTestFailedException(prefix + " - Failed to get the available machines list.");
                }
            }

            // Find the item that matches the current machine
            var item = list.Items.Find(i => string.Equals(Environment.MachineName, i.Text));
            if (item == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get the item for the current machine.");
            }

            item.Select();

            var buttonSearchCriteria = SearchCriteria
                .ByAutomationId(MachineSelectorViewAutomationIds.ConfirmSelection);
            var confirmButton = Retry.Times(
                () =>
                {
                    log.Debug(prefix, "Trying to get confirm button.");
                    var button = dialog.Get<Button>(buttonSearchCriteria);
                    if (button == null)
                    {
                        log.Error(prefix, "Failed to get the confirm button.");
                    }

                    return button;
                });
            if (confirmButton == null)
            {
                confirmButton = (Button)ControlProxies.FindItemManuallyInUIContainer(
                    dialog,
                    MachineSelectorViewAutomationIds.ConfirmSelection,
                    log);
                if (confirmButton == null)
                {
                    throw new RegressionTestFailedException(prefix + " - Failed to get the machine selection confirm button.");
                }
            }

            try
            {
                confirmButton.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + "Failed to confirm the machine selection.", e);
            }
        }

        private static void WaitForDatasetActivation(Application application, Log log, int id)
        {
            const string prefix = "Project page - Wait for dataset activation";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project tab.");
            }

            var textBlockId = string.Format(
                CultureInfo.InvariantCulture,
                "TextBlock_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetRunningOn,
                id);
            var textBlockSearchCriteria = SearchCriteria
                .ByAutomationId(textBlockId);
            var label = tab.Get<Label>(textBlockSearchCriteria);
            if (label == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get the dataset status label.");
            }

            var endTime = DateTimeOffset.Now + s_DatasetActivationTime;
            while (DateTimeOffset.Now < endTime)
            {
                try
                {
                    var text = label.Text;
                    if (!text.Contains("is not activated"))
                    {
                        return;
                    }

                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    log.Error(
                        prefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed to read the dataset status for dataset: {0}. Error was: {1}",
                            id,
                            e));
                }
            }
        }

        public static bool IsDatasetActivated(Application application, Log log, int id)
        {
            const string prefix = "Project page - Wait for dataset deactivation";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project tab.");
            }

            var textBlockId = string.Format(
                CultureInfo.InvariantCulture,
                "TextBlock_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetRunningOn,
                id);
            var textBlockSearchCriteria = SearchCriteria
                .ByAutomationId(textBlockId);
            var label = tab.Get<Label>(textBlockSearchCriteria);
            if (label == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get the dataset status label.");
            }

            try
            {
                var text = label.Text;
                return !text.Contains("is not activated");
            }
            catch (Exception e)
            {
                log.Error(
                    prefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to read the dataset status for dataset: {0}. Error was: {1}",
                        id,
                        e));
                return false;
            }
        }

        public static void DeactivateDataset(Application application, Log log, int id)
        {
            const string prefix = "Project page - Activate dataset";
            if (!IsDatasetActivated(application, log, id))
            {
                log.Info(
                    prefix,
                    "Dataset is not activated.");
                return;
            }

            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project tab.");
            }

            var datasets = GetDatasetControls(application, log);
            if (datasets.Count == 0)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get dataset controls.");
            }

            if (!datasets.ContainsKey(id))
            {
                throw new RegressionTestFailedException(prefix + " - Failed to find dataset with id: " + id.ToString(CultureInfo.InvariantCulture));
            }

            var buttonId = string.Format(
                CultureInfo.InvariantCulture,
                "Button_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetActivateDeactivate,
                id);
            var buttonSearchCriteria = SearchCriteria
                .ByAutomationId(buttonId);
            var button = tab.Get<Button>(buttonSearchCriteria);
            if (button == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get deactivate dataset button.");
            }

            try
            {
                button.Click();
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to deactivate the dataset.", e);
            }

            // Wait for the dataset to be activated
            WaitForDatasetDeactivation(application, log, id);
        }

        private static void WaitForDatasetDeactivation(Application application, Log log, int id)
        {
            const string prefix = "Project page - Wait for dataset deactivation";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project tab.");
            }

            var textBlockId = string.Format(
                CultureInfo.InvariantCulture,
                "TextBlock_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetRunningOn,
                id);
            var textBlockSearchCriteria = SearchCriteria
                .ByAutomationId(textBlockId);
            var label = tab.Get<Label>(textBlockSearchCriteria);
            if (label == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get the dataset status label.");
            }

            var endTime = DateTimeOffset.Now + s_DatasetActivationTime;
            while (DateTimeOffset.Now < endTime)
            {
                try
                {
                    var text = label.Text;
                    if (text.Contains("is not activated"))
                    {
                        return;
                    }

                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    log.Error(
                        prefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed to read the dataset status for dataset: {0}. Error was: {1}",
                            id,
                            e));
                }
            }
        }

        /// <summary>
        /// Deletes the dataset with the given ID.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        /// <param name="id">The ID of the dataset that should be deleted.</param>
        public static void DeleteDataset(Application application, Log log, int id)
        {
            const string prefix = "Project page - Delete dataset";
            var tab = TabProxies.GetProjectPageTabItem(application, log);
            if (tab == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get project tab.");
            }

            var datasets = GetDatasetControls(application, log);
            if (datasets.Count == 0)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get dataset controls.");
            }

            if (!datasets.ContainsKey(id))
            {
                throw new RegressionTestFailedException(prefix + " - Failed to find dataset with id: " + id.ToString(CultureInfo.InvariantCulture));
            }

            var buttonId = string.Format(
                CultureInfo.InvariantCulture,
                "Button_[{0}_[DatasetId: [{1}]]]",
                DatasetViewAutomationIds.DatasetDelete,
                id);
            var buttonSearchCriteria = SearchCriteria
                .ByAutomationId(buttonId);
            var button = tab.Get<Button>(buttonSearchCriteria);
            if (button == null)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to get delete dataset button.");
            }

            try
            {
                var ids = GetDatasetIds(application, log);

                button.Click();

                WaitForDatasetCreationOrDeletion(application, log, ids.Count() - 1);
            }
            catch (Exception e)
            {
                throw new RegressionTestFailedException(prefix + " - Failed to delete the dataset.", e);
            }
        }
    }
}
