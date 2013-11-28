//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Test.Regression.Explorer
{
    /// <summary>
    /// Defines the interface for objects that provide verification of one or more parts of the user interface.
    /// </summary>
    internal interface IUserInterfaceVerifier
    {
        /// <summary>
        /// Returns a collection of tests that should be executed.
        /// </summary>
        /// <returns>The list of test cases that should be executed for the current verifier.</returns>
        IEnumerable<TestCase> TestsToExecute();
    }
}
