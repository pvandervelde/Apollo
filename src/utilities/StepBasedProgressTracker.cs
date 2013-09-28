//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines methods used to track progress based on inputs from a user.
    /// </summary>
    public sealed class StepBasedProgressTracker : ITrackSteppingProgress
    {
        /// <summary>
        /// Indicates if the tracker has started tracking.
        /// </summary>
        private bool m_IsTracking;

        /// <summary>
        /// Provides the progress for the current step.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="mark">The action that is currently being processed.</param>
        /// <param name="hasErrors">A flag that indicates if there were any errors while processing the current action.</param>
        public void UpdateProgress(int progress, string mark, bool hasErrors)
        {
            if (m_IsTracking)
            {
                RaiseOnProgress(progress, mark, hasErrors);
            }
        }

        /// <summary>
        /// Starts the tracking of the progress.
        /// </summary>
        public void StartTracking()
        {
            if (!m_IsTracking)
            {
                m_IsTracking = true;
                RaiseOnStartProgress();
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

        /// <summary>
        /// Raises the startup progress event with the specified values.
        /// </summary>
        /// <param name="progress">The progress percentage. Should be between 0 and 100.</param>
        /// <param name="currentlyProcessing">The description of what is currently being processed.</param>
        /// <param name="hasErrors">A flag that indicates if there were any errors while processing the current action.</param>
        private void RaiseOnProgress(int progress, string currentlyProcessing, bool hasErrors)
        {
            var local = OnProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, currentlyProcessing, hasErrors));
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
            if (m_IsTracking)
            {
                m_IsTracking = false;
                RaiseOnStopProgress();
            }
        }

        /// <summary>
        /// Resets the progress tracker.
        /// </summary>
        public void Reset()
        {
            m_IsTracking = false;
        }
    }
}
