//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.UI.Wpf.Profiling
{
    /// <summary>
    /// Defines the interface for objects that store information about a profiling report.
    /// </summary>
    public interface IProfilingTimeReport
    {
        /// <summary>
        /// Gets the description for the timing report.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Gets the timing results in textual format.
        /// </summary>
        string TimingResults
        {
            get;
        }
    }
}
