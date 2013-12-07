//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Test.Regression.Explorer.Reporting
{
    /// <summary>
    /// A <see cref="IReporter"/> that writes to the console.
    /// </summary>
    internal sealed class ConsoleReporter : IReporter
    {
        /// <summary>
        /// Initializes the report.
        /// </summary>
        public void Initialize()
        {
            // Do nothing for now
        }

        /// <summary>
        /// Adds a new information report entry.
        /// </summary>
        /// <param name="message">The information message for the report entry.</param>
        public void AddInformationalMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Adds a new error message to the report.
        /// </summary>
        /// <param name="message">The error message for the report entry.</param>
        public void AddErrorMessage(string message)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Completes the report.
        /// </summary>
        public void Complete()
        {
            // Do nothing
        }
    }
}
