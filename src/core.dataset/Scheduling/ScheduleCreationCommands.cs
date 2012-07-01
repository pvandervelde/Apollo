//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the commands for creating and updating stored schedules.
    /// </summary>
    internal sealed class ScheduleCreationCommands : IScheduleCreationCommands
    {
        /// <summary>
        /// The object that stores the schedules for the current dataset.
        /// </summary>
        private readonly IStoreSchedules m_Schedules;

        /// <summary>
        /// The object that tracks lock requests for the current dataset.
        /// </summary>
        private readonly ITrackDatasetLocks m_DatasetLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleCreationCommands"/> class.
        /// </summary>
        /// <param name="schedules">The collection of known schedules.</param>
        /// <param name="datasetLock">The object that tracks lock requests for the current dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedules"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="datasetLock"/> is <see langword="null" />.
        /// </exception>
        public ScheduleCreationCommands(IStoreSchedules schedules, ITrackDatasetLocks datasetLock)
        {
            {
                Lokad.Enforce.Argument(() => schedules);
                Lokad.Enforce.Argument(() => datasetLock);
            }

            m_Schedules = schedules;
            m_DatasetLock = datasetLock;
        }

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
        public Task<ScheduleInformation> AddNewSchedule(
            IEditableSchedule schedule,
            string name,
            string summary,
            string description,
            IEnumerable<IScheduleVariable> produces,
            IEnumerable<IScheduleDependency> dependencies)
        {
            var result = Task<ScheduleInformation>.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.BlockLocking();
                    try
                    {
                        return m_Schedules.Add(schedule, name, summary, description, produces, dependencies);
                    }
                    finally
                    {
                        m_DatasetLock.UnblockLocking(key);
                    }
                },
                TaskCreationOptions.None);

            return result;
        }

        /// <summary>
        /// Replaces the schedule current stored against the given ID with a new one.
        /// </summary>
        /// <param name="scheduleToUpdate">The ID of the schedule that should be replaced.</param>
        /// <param name="newSchedule">The new schedule.</param>
        /// <returns>A task that completes when the schedule has been replaced.</returns>
        public Task UpdateSchedule(ScheduleId scheduleToUpdate, IEditableSchedule newSchedule)
        {
            var result = Task.Factory.StartNew(
                () =>
                {
                    var key = m_DatasetLock.BlockLocking();
                    try
                    {
                        m_Schedules.Update(scheduleToUpdate, newSchedule);
                    }
                    finally
                    {
                        m_DatasetLock.UnblockLocking(key);
                    }
                },
                TaskCreationOptions.None);

            return result;
        }
    }
}
