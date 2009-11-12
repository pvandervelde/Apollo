//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utils
{
    /// <summary>
    /// Defines the interface for objects which track progress.
    /// </summary>
    public interface ITrackProgress
    {
        /// <summary>
        /// Starts the tracking of the progress.
        /// </summary>
        /// <param name="progress">The function called each time the progress event is invoked.</param>
        void StartTracking(Action<int, IProgressMark> progress);

        /// <summary>
        /// Marks the current time with the specified marker.
        /// </summary>
        /// <param name="mark">The mark.</param>
        void Mark(IProgressMark mark);

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
