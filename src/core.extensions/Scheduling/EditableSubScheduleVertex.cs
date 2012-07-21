//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// A vertex for the <see cref="IEditableSchedule"/> which links to a sub-schedule.
    /// </summary>
    /// <remarks>
    /// All editable schedule vertices should be immutable because a schedule is copied
    /// by reusing the vertices.
    /// </remarks>
    [Serializable]
    public sealed class EditableSubScheduleVertex : IEditableScheduleVertex
    {
        /// <summary>
        /// The ID of the schedule that must be executed.
        /// </summary>
        private readonly ScheduleId m_SubSchedule;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSubScheduleVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        /// <param name="subSchedule">The ID of the schedule that should be executed.</param>
        internal EditableSubScheduleVertex(int index, ScheduleId subSchedule)
        {
            {
                Debug.Assert(subSchedule != null, "The ID of the sub-schedule cannot be a null reference.");
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
        /// Gets the ID of the schedule that should be executed.
        /// </summary>
        public ScheduleId ScheduleToExecute
        {
            get
            {
                return m_SubSchedule;
            }
        }
    }
}
