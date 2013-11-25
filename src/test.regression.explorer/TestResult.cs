//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Test.UI.Explorer
{
    /// <summary>
    /// Stores the result of a test.
    /// </summary>
    internal sealed class TestResult
    {
        private readonly List<string> m_Errors
            = new List<string>();

        private TestStatus m_Status = TestStatus.Passed;

        /// <summary>
        /// Adds an error to the test result error collection.
        /// </summary>
        /// <param name="message">The error message.</param>
        public void AddError(string message)
        {
            {
                Lokad.Enforce.Argument(() => message);
                Lokad.Enforce.Argument(() => message, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Errors.Add(message);
            m_Status = TestStatus.Failed;
        }

        /// <summary>
        /// Gets the collection that contains all the error messages.
        /// </summary>
        public IEnumerable<string> Errors
        {
            get
            {
                return m_Errors;
            }
        }

        /// <summary>
        /// Gets the final status of the test.
        /// </summary>
        public TestStatus Status
        {
            get
            {
                return m_Status;
            }
        }
    }
}
