//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Test.Regression.Explorer.Reporting
{
    /// <summary>
    /// Defines the interface for objects that build reports.
    /// </summary>
    internal interface IReporter
    {
        /// <summary>
        /// Initializes the report.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds a new information report entry.
        /// </summary>
        /// <param name="message">The information message for the report entry.</param>
        void AddInformationalMessage(string message);

        /// <summary>
        /// Adds a new error message to the report.
        /// </summary>
        /// <param name="message">The error message for the report entry.</param>
        void AddErrorMessage(string message);

        /// <summary>
        /// Completes the report.
        /// </summary>
        void Complete();
    }
}
