//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// An <see cref="EventArgs"/> class tath stores a <see cref="ScheduleElementId"/>.
    /// </summary>
    public sealed class ScheduleElementEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the schedule element that is referred to in the event.
        /// </summary>
        private readonly ScheduleElementId m_ScheduleElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleElementEventArgs"/> class.
        /// </summary>
        /// <param name="scheduleElement">The ID of the schedule element that is referred to in the event.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduleElement"/> is <see langword="null" />.
        /// </exception>
        public ScheduleElementEventArgs(ScheduleElementId scheduleElement)
        {
            {
                Lokad.Enforce.Argument(() => scheduleElement);
            }

            m_ScheduleElement = scheduleElement;
        }

        /// <summary>
        /// Gets the ID of the schedule element that is referred to in the event.
        /// </summary>
        public ScheduleElementId ScheduleElement
        {
            get
            {
                return m_ScheduleElement;
            }
        }
    }
}
