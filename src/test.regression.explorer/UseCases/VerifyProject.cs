//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
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
                    // new TestStep("New dataset", VerifyDatasetCreation),
                    // new TestStep("Update dataset information", VerifyDatasetInformation),
                    // new TestStep("ActivateDataset", VerifyActivateDataset),
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
                assert.IsNull(projectPage, prefix + " - The project page was not closed.");


                // Set a name

                // Set a summary

                // Undo

                // Undo 

                // Redo

                // Redo

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
            throw new NotImplementedException();
        }

        private TestResult VerifyDatasetInformation(Application application, Log log)
        {
            throw new NotImplementedException();
        }

        private TestResult VerifyActivateDataset(Application application, Log log)
        {
            throw new NotImplementedException();
        }
    }
}
