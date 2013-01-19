//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Extensions.Scheduling;
using Utilities.Progress;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that handle schedule execution.
    /// </summary>
    internal interface IExecuteSchedules
    {
        /// <summary>
        /// Gets the ID of the schedule that is being executed by the current executor.
        /// </summary>
        ScheduleId Schedule 
        { 
            get; 
        }

        /// <summary>
        /// Gets the collection of parameters that were provided when the schedule execution started.
        /// </summary>
        IEnumerable<IScheduleVariable> Parameters
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the schedule is being executed in the current application or not.
        /// </summary>
        bool IsLocal
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the schedule is currently being executed.
        /// </summary>
        bool IsRunning
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the schedule execution is currently paused.
        /// </summary>
        bool IsPaused
        {
            get;
        }

        /// <summary>
        /// Starts the execution of the schedule if it is currently not being executed.
        /// </summary>
        /// <param name="scheduleParameters">The collection of parameters that have to be provided to the schedule before executing.</param>
        void Start(IEnumerable<IScheduleVariable> scheduleParameters = null);

        /// <summary>
        /// Pauses the execution of the schedule. The execution of the schedule
        /// can be resumed from this state.
        /// </summary>
        void Pause();

        /// <summary>
        /// Stops the execution of the schedule. The execution of the schedule cannot
        /// be resumed from this state.
        /// </summary>
        void Stop();

        /// <summary>
        /// An event raised when the execution of the schedule starts.
        /// </summary>
        event EventHandler<EventArgs> OnStart;

        /// <summary>
        /// An event raised when the execution of the schedule has been paused.
        /// </summary>
        event EventHandler<EventArgs> OnPause;

        /// <summary>
        /// An event raised when a vertex is being processed.
        /// </summary>
        event EventHandler<ExecutingVertexEventArgs> OnVertexProcess;

        /// <summary>
        /// An event raised when there is progress in the execution of the schedule.
        /// </summary>
        event EventHandler<ProgressEventArgs> OnExecutionProgress;

        /// <summary>
        /// An event raised when the execution of the schedule has been stopped, either
        /// due to the user stopping the execution directly or if the schedule executor 
        /// reaches the end of the schedule.
        /// </summary>
        event EventHandler<ScheduleExecutionStateEventArgs> OnFinish;
    }
}
