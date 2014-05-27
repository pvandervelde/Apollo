//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Base;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Profiling;

namespace Apollo.UI.Wpf.Profiling
{
    /// <summary>
    /// Provides an easy way to get a timing interval and store the interval so that it can be displayed in the UI.
    /// </summary>
    internal sealed class TimingIntervalHelper : IDisposable
    {
        /// <summary>
        /// The object that stores all the timing intervals.
        /// </summary>
        private readonly IGenerateTimingReports m_Storage;

        /// <summary>
        /// The collection that stores the timing reports.
        /// </summary>
        private readonly TimingReportCollection m_Collection;

        /// <summary>
        /// The function that transforms the report to a string.
        /// </summary>
        private readonly Func<TimingReport, string> m_Transformer;

        /// <summary>
        /// The current interval.
        /// </summary>
        private readonly ITimerInterval m_Interval;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingIntervalHelper"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides access to the system wide diagnostics.</param>
        /// <param name="timingStorage">The object that generates reports for a stored interval.</param>
        /// <param name="collection">The object that stores the generated reports.</param>
        /// <param name="reportTransformer">The function which transforms a report into a string.</param>
        /// <param name="description">The description for the current interval.</param>
        public TimingIntervalHelper(
            SystemDiagnostics diagnostics,
            IGenerateTimingReports timingStorage,
            TimingReportCollection collection, 
            Func<TimingReport, string> reportTransformer,
            string description)
        {
            {
                Debug.Assert(diagnostics != null, "The diagnostics object should not be a null reference.");
                Debug.Assert(timingStorage != null, "The storage object should not be a null reference.");
                Debug.Assert(collection != null, "The collection object should not be a null reference.");
                Debug.Assert(reportTransformer != null, "The report transformer object should not be a null reference.");
                Debug.Assert(!string.IsNullOrWhiteSpace(description), "The description string should not be a null reference or an empty string.");
            }

            m_Storage = timingStorage;
            m_Collection = collection;
            m_Transformer = reportTransformer;
            m_Interval = (ITimerInterval)diagnostics.Profiler.Measure(BaseConstants.TimingGroup, description);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_Interval.Dispose();

            var report = m_Storage.ForInterval(m_Interval);
            m_Collection.Add(new ProfilingTimeReport(m_Interval.Description, m_Transformer(report)));
        }
    }
}
