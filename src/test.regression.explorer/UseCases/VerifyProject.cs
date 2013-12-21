//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Test.Regression.Explorer.Controls;
using TestStack.White;

namespace Test.Regression.Explorer.UseCases
{
    /// <summary>
    /// Verifies that it is possible to create a new project and
    /// that it is possible to manipulate that project via the user interface.
    /// </summary>
    internal sealed class VerifyProject : IUserInterfaceVerifier
    {
        /// <summary>
        /// Returns a collection of tests that should be executed.
        /// </summary>
        /// <returns>The list of test cases that should be executed for the current verifier.</returns>
        public IEnumerable<TestStep> TestsToExecute()
        {
            return new List<TestStep>
                {
                    new TestStep("Update project information", VerifyProjectInformation),
                    new TestStep("Create datasets", VerifyDatasetCreation),
                    new TestStep("Update dataset information", VerifyDatasetInformation),
                    new TestStep("ActivateDataset", VerifyActivateDataset),
                };
        }

        private TestResult VerifyProjectInformation(Application application, Log log)
        {
            const string prefix = "Project information";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                // Start new project via File menu
                var projectPage = TabProxies.GetProjectPageTabItem(application, log);
                if (projectPage == null)
                {
                    MenuProxies.CreateNewProjectViaFileNewMenuItem(application, log);
                }

                projectPage = TabProxies.GetProjectPageTabItem(application, log);
                assert.IsNotNull(projectPage, prefix + " - The project page was not opened.");

                // Set a name
                var name = "Project-Test-Name";
                ProjectPageControlProxies.ProjectName(application, log, name);

                var storedName = ProjectPageControlProxies.ProjectName(application, log);
                assert.AreEqual(name, storedName, prefix + " - The written project name does not match the stored project name.");

                // Set a summary
                var summary = "Project-Test-Summary";
                ProjectPageControlProxies.ProjectSummary(application, log, summary);

                var storedSummary = ProjectPageControlProxies.ProjectSummary(application, log);
                assert.AreEqual(summary, storedSummary, prefix + " - The written project summary does not match the stored project summary.");

                // Set focus away from the text control so that the changes 'stick' by clicking somewhere, in this case the project tab item.
                projectPage.Click();

                // Undo
                MenuProxies.UndoViaEditMenu(application, log);
                storedName = ProjectPageControlProxies.ProjectName(application, log);
                assert.AreEqual(name, storedName, prefix + " - The project name change was undone too early.");

                storedSummary = ProjectPageControlProxies.ProjectSummary(application, log);
                assert.IsTrue(string.IsNullOrEmpty(storedSummary), prefix + " - The change to the project summary was not undone.");

                // Undo 
                MenuProxies.UndoViaEditMenu(application, log);

                storedName = ProjectPageControlProxies.ProjectName(application, log);
                assert.IsTrue(string.IsNullOrEmpty(storedName), prefix + " - The change to the project name was not undone.");

                storedSummary = ProjectPageControlProxies.ProjectSummary(application, log);
                assert.IsTrue(string.IsNullOrEmpty(storedSummary), prefix + " - The change to the project summary was not undone.");

                // Redo
                MenuProxies.RedoViaEditMenu(application, log);
                storedName = ProjectPageControlProxies.ProjectName(application, log);
                assert.AreEqual(name, storedName, prefix + " - The change to the project name was not redone.");

                storedSummary = ProjectPageControlProxies.ProjectSummary(application, log);
                assert.IsTrue(string.IsNullOrEmpty(storedSummary), prefix + " - The change to the project summary was redone too early.");

                // Redo
                MenuProxies.RedoViaEditMenu(application, log);
                storedName = ProjectPageControlProxies.ProjectName(application, log);
                assert.AreEqual(name, storedName, prefix + " - The change to the project name was not redone.");

                storedSummary = ProjectPageControlProxies.ProjectSummary(application, log);
                assert.AreEqual(summary, storedSummary, prefix + " - The change to the project summary was not redone.");
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

        private TestResult VerifyDatasetCreation(Application application, Log log)
        {
            const string prefix = "Dataset information";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                var projectPage = TabProxies.GetProjectPageTabItem(application, log);
                if (projectPage == null)
                {
                    MenuProxies.CreateNewProjectViaFileNewMenuItem(application, log);
                }

                projectPage = TabProxies.GetProjectPageTabItem(application, log);
                assert.IsNotNull(projectPage, prefix + " - The project page was not opened.");

                var datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                var datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(), 
                    datasetCount, 
                    prefix + " - The number of datasets does not match the number of dataset IDs before creating sub-datasets.");

                ProjectPageControlProxies.CreateChildDatasetForRoot(application, log);

                datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(),
                    datasetCount,
                    prefix + " - The number of datasets does not match the number of dataset IDs after creating 1 sub-dataset.");

                ProjectPageControlProxies.CreateChildDatasetForRoot(application, log);

                datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(),
                    datasetCount,
                    prefix + " - The number of datasets does not match the number of dataset IDs after creating 2 sub-datasets.");

                // Undo
                MenuProxies.UndoViaEditMenu(application, log);
                ProjectPageControlProxies.WaitForDatasetCreationOrDeletion(application, log, 2);

                datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(),
                    datasetCount,
                    prefix + " - The number of datasets does not match the number of dataset IDs after undoing the creation of the second dataset.");

                // Undo
                MenuProxies.UndoViaEditMenu(application, log);
                ProjectPageControlProxies.WaitForDatasetCreationOrDeletion(application, log, 1);

                datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(),
                    datasetCount,
                    prefix + " - The number of datasets does not match the number of dataset IDs after undoing the creation of the first dataset.");

                // Redo
                MenuProxies.RedoViaEditMenu(application, log);
                ProjectPageControlProxies.WaitForDatasetCreationOrDeletion(application, log, 2);

                datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(),
                    datasetCount,
                    prefix + " - The number of datasets does not match the number of dataset IDs after redoing the creation of the first dataset.");

                // Redo
                MenuProxies.RedoViaEditMenu(application, log);
                ProjectPageControlProxies.WaitForDatasetCreationOrDeletion(application, log, 3);

                datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(),
                    datasetCount,
                    prefix + " - The number of datasets does not match the number of dataset IDs after redoing the creation of the second dataset.");

                // Delete first child
                var ids = new List<int>(datasetIds);
                ids.Sort();
                ProjectPageControlProxies.DeleteDataset(application, log, ids[1]);
                ProjectPageControlProxies.WaitForDatasetCreationOrDeletion(application, log, 2);

                datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(),
                    datasetCount,
                    prefix + " - The number of datasets does not match the number of dataset IDs after the deletion of the first dataset.");
                assert.IsTrue(datasetIds.Contains(ids[2]), prefix + " - The second dataset was deleted but should not have been.");

                // Delete second child
                ProjectPageControlProxies.DeleteDataset(application, log, ids[2]);
                ProjectPageControlProxies.WaitForDatasetCreationOrDeletion(application, log, 1);

                datasetCount = ProjectPageControlProxies.GetNumberOfDatasetsViaProjectControl(application, log);
                datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                assert.AreEqual(
                    datasetIds.Count(),
                    datasetCount,
                    prefix + " - The number of datasets does not match the number of dataset IDs after the deletion of the second dataset.");
            }
            catch (RegressionTestFailedException e)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Failed with exception. Error: {0}", e);
                log.Error(prefix, message);
                result.AddError(prefix + " - " + message);
            }

            return result;
        }

        private TestResult VerifyDatasetInformation(Application application, Log log)
        {
            const string prefix = "Dataset information";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                // Start new project via File menu
                var projectPage = TabProxies.GetProjectPageTabItem(application, log);
                if (projectPage == null)
                {
                    MenuProxies.CreateNewProjectViaFileNewMenuItem(application, log);
                }

                projectPage = TabProxies.GetProjectPageTabItem(application, log);
                assert.IsNotNull(projectPage, prefix + " - The project page was not opened.");

                ProjectPageControlProxies.CreateChildDatasetForRoot(application, log);

                // Wait for datasets to be created
                ProjectPageControlProxies.WaitForDatasetCreationOrDeletion(application, log, 2);

                var datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                var ids = new List<int>(datasetIds);
                ids.Sort();

                // Set a name
                var name = "Dataset-Test-Name";
                ProjectPageControlProxies.DatasetName(application, log, ids[1], name);

                var storedName = ProjectPageControlProxies.DatasetName(application, log, ids[1]);
                assert.AreEqual(name, storedName, prefix + " - The written dataset name does not match the stored dataset name.");

                // Set a summary
                var summary = "Dataset-Test-Summary";
                ProjectPageControlProxies.DatasetSummary(application, log, ids[1], summary);

                var storedSummary = ProjectPageControlProxies.DatasetSummary(application, log, ids[1]);
                assert.AreEqual(summary, storedSummary, prefix + " - The written dataset summary does not match the stored dataset summary.");

                // Set focus away from the text control so that the changes 'stick' by clicking somewhere, in this case the project tab item.
                projectPage.Click();

                // Undo
                MenuProxies.UndoViaEditMenu(application, log);
                storedName = ProjectPageControlProxies.DatasetName(application, log, ids[1]);
                assert.AreEqual(name, storedName, prefix + " - The dataset name change was undone too early.");

                storedSummary = ProjectPageControlProxies.DatasetSummary(application, log, ids[1]);
                assert.IsTrue(string.IsNullOrEmpty(storedSummary), prefix + " - The change to the dataset summary was not undone.");

                // Undo 
                MenuProxies.UndoViaEditMenu(application, log);

                storedName = ProjectPageControlProxies.DatasetName(application, log, ids[1]);
                assert.IsTrue(string.IsNullOrEmpty(storedName), prefix + " - The change to the dataset name was not undone.");

                storedSummary = ProjectPageControlProxies.DatasetSummary(application, log, ids[1]);
                assert.IsTrue(string.IsNullOrEmpty(storedSummary), prefix + " - The change to the dataset summary was not undone.");

                // Redo
                MenuProxies.RedoViaEditMenu(application, log);
                storedName = ProjectPageControlProxies.DatasetName(application, log, ids[1]);
                assert.AreEqual(name, storedName, prefix + " - The change to the dataset name was not redone.");

                storedSummary = ProjectPageControlProxies.DatasetSummary(application, log, ids[1]);
                assert.IsTrue(string.IsNullOrEmpty(storedSummary), prefix + " - The change to the dataset summary was redone too early.");

                // Redo
                MenuProxies.RedoViaEditMenu(application, log);
                storedName = ProjectPageControlProxies.DatasetName(application, log, ids[1]);
                assert.AreEqual(name, storedName, prefix + " - The change to the dataset name was not redone.");

                storedSummary = ProjectPageControlProxies.DatasetSummary(application, log, ids[1]);
                assert.AreEqual(summary, storedSummary, prefix + " - The change to the dataset summary was not redone.");
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

        private TestResult VerifyActivateDataset(Application application, Log log)
        {
            const string prefix = "Dataset activation";
            var result = new TestResult();
            var assert = new Assert(result, log);
            try
            {
                // Start new project via File menu
                var projectPage = TabProxies.GetProjectPageTabItem(application, log);
                if (projectPage == null)
                {
                    MenuProxies.CreateNewProjectViaFileNewMenuItem(application, log);
                }

                projectPage = TabProxies.GetProjectPageTabItem(application, log);
                assert.IsNotNull(projectPage, prefix + " - The project page was not opened.");

                ProjectPageControlProxies.CreateChildDatasetForRoot(application, log);
                ProjectPageControlProxies.CreateChildDatasetForRoot(application, log);

                // Wait for datasets to be created
                ProjectPageControlProxies.WaitForDatasetCreationOrDeletion(application, log, 3);

                var datasetIds = ProjectPageControlProxies.GetDatasetIds(application, log);
                var ids = new List<int>(datasetIds);
                ids.Sort();

                ProjectPageControlProxies.ActivateDataset(application, log, ids[1]);

                var isDataset1Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[1]);
                assert.IsTrue(isDataset1Activated, prefix + " - Failed to activate the first dataset.");

                var isDataset2Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[2]);
                assert.IsFalse(isDataset2Activated, prefix + " - Activated the second dataset while it should not have been.");

                ProjectPageControlProxies.ActivateDataset(application, log, ids[2]);

                isDataset1Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[1]);
                assert.IsTrue(isDataset1Activated, prefix + " - Deactivated the first dataset when it should not have been.");

                isDataset2Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[2]);
                assert.IsTrue(isDataset2Activated, prefix + " - Failed to activate the second dataset.");

                // Undo
                MenuProxies.UndoViaEditMenu(application, log);

                isDataset1Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[1]);
                assert.IsTrue(isDataset1Activated, prefix + " - Deactivated the first dataset when it should not have been.");

                isDataset2Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[2]);
                assert.IsFalse(isDataset2Activated, prefix + " - Did not undo the activation state of the second dataset.");

                // Undo
                MenuProxies.UndoViaEditMenu(application, log);

                isDataset1Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[1]);
                assert.IsFalse(isDataset1Activated, prefix + " - Did not undo the activation state of the first dataset.");

                isDataset2Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[2]);
                assert.IsFalse(isDataset2Activated, prefix + " - Still did not undo the activation state of the second dataset.");

                // Redo
                MenuProxies.RedoViaEditMenu(application, log);

                isDataset1Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[1]);
                assert.IsTrue(isDataset1Activated, prefix + " - Did not redo the undone activation state of the first dataset.");

                isDataset2Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[2]);
                assert.IsFalse(isDataset2Activated, prefix + " - Redid the activation state of the second dataset when it should not have been.");

                // Redo
                MenuProxies.RedoViaEditMenu(application, log);

                isDataset1Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[1]);
                assert.IsTrue(isDataset1Activated, prefix + " - Changed the activation state of the first dataset when it should not have been.");

                isDataset2Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[2]);
                assert.IsTrue(isDataset2Activated, prefix + " - Did not redo the undone activation state of the second dataset.");

                ProjectPageControlProxies.DeactivateDataset(application, log, ids[1]);

                isDataset1Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[1]);
                assert.IsFalse(isDataset1Activated, prefix + " - Failed to deactivate the first dataset.");

                isDataset2Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[2]);
                assert.IsTrue(isDataset2Activated, prefix + " - Deactivated the second dataset when it should not have been.");

                ProjectPageControlProxies.DeactivateDataset(application, log, ids[2]);

                isDataset1Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[1]);
                assert.IsFalse(isDataset1Activated, prefix + " - Failed to deactivate the first dataset.");

                isDataset2Activated = ProjectPageControlProxies.IsDatasetActivated(application, log, ids[2]);
                assert.IsFalse(isDataset2Activated, prefix + " - Failed to deactivate the second dataset.");
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
    }
}
