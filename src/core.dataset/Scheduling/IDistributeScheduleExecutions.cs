//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that schedule executions across multiple processes.
    /// </summary>
    internal interface IDistributeScheduleExecutions
    {
        /// <summary>
        /// Determines the most suitable execution location for the given schedule and then starts the execution in that location.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule that should be executed.</param>
        /// <param name="scheduleParameters">The collection of parameters that have to be provided to the schedule before executing.</param>
        /// <param name="executionInfo">The object that provides information about the schedule that is currently being executed.</param>
        /// <param name="executeOutOfProcess">A flag indicating if the schedule should be executed in another processor or not.</param>
        /// <returns>
        /// The token that is related to the execution of the given schedule.
        /// </returns>
        /// <remarks>
        /// For local schedules the cancellation token is checked regularly and the schedule execution is cancelled on request. For
        /// remote schedule execution setting the cancellation token means that a termination signal is send to the remote
        /// application.
        /// </remarks>
        IExecuteSchedules Execute(
            ScheduleId scheduleId, 
            IEnumerable<IScheduleVariable> scheduleParameters = null, 
            ScheduleExecutionInfo executionInfo = null, 
            bool executeOutOfProcess = false);
    }
}
