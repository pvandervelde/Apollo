//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Projects;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    /// <summary>
    /// Defines the facade for a schedule.
    /// </summary>
    public sealed class ScheduleFacade
    {
        /// <summary>
        /// The scene that owns the current schedule.
        /// </summary>
        private readonly SceneFacade m_Owner;

        /// <summary>
        /// The object that provides the information about the schedule.
        /// </summary>
        private readonly IHoldSceneScheduleData m_Schedule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleFacade"/> class.
        /// </summary>
        /// <param name="owner">The scene that owns the current schedule.</param>
        /// <param name="schedule">The object that provides the information about the schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        internal ScheduleFacade(SceneFacade owner, IHoldSceneScheduleData schedule)
        {
            {
                Lokad.Enforce.Argument(() => owner);
                Lokad.Enforce.Argument(() => schedule);
            }

            m_Owner = owner;
            m_Schedule = schedule;
        }

        /// <summary>
        /// Gets the ID of the schedule.
        /// </summary>
        public ScheduleId Id
        {
            get
            {
                return m_Schedule.Id;
            }
        }

        /// <summary>
        /// Gets the scene that owns the current schedule.
        /// </summary>
        public SceneFacade Owner
        {
            get
            {
                return m_Owner;
            }
        }

        /// <summary>
        /// Gets or sets the name of the schedule.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Schedule.Name;
            }

            set
            {
                m_Schedule.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the summary of the schedule.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Schedule.Summary;
            }

            set
            {
                m_Schedule.Summary = value;
            }
        }
    }
}
