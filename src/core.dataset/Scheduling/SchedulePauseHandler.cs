//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines methods for handling pausing the execution of a schedule.
    /// </summary>
    internal sealed class SchedulePauseHandler : IDisposable
    {
        private readonly ManualResetEventSlim m_PauseEvent
            = new ManualResetEventSlim(true);

        /// <summary>
        /// Indicates if the current endpoint has been disposed.
        /// </summary>
        private volatile bool m_IsDisposed = false;

        /// <summary>
        /// Gets a value indicating whether the schedule should be paused or not.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return !m_PauseEvent.IsSet;
            }
        }

        /// <summary>
        /// Indicates that the schedule should be paused.
        /// </summary>
        public void Pause()
        {
            m_PauseEvent.Reset();
        }

        /// <summary>
        /// Indicates that the schedule should not be paused anymore.
        /// </summary>
        public void Unpause()
        {
            m_PauseEvent.Set();
        }

        /// <summary>
        /// Blocks the current thread until the schedule should no longer be paused.
        /// </summary>
        /// <param name="token">The cancellation token that can be used to cancel the current action.</param>
        public void WaitForUnPause(CancellationToken token)
        {
            m_PauseEvent.Wait(token);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed)
            {
                // We've already disposed of the channel. Job done.
                return;
            }

            m_IsDisposed = true;
            m_PauseEvent.Dispose();
        }
    }
}
