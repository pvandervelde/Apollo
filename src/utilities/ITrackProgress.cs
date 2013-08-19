//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects which track progress.
    /// </summary>
    public interface ITrackProgress
    {
        /// <summary>
        /// Starts the tracking of the progress.
        /// </summary>
        void StartTracking();

        /// <summary>
        /// Occurs when the process for which progress is
        /// being reported is starting.
        /// </summary>
        event EventHandler<EventArgs> OnStartProgress;

        /// <summary>
        /// Occurs when there is a change in the progress.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnProgress;

        /// <summary>
        /// Occurs when the process for which progress is
        /// being reported is finished.
        /// </summary>
        event EventHandler<EventArgs> OnStopProgress;

        /// <summary>
        /// Stops the tracking of the progress.
        /// </summary>
        void StopTracking();

        /// <summary>
        /// Resets the progress tracker.
        /// </summary>
        void Reset();
    }
}
