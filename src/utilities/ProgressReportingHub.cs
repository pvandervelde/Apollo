//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Utilities
{
    /// <summary>
    /// Collects the progress reports from one or more <see cref="ITrackProgress"/> object
    /// and combines those results before passing them on.
    /// </summary>
    public sealed class ProgressReportingHub : ICollectProgressReports
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection that holds the progress reporters and their associated progress and estimated time values.
        /// </summary>
        private readonly Dictionary<ITrackProgress, Tuple<int, TimeSpan>> m_ReporterToProgressMap
            = new Dictionary<ITrackProgress, Tuple<int, TimeSpan>>();

        /// <summary>
        /// Adds a progress reporter to the collection of reporters that
        /// are being tracked.
        /// </summary>
        /// <param name="reporterToAdd">The reporter.</param>
        public void AddReporter(ITrackProgress reporterToAdd)
        {
            lock (m_Lock)
            {
                if (m_ReporterToProgressMap.ContainsKey(reporterToAdd))
                {
                    return;
                }

                reporterToAdd.OnStartProgress += ProcessStartProgress;
                reporterToAdd.OnProgress += ProcessNewProgress;
                reporterToAdd.OnStopProgress += ProcessStopProgress;
                m_ReporterToProgressMap.Add(reporterToAdd, new Tuple<int, TimeSpan>(-1, TimeSpan.FromTicks(-1)));
            }
        }

        private void ProcessStartProgress(object s, EventArgs e)
        {
            var reporter = s as ITrackProgress;
            if (reporter == null)
            {
                return;
            }

            bool notify = false;
            lock (m_Lock)
            {
                if (!m_ReporterToProgressMap.ContainsKey(reporter))
                {
                    return;
                }

                notify = !(from pair in m_ReporterToProgressMap
                           where pair.Value.Item1 > -1
                           select pair.Value).Any();

                m_ReporterToProgressMap[reporter] = new Tuple<int, TimeSpan>(0, TimeSpan.FromTicks(1));
            }

            if (notify)
            {
                RaiseOnStartProgress();
            }
        }

        private void ProcessNewProgress(object s, ProgressEventArgs e)
        {
            var reporter = s as ITrackProgress;
            if (reporter == null)
            {
                return;
            }

            double time = 0;
            double average = 0;
            lock (m_Lock)
            {
                if (!m_ReporterToProgressMap.ContainsKey(reporter))
                {
                    return;
                }

                m_ReporterToProgressMap[reporter] = new Tuple<int, TimeSpan>(e.Progress, e.EstimatedFinishingTime);
                time = (from pair in m_ReporterToProgressMap
                            where pair.Value.Item1 > -1
                            select (1.0 - (pair.Value.Item1 / 100.0)) * pair.Value.Item2.Ticks).Max();

                average = (from pair in m_ReporterToProgressMap
                           where pair.Value.Item1 > -1
                           select pair.Value.Item1).Average();
            }

            int progress = (int)Math.Round(average);
            progress = (progress <= 100) ? progress : 100;

            RaiseOnProgress(progress, new IndeterminateProgressMark(), TimeSpan.FromTicks((long)time));
        }

        private void ProcessStopProgress(object s, EventArgs e)
        {
            var reporter = s as ITrackProgress;
            if (reporter == null)
            {
                return;
            }

            bool notify = false;
            lock (m_Lock)
            {
                if (!m_ReporterToProgressMap.ContainsKey(reporter))
                {
                    return;
                }

                m_ReporterToProgressMap[reporter] = new Tuple<int, TimeSpan>(-1, TimeSpan.FromTicks(-1));

                // If there aren't any items with a progress value over -1
                // then we just reset the last one.
                notify = !(from pair in m_ReporterToProgressMap
                           where pair.Value.Item1 > -1
                           select pair.Value).Any();
            }

            if (notify)
            {
                RaiseOnStopProgress();
            }
        }

        /// <summary>
        /// Removes a progress reporter from the collection of reporters
        /// that are being tracked.
        /// </summary>
        /// <param name="reporterToRemove">The reporter.</param>
        public void RemoveReporter(ITrackProgress reporterToRemove)
        {
            lock (m_Lock)
            {
                if (!m_ReporterToProgressMap.ContainsKey(reporterToRemove))
                {
                    return;
                }

                reporterToRemove.OnProgress -= ProcessNewProgress;
                m_ReporterToProgressMap.Remove(reporterToRemove);
            }
        }

        /// <summary>
        /// Indicates that one or more progress reporters have indicated
        /// that a process has started.
        /// </summary>
        public event EventHandler<EventArgs> OnStartProgress;

        private void RaiseOnStartProgress()
        {
            var local = OnStartProgress;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Indicates that one or more progress reporters have reported
        /// new progress. The reported progress value is the combination
        /// of the progress of all the reporters.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnProgress;

        private void RaiseOnProgress(int progress, IProgressMark currentMark, TimeSpan estimatedTime)
        {
            var local = OnProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, currentMark, estimatedTime));
            }
        }

        /// <summary>
        /// Indicates that one or more progress reporters have indicated
        /// that a process has stopped.
        /// </summary>
        public event EventHandler<EventArgs> OnStopProgress;

        private void RaiseOnStopProgress()
        {
            var local = OnStopProgress;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
