//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that stores a <see cref="ScheduleId"/>.
    /// </summary>
    [Serializable]
    public sealed class ScheduleEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the schedule that is referred to in the event.
        /// </summary>
        private readonly ScheduleId m_Schedule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleEventArgs"/> class.
        /// </summary>
        /// <param name="schedule">The ID of the schedule that is referred to in the event.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        public ScheduleEventArgs(ScheduleId schedule)
        {
            {
                Lokad.Enforce.Argument(() => schedule);
            }

            m_Schedule = schedule;
        }

        /// <summary>
        /// Gets the ID of the schedule that is referred to in the event.
        /// </summary>
        public ScheduleId Schedule
        {
            get
            {
                return m_Schedule;
            }
        }
    }
}
