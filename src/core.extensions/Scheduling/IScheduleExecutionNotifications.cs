//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Provides notifications about the execution of a schedule.
    /// </summary>
    public interface IScheduleExecutionNotifications : INotificationSet
    {
        /// <summary>
        /// An event raised when the execution of the schedule starts.
        /// </summary>
        event EventHandler<ScheduleEventArgs> OnStart;

        /// <summary>
        /// An event raised when the execution of the schedule has been paused.
        /// </summary>
        event EventHandler<ScheduleEventArgs> OnPause;

        /// <summary>
        /// An event raised when there is progress in the execution of the schedule.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnExecutionProgress;

        /// <summary>
        /// An event raised when a vertex is being processed.
        /// </summary>
        event EventHandler<ExecutingVertexEventArgs> OnVertexProcess;

        /// <summary>
        /// An event raised when the execution of the schedule has been stopped, either
        /// due to the user stopping the execution directly or if the schedule executor 
        /// reaches the end of the schedule.
        /// </summary>
        event EventHandler<ScheduleEventArgs> OnFinish;
    }
}
