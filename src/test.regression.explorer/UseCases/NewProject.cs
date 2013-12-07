//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Test.Regression.Explorer.UseCases
{
    /// <summary>
    /// Verifies that it is possible to create a new project and
    /// that it is possible to manipulate that project via the user interface.
    /// </summary>
    internal sealed class NewProject : IUserInterfaceVerifier
    {
        /// <summary>
        /// Returns a collection of tests that should be executed.
        /// </summary>
        /// <returns>The list of test cases that should be executed for the current verifier.</returns>
        public IEnumerable<TestStep> TestsToExecute()
        {
            return new List<TestStep>
                {
                    // new TestStep("Tab behaviour", VerifyTabBehaviour),
                    // Create new dataset
                    // Update project settings + undo + redo
                    // Update dataset settings + undo + redo
                    // Activate dataset
                    // Deactivate dataset
                    // Add child
                    // Remove child
                };
        }
    }
}
