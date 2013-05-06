//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities;
using Nuclei.Progress;

namespace Apollo.UI.Console
{
    /// <summary>
    /// An <see cref="ITrackProgress"/> implementation that does nothing.
    /// </summary>
    internal sealed class FakeProgressTracker : ITrackProgress
    {
        /// <summary>
        /// Starts the tracking of the progress.
        /// </summary>
        /// <exception cref="CurrentProgressMarkNotSetException">Thrown if no <see cref="IProgressMark"/> has been set.</exception>
        public void StartTracking()
        {
            // do nothing on purpose. We don't really care
        }

        /// <summary>
        /// Marks the current time with the specified marker.
        /// </summary>
        /// <param name="progressMark">The progress mark.</param>
        public void Mark(IProgressMark progressMark)
        {
        }

        /// <summary>
        /// Occurs when a new mark is provided to the tracker.
        /// </summary>
        public event EventHandler<ProgressMarkEventArgs> OnMarkAdded;

        private void RaiseOnMarkAdded(IProgressMark mark)
        {
            var local = OnMarkAdded;
            if (local != null)
            {
                local(this, new ProgressMarkEventArgs(mark));
            }
        }

        /// <summary>
        /// Occurs when the process for which progress is 
        /// being reported is starting.
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
        /// Occurs when there is a change in the progress of the system
        /// startup.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnProgress;

        private void RaiseOnProgress(int progress, IProgressMark mark)
        {
            var local = OnProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, mark));
            }
        }

        /// <summary>
        /// Occurs when the process for which progress is
        /// being reported is finished.
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

        /// <summary>
        /// Stops the tracking of the progress.
        /// </summary>
        public void StopTracking()
        {
            // Do nothing
        }

        /// <summary>
        /// Resets the progress tracker.
        /// </summary>
        public void Reset()
        {
            // Still do nothing
        }
    }
}
