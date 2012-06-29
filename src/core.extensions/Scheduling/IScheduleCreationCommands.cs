//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the commands for the creation and editing of schedules.
    /// </summary>
    public interface IScheduleCreationCommands : ICommandSet
    {
        /// <summary>
        /// Adds a new schedule to the storage and provides an information object
        /// that describes the new schedule.
        /// </summary>
        /// <param name="schedule">The new schedule.</param>
        /// <param name="name">The name of the schedule.</param>
        /// <param name="summary">The summary of the schedule.</param>
        /// <param name="description">The description of the schedule.</param>
        /// <param name="produces">The variables that are affected when the schedule is executed.</param>
        /// <param name="dependencies">The dependencies that are required for the execution of the schedule.</param>
        /// <returns>A task which returns the schedule information.</returns>
        Task<ScheduleInformation> AddNewSchedule(
            IEditableSchedule schedule, 
            string name, 
            string summary, 
            string description,
            IEnumerable<IScheduleVariable> produces,
            IEnumerable<IScheduleDependency> dependencies);

        /// <summary>
        /// Replaces the schedule current stored against the given ID with a new one.
        /// </summary>
        /// <param name="scheduleToUpdate">The ID of the schedule that should be replaced.</param>
        /// <param name="newSchedule">The new schedule.</param>
        /// <returns>A task that completes when the schedule has been replaced.</returns>
        Task UpdateSchedule(ScheduleId scheduleToUpdate, IEditableSchedule newSchedule);
    }
}
