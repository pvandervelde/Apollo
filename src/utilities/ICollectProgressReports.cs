//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects that collect progress reports from 
    /// multiple sources.
    /// </summary>
    public interface ICollectProgressReports
    {
        /// <summary>
        /// Adds a progress reporter to the collection of reporters that
        /// are being tracked.
        /// </summary>
        /// <param name="reporterToAdd">The reporter.</param>
        void AddReporter(ITrackProgress reporterToAdd);

        /// <summary>
        /// Removes a progress reporter from the collection of reporters
        /// that are being tracked.
        /// </summary>
        /// <param name="reporterToRemove">The reporter.</param>
        void RemoveReporter(ITrackProgress reporterToRemove);

        /// <summary>
        /// Indicates that one or more progress reporters have indicated
        /// that a process has started.
        /// </summary>
        event EventHandler<EventArgs> OnStartProgress;

        /// <summary>
        /// Indicates that one or more progress reporters have reported
        /// new progress. The reported progress value is the combination
        /// of the progress of all the reporters.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnProgress;

        /// <summary>
        /// Indicates that one or more progress reporters have indicated
        /// that a process has stopped.
        /// </summary>
        event EventHandler<EventArgs> OnStopProgress;
    }
}
