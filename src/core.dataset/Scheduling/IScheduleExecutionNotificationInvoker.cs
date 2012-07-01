//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that implement <see cref="IScheduleExecutionNotifications"/>
    /// and need to provide external objects with access to their events.
    /// </summary>
    internal interface IScheduleExecutionNotificationInvoker : IScheduleExecutionNotifications
    {
        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnStart"/> event.
        /// </summary>
        /// <param name="schedule">The schedule for which execution has just started.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnStart(ScheduleId schedule);

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnPause"/> event.
        /// </summary>
        /// <param name="schedule">The schedule for which execution has just been paused.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnPause(ScheduleId schedule);

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnExecutionProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage, ranging from 0 to 100.</param>
        /// <param name="currentlyProcessing">The action that is currently being processed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnExecutionProgress(int progress, IProgressMark currentlyProcessing);

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnVertexProcess"/> event.
        /// </summary>
        /// <param name="schedule">The schedule for which execution has just started.</param>
        /// <param name="vertex">The index of the vertex which is being processed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnVertexProcess(ScheduleId schedule, int vertex);

        /// <summary>
        /// Raises the <see cref="IScheduleExecutionNotifications.OnFinish"/> event.
        /// </summary>
        /// <param name="schedule">The schedule for which execution has just completed.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnFinish(ScheduleId schedule);
    }
}
