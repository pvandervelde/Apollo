//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Test.UI.Explorer
{
    /// <summary>
    /// Indicates the final status of the test, either pass or fail.
    /// </summary>
    internal enum TestStatus
    {
        /// <summary>
        /// The test has no result.
        /// </summary>
        None,

        /// <summary>
        /// The test has failed.
        /// </summary>
        Failed,
        
        /// <summary>
        /// The test has passed.
        /// </summary>
        Passed,
    }
}
