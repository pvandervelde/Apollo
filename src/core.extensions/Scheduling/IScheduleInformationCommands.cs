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
    /// Defines the commands used to retrieve information about a schedule and its components.
    /// </summary>
    public interface IScheduleInformationCommands : ICommandSet
    {
        /// <summary>
        /// Returns a collection containing all the known actions that can be used in
        /// a schedule.
        /// </summary>
        /// <returns>A task that returns the collection containing all the known schedule actions.</returns>
        Task<IEnumerable<ScheduleActionInformation>> AvailableActions();

        /// <summary>
        /// Returns a collection containing all the known conditions that can be used in a schedule.
        /// </summary>
        /// <returns>A task that returns the collection containing all the known schedule conditions.</returns>
        Task<IEnumerable<ScheduleConditionInformation>> AvailableConditions();

        /// <summary>
        /// Returns a collection containing all the known schedules.
        /// </summary>
        /// <returns>A task that returns the collection containing all the known schedules.</returns>
        Task<IEnumerable<ScheduleInformation>> AvailableSchedules();

        /// <summary>
        /// Returns the schedule description for the schedule with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule for which the information is requested.</param>
        /// <returns>A task that returns the desired schedule description.</returns>
        Task<ScheduleInformation> ScheduleInformation(ScheduleId id);

        /// <summary>
        /// Returns the schedule with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <returns>A task that returns the desired schedule.</returns>
        Task<IEditableSchedule> Schedule(ScheduleId id);
    }
}
