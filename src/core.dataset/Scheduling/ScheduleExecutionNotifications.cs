//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Provides the methods for notification changes related to schedule executions.
    /// </summary>
    internal sealed class ScheduleExecutionNotifications : IScheduleExecutionNotificationInvoker
    {
        /// <summary>
        /// An event raised when the execution of the schedule starts.
        /// </summary>
        public event EventHandler<ScheduleEventArgs> OnStart;

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnStart"/> event.
        /// </summary>
        /// <param name="schedule">The schedule for which execution has just started.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        public void RaiseOnStart(ScheduleId schedule)
        {
            var local = OnStart;
            if (local != null)
            {
                local(this, new ScheduleEventArgs(schedule));
            }
        }

        /// <summary>
        /// An event raised when the execution of the schedule has been paused.
        /// </summary>
        public event EventHandler<ScheduleEventArgs> OnPause;

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnPause"/> event.
        /// </summary>
        /// <param name="schedule">The schedule for which execution has just been paused.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        public void RaiseOnPause(ScheduleId schedule)
        {
            var local = OnPause;
            if (local != null)
            {
                local(this, new ScheduleEventArgs(schedule));
            }
        }

        /// <summary>
        /// An event raised when there is progress in the execution of the schedule.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnExecutionProgress;

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnExecutionProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        public void RaiseOnExecutionProgress(int progress, IProgressMark currentlyProcessing)
        {
            var local = OnExecutionProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, currentlyProcessing));
            }
        }

        /// <summary>
        /// An event raised when a vertex is being processed.
        /// </summary>
        public event EventHandler<ExecutingVertexEventArgs> OnVertexProcess;

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnVertexProcess"/> event.
        /// </summary>
        /// <param name="schedule">The schedule for which execution has just started.</param>
        /// <param name="vertex">The index of the vertex which is being processed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        public void RaiseOnVertexProcess(ScheduleId schedule, int vertex)
        {
            var local = OnVertexProcess;
            if (local != null)
            {
                local(this, new ExecutingVertexEventArgs(schedule, vertex));
            }
        }

        /// <summary>
        /// An event raised when the execution of the schedule has been stopped, either
        /// due to the user stopping the execution directly or if the schedule executor 
        /// reaches the end of the schedule.
        /// </summary>
        public event EventHandler<ScheduleEventArgs> OnFinish;

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnFinish"/> event.
        /// </summary>
        /// <param name="schedule">The schedule for which execution has just completed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        public void RaiseOnFinish(ScheduleId schedule)
        {
            var local = OnFinish;
            if (local != null)
            {
                local(this, new ScheduleEventArgs(schedule));
            }
        }
    }
}
