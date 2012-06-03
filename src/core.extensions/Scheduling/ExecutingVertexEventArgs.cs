//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that provides information about the vertex that is
    /// currently being executed in a schedule.
    /// </summary>
    [Serializable]
    public sealed class ExecutingVertexEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the schedule that is being executed.
        /// </summary>
        private readonly ScheduleId m_Schedule;

        /// <summary>
        /// The index of the vertex that is currently being executed.
        /// </summary>
        private readonly int m_Vertex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutingVertexEventArgs"/> class.
        /// </summary>
        /// <param name="schedule">The ID of the schedule that is currently being executed.</param>
        /// <param name="vertex">The index of the vertex that is currently being executed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        public ExecutingVertexEventArgs(ScheduleId schedule, int vertex)
        {
            {
                Lokad.Enforce.Argument(() => schedule);
            }

            m_Schedule = schedule;
            m_Vertex = vertex;
        }

        /// <summary>
        /// Gets the ID of the schedule that is currently being executed.
        /// </summary>
        public ScheduleId Schedule
        {
            get
            {
                return m_Schedule;
            }
        }

        /// <summary>
        /// Gets the index of the vertex that is currently being executed.
        /// </summary>
        public int Vertex
        {
            get
            {
                return m_Vertex;
            }
        }
    }
}
