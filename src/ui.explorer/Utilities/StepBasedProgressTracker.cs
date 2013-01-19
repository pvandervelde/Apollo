//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Utilities.Progress;

namespace Apollo.UI.Explorer.Utilities
{
    /// <summary>
    /// Defines methods used to track progress based on inputs from a user.
    /// </summary>
    internal sealed class StepBasedProgressTracker : ITrackSteppingProgress
    {
        /// <summary>
        /// The mark which indicates what is currently being processed.
        /// </summary>
        private IProgressMark m_CurrentMark;

        /// <summary>
        /// Indicates if the tracker has started tracking.
        /// </summary>
        private bool m_IsTracking;

        /// <summary>
        /// Provides the progress for the current step.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="estimatedTime">
        ///     The amount of time it will take to finish the entire task from start to finish. Can be negative 
        ///     if no time is known.
        /// </param>
        public void UpdateProgress(int progress, TimeSpan estimatedTime)
        {
            if (m_IsTracking)
            {
                RaiseOnProgress(progress, m_CurrentMark, estimatedTime);
            }
        }

        /// <summary>
        /// Provides the progress for the current step.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="mark">The action that is currently being processed.</param>
        /// <param name="estimatedTime">
        ///     The amount of time it will take to finish the entire task from start to finish. Can be negative 
        ///     if no time is known.
        /// </param>
        public void UpdateProgress(int progress, IProgressMark mark, TimeSpan estimatedTime)
        {
            Mark(mark);
            UpdateProgress(progress, estimatedTime);
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
        /// Marks the current time with the specified marker.
        /// </summary>
        /// <param name="progressMark">The progress mark.</param>
        public void Mark(IProgressMark progressMark)
        {
            if (m_CurrentMark != progressMark)
            {
                m_CurrentMark = progressMark;
                RaiseOnMarkAdded(progressMark);
            }
        }

        /// <summary>
        /// Occurs when a new mark is provided to the tracker.
        /// </summary>
        public event EventHandler<ProgressMarkEventArgs> OnMarkAdded;

        /// <summary>
        /// Raises the mark added event.
        /// </summary>
        /// <param name="mark">The progress mark.</param>
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

        /// <summary>
        /// Raises the startup progress event with the specified values.
        /// </summary>
        /// <param name="progress">The progress percentage. Should be between 0 and 100.</param>
        /// <param name="currentlyProcessing">The description of what is currently being processed.</param>
        /// <param name="estimatedTime">
        ///     The amount of time it will take to finish the entire task from start to finish. Can be negative 
        ///     if no time is known.
        /// </param>
        private void RaiseOnProgress(int progress, IProgressMark currentlyProcessing, TimeSpan estimatedTime)
        {
            var local = OnProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, currentlyProcessing, estimatedTime));
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
            m_CurrentMark = null;
        }
    }
}
