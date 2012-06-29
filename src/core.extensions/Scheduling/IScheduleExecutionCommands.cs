//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Apollo.Core.Base.Communication;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines a <see cref="ICommandSet"/> used to control the execution of a schedule.
    /// </summary>
    public interface IScheduleExecutionCommands : ICommandSet
    {
        /// <summary>
        /// Starts the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToExecute">The ID of the schedule that should be executed.</param>
        /// <returns>A task that will finish once the schedule execution completes.</returns>
        Task Start(ScheduleId scheduleToExecute);

        /// <summary>
        /// Pauses the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToExecute">The ID of the schedule that should be executed.</param>
        /// <returns>A task that will finish once the schedule execution has been paused.</returns>
        Task Pause(ScheduleId scheduleToPause);

        /// <summary>
        /// Stops the execution of the given schedule.
        /// </summary>
        /// <param name="scheduleToExecute">The ID of the schedule that should be executed.</param>
        /// <returns>A task that will finish once the schedule execution has been stopped.</returns>
        Task Stop(ScheduleId scheduleToStop);
    }
}
