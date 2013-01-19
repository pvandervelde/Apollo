//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Wpf.Profiling
{
    /// <summary>
    /// Stores a collection of timing reports.
    /// </summary>
    public sealed class TimingReportCollection : IEnumerable<IProfilingTimeReport>, INotifyCollectionChanged
    {
        private readonly List<IProfilingTimeReport> m_Reports = new List<IProfilingTimeReport>();

        /// <summary>
        /// Adds a new report to the storage.
        /// </summary>
        /// <param name="reportToAdd">The report that should be added.</param>
        public void Add(IProfilingTimeReport reportToAdd)
        {
            {
                Lokad.Enforce.Argument(() => reportToAdd);
            }

            m_Reports.Add(reportToAdd);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, reportToAdd);
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerator{T}" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<IProfilingTimeReport> GetEnumerator()
        {
            foreach (var report in m_Reports)
            {
                yield return report;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// An event raised the collection changes.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Event is inherited from the INotifyCollectionChanged interface.")]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action, IProfilingTimeReport report)
        {
            var local = CollectionChanged;
            if (local != null)
            {
                local(this, new NotifyCollectionChangedEventArgs(action, report));
            }
        }
    }
}
