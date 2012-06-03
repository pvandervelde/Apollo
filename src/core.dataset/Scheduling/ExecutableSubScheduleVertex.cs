//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines an <see cref="IExecutableScheduleVertex"/> which has the ID of a sub-schedule that should be executed.
    /// </summary>
    internal sealed class ExecutableSubScheduleVertex : IExecutableScheduleVertex
    {
        /// <summary>
        /// The ID of the sub-schedule.
        /// </summary>
        private readonly ScheduleId m_SubSchedule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableSubScheduleVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        /// <param name="subSchedule">The ID of the sub-schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subSchedule"/> is <see langword="null" />.
        /// </exception>
        public ExecutableSubScheduleVertex(int index, ScheduleId subSchedule)
        {
            {
                Lokad.Enforce.Argument(() => subSchedule);
            }

            Index = index;
            m_SubSchedule = subSchedule;
        }

        /// <summary>
        /// Gets the index of the vertex in the graph.
        /// </summary>
        public int Index
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID of the sub-schedule.
        /// </summary>
        public ScheduleId SubSchedule
        {
            get
            {
                return m_SubSchedule;
            }
        }
    }
}
