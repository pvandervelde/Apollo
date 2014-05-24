//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Stores the information used during the execution of a schedule.
    /// </summary>
    internal sealed class ScheduleExecutionInfo : IDisposable
    {
        /// <summary>
        /// The info object for the parent schedule.
        /// </summary>
        private readonly ScheduleExecutionInfo m_Parent;

        /// <summary>
        /// The task scheduler that is used to execute the schedule.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// The token that is used to indicate if the local schedule execution has been canceled.
        /// </summary>
        private readonly CancellationTokenSource m_LocalCancellationSource;

        /// <summary>
        /// The token that is used to indicate if the local schedule or any of it's parents has been canceled.
        /// </summary>
        private readonly CancellationTokenSource m_CombinedCancellationSource;

        /// <summary>
        /// The object that indicates if the schedule execution should be paused or not.
        /// </summary>
        private readonly SchedulePauseHandler m_PauseHandler;

        /// <summary>
        /// Indicates if the current endpoint has been disposed.
        /// </summary>
        private volatile bool m_IsDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleExecutionInfo"/> class.
        /// </summary>
        public ScheduleExecutionInfo()
        {
            m_Scheduler = TaskScheduler.Default;

            m_LocalCancellationSource = new CancellationTokenSource();
            m_CombinedCancellationSource = m_LocalCancellationSource;

            m_PauseHandler = new SchedulePauseHandler();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleExecutionInfo"/> class.
        /// </summary>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> that should be used to execute the schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduler"/> is <see langword="null" />.
        /// </exception>
        public ScheduleExecutionInfo(TaskScheduler scheduler)
        {
            {
                Lokad.Enforce.Argument(() => scheduler);
            }

            m_Scheduler = scheduler;

            m_LocalCancellationSource = new CancellationTokenSource();
            m_CombinedCancellationSource = m_LocalCancellationSource;

            m_PauseHandler = new SchedulePauseHandler();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleExecutionInfo"/> class.
        /// </summary>
        /// <param name="parent">The information object for the parent schedule.</param>
        public ScheduleExecutionInfo(ScheduleExecutionInfo parent)
            : this()
        {
            m_Parent = parent;
            if (m_Parent != null)
            {
                m_CombinedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
                    m_Parent.Cancellation, 
                    m_LocalCancellationSource.Token);
            }
        }

        /// <summary>
        /// Gets the <see cref="TaskScheduler"/> that should be used to execute the schedule.
        /// </summary>
        public TaskScheduler TaskScheduler
        {
            get
            {
                return (m_Parent != null) ? m_Parent.TaskScheduler : m_Scheduler;
            }
        }

        /// <summary>
        /// Sends the message that the schedule execution should be canceled.
        /// </summary>
        public void CancelScheduleExecution()
        {
            // We only want to cancel the local schedule execution
            m_LocalCancellationSource.Cancel();
        }

        /// <summary>
        /// Gets the token that is used to indicate if the schedule execution should be canceled.
        /// </summary>
        public CancellationToken Cancellation
        {
            get
            {
                // We want to cancel the schedule execution even if the parent gets canceled
                return m_CombinedCancellationSource.Token;
            }
        }

        /// <summary>
        /// Gets the object that is used to indicate if the schedule should be paused or not.
        /// </summary>
        public SchedulePauseHandler PauseHandler
        {
            get
            {
                return m_PauseHandler;
            }
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
            m_CombinedCancellationSource.Dispose();
            m_LocalCancellationSource.Dispose();
            m_PauseHandler.Dispose();
        }
    }
}
