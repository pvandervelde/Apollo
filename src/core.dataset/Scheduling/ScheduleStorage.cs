//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Dataset.Properties;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Stores schedules by ID.
    /// </summary>
    internal sealed class ScheduleStorage : IStoreSchedules
    {
        /// <summary>
        /// Maps the schedule to the information describing the schedule.
        /// </summary>
        private sealed class ScheduleMap
        {
            /// <summary>
            /// The schedule.
            /// </summary>
            private readonly IEditableSchedule m_Schedule;

            /// <summary>
            /// The information describing the schedule.
            /// </summary>
            private readonly ScheduleInformation m_Info;

            /// <summary>
            /// Initializes a new instance of the <see cref="ScheduleMap"/> class.
            /// </summary>
            /// <param name="information">The information describing the schedule.</param>
            /// <param name="schedule">The schedule.</param>
            public ScheduleMap(ScheduleInformation information, IEditableSchedule schedule)
            {
                {
                    Debug.Assert(information != null, "The schedule information should not be a null reference.");
                    Debug.Assert(schedule != null, "The schedule should not be a null reference.");
                }

                m_Info = information;
                m_Schedule = schedule;
            }

            /// <summary>
            /// Gets the schedule information.
            /// </summary>
            public ScheduleInformation Information
            {
                get
                {
                    return m_Info;
                }
            }

            /// <summary>
            /// Gets the schedule.
            /// </summary>
            public IEditableSchedule Schedule
            {
                get
                {
                    return m_Schedule;
                }
            }
        }

        /// <summary>
        /// The collection of schedules.
        /// </summary>
        private readonly Dictionary<ScheduleId, ScheduleMap> m_Schedules
            = new Dictionary<ScheduleId, ScheduleMap>();

        /// <summary>
        /// Adds the <see cref="IEditableSchedule"/> object with the variables it affects and the dependencies for that schedule.
        /// </summary>
        /// <param name="schedule">The schedule that should be stored.</param>
        /// <param name="name">The name of the schedule that is being described by this information object.</param>
        /// <param name="summary">The summary of the schedule that is being described by this information object.</param>
        /// <param name="description">The description of the schedule that is being described by this information object.</param>
        /// <param name="produces">The variables that are affected by the schedule.</param>
        /// <param name="dependsOn">The variables for which data should be available in order to execute the schedule.</param>
        /// <returns>An object identifying and describing the schedule.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="produces"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dependsOn"/> is <see langword="null" />.
        /// </exception>
        public ScheduleInformation Add(
            IEditableSchedule schedule,
            string name,
            string summary,
            string description,
            IEnumerable<IScheduleVariable> produces,
            IEnumerable<IScheduleDependency> dependsOn)
        {
            {
                Lokad.Enforce.Argument(() => schedule);
                Lokad.Enforce.Argument(() => produces);
                Lokad.Enforce.Argument(() => dependsOn);
            }

            var id = new ScheduleId();
            var info = new ScheduleInformation(id, name, summary, description, produces, dependsOn);
            m_Schedules.Add(id, new ScheduleMap(info, schedule));

            return info;
        }

        /// <summary>
        /// Replaces the schedule current stored against the given ID with a new one.
        /// </summary>
        /// <param name="scheduleToReplace">The ID of the schedule that should be replaced.</param>
        /// <param name="newSchedule">The new schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduleToReplace"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="newSchedule"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleException">
        ///     Thrown if <paramref name="scheduleToReplace"/> is not linked to a known schedule.
        /// </exception>
        public void Update(
            ScheduleId scheduleToReplace,
            IEditableSchedule newSchedule)
        {
            {
                Lokad.Enforce.Argument(() => scheduleToReplace);
                Lokad.Enforce.Argument(() => newSchedule);
                Lokad.Enforce.With<UnknownScheduleException>(
                    m_Schedules.ContainsKey(scheduleToReplace),
                    Resources.Exceptions_Messages_UnknownSchedule);
            }

            var oldInfo = m_Schedules[scheduleToReplace].Information;
            var info = new ScheduleInformation(
                scheduleToReplace,
                oldInfo.Name,
                oldInfo.Summary,
                oldInfo.Description,
                oldInfo.Produces(),
                oldInfo.DependsOn());
            m_Schedules[scheduleToReplace] = new ScheduleMap(info, newSchedule);
        }

        /// <summary>
        /// Removes the schedule with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the schedule that should be removed.</param>
        public void Remove(ScheduleId id)
        {
            if ((id != null) && m_Schedules.ContainsKey(id))
            {
                m_Schedules.Remove(id);
            }
        }

        /// <summary>
        /// Returns a value indicating if there is an schedule with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <returns>
        /// <see langword="true" /> if there is an schedule with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Contains(ScheduleId id)
        {
            return (id != null) && m_Schedules.ContainsKey(id);
        }

        /// <summary>
        /// Returns the schedule that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the schedule.</param>
        /// <returns>The schedule.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleException">
        ///     Thrown if <paramref name="id"/> is not linked to a known schedule.
        /// </exception>
        public IEditableSchedule Schedule(ScheduleId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownScheduleException>(
                    m_Schedules.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownSchedule);
            }

            return m_Schedules[id].Schedule;
        }

        /// <summary>
        /// Returns the schedule information for the schedule that is mapped to the given ID.
        /// </summary>
        /// <param name="id">The ID for the schedule.</param>
        /// <returns>The schedule information.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleActionException">
        ///     Thrown if <paramref name="id"/> is not linked to a known schedule.
        /// </exception>
        public ScheduleInformation Information(ScheduleId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownScheduleException>(
                    m_Schedules.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownSchedule);
            }

            return m_Schedules[id].Information;
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<ScheduleId> GetEnumerator()
        {
            foreach (var key in m_Schedules.Keys)
            {
                yield return key;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through 
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
