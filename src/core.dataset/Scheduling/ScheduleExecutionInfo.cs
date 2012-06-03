//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Threading;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Stores the information used during the execution of a schedule.
    /// </summary>
    internal sealed class ScheduleExecutionInfo
    {
        /// <summary>
        /// The info object for the parent schedule.
        /// </summary>
        private readonly ScheduleExecutionInfo m_Parent;

        /// <summary>
        /// The token that is used to indicate if the local schedule execution has been cancelled.
        /// </summary>
        private readonly CancellationTokenSource m_LocalCancellationSource;

        /// <summary>
        /// The token that is used to indicate if the local schedule or any of it's parents has been cancelled.
        /// </summary>
        private readonly CancellationTokenSource m_CombinedCancellationSource;

        /// <summary>
        /// The object that indicates if the schedule execution should be paused or not.
        /// </summary>
        private readonly SchedulePauseHandler m_PauseHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleExecutionInfo"/> class.
        /// </summary>
        public ScheduleExecutionInfo()
        {
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
        /// Sends the message that the schedule execution should be cancelled.
        /// </summary>
        public void CancelScheduleExecution()
        {
            // We only want to cancel the local schedule execution
            m_LocalCancellationSource.Cancel();
        }

        /// <summary>
        /// Gets the token that is used to indicate if the schedule execution should be cancelled.
        /// </summary>
        public CancellationToken Cancellation
        {
            get
            {
                // We want to cancel the schedule execution even if the parent gets cancelled
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
    }
}
