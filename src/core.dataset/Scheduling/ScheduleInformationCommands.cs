//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the commands that provide information about the available schedules.
    /// </summary>
    internal sealed class ScheduleInformationCommands : IScheduleInformationCommands
    {
        /// <summary>
        /// The collection that stores the actions.
        /// </summary>
        private readonly IStoreScheduleActions m_Actions;

        /// <summary>
        /// The collection that stores the conditions.
        /// </summary>
        private readonly IStoreScheduleConditions m_Conditions;

        /// <summary>
        /// The collection that stores the schedules.
        /// </summary>
        private readonly IStoreSchedules m_Schedules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleInformationCommands"/> class
        /// </summary>
        /// <param name="actions">The collection that contains the actions that can be scheduled.</param>
        /// <param name="conditions">The collection that contains the conditions that can be used in a schedule.</param>
        /// <param name="schedules">The collection of known editable schedules.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="conditions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedules"/> is <see langword="null" />.
        /// </exception>
        public ScheduleInformationCommands(
            IStoreScheduleActions actions,
            IStoreScheduleConditions conditions,
            IStoreSchedules schedules)
        {
            {
                Lokad.Enforce.Argument(() => actions);
                Lokad.Enforce.Argument(() => conditions);
                Lokad.Enforce.Argument(() => schedules);
            }

            m_Actions = actions;
            m_Conditions = conditions;
            m_Schedules = schedules;
        }

        /// <summary>
        /// Returns a collection containing all the known actions that can be used in
        /// a schedule.
        /// </summary>
        /// <returns>A task that returns the collection containing all the known schedule actions.</returns>
        public Task<IEnumerable<ScheduleActionInformation>> AvailableActions()
        {
            var result = Task<IEnumerable<ScheduleActionInformation>>.Factory.StartNew(
                () =>
                {
                    var list = new List<ScheduleActionInformation>();
                    foreach (var id in m_Actions)
                    {
                        list.Add(m_Actions.Information(id));
                    }

                    return list;
                });

            return result;
        }

        /// <summary>
        /// Returns a collection containing all the known conditions that can be used in a schedule.
        /// </summary>
        /// <returns>A task that returns the collection containing all the known schedule conditions.</returns>
        public Task<IEnumerable<ScheduleConditionInformation>> AvailableConditions()
        {
            var result = Task<IEnumerable<ScheduleConditionInformation>>.Factory.StartNew(
                () =>
                {
                    var list = new List<ScheduleConditionInformation>();
                    foreach (var id in m_Conditions)
                    {
                        list.Add(m_Conditions.Information(id));
                    }

                    return list;
                });

            return result;
        }

        /// <summary>
        /// Returns a collection containing all the known schedules.
        /// </summary>
        /// <returns>A task that returns the collection containing all the known schedules.</returns>
        public Task<IEnumerable<ScheduleInformation>> AvailableSchedules()
        {
            var result = Task<IEnumerable<ScheduleInformation>>.Factory.StartNew(
                () =>
                { 
                    var list = new List<ScheduleInformation>();
                    foreach (var id in m_Schedules)
                    {
                        list.Add(m_Schedules.Information(id));
                    }

                    return list;
                });

            return result;
        }

        /// <summary>
        /// Returns the schedule description for the schedule with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule for which the information is requested.</param>
        /// <returns>A task that returns the desired schedule description.</returns>
        public Task<ScheduleInformation> ScheduleInformation(ScheduleId id)
        {
            var result = Task<ScheduleInformation>.Factory.StartNew(
                () =>
                {
                    return m_Schedules.Information(id);
                });

            return result;
        }

        /// <summary>
        /// Returns the schedule with the given ID.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <returns>A task that returns the desired schedule.</returns>
        public Task<IEditableSchedule> Schedule(ScheduleId id)
        {
            var result = Task<IEditableSchedule>.Factory.StartNew(
                () =>
                {
                    return m_Schedules.Schedule(id);
                });

            return result;
        }
    }
}
