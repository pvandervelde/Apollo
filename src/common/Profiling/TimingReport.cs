//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.UI.Common.Profiling
{
    /// <summary>
    /// Stores the timing results for a given timing interval.
    /// </summary>
    public sealed class TimingReport : ITimingReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimingReport"/> class.
        /// </summary>
        /// <param name="description">The description of the timing measurement.</param>
        /// <param name="results">The timing results.</param>
        public TimingReport(string description, string results)
        {
            {
                Lokad.Enforce.Argument(() => description);
                Lokad.Enforce.Argument(() => description, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => results);
                Lokad.Enforce.Argument(() => results, Lokad.Rules.StringIs.NotEmpty);
            }

            Description = description;
            TimingResults = results;
        }

        /// <summary>
        /// Gets the description for the timing report.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the timing results in textual format.
        /// </summary>
        public string TimingResults
        {
            get;
            private set;
        }
    }
}
