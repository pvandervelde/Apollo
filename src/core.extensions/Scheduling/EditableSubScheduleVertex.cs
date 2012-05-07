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
        /// A flag that indicates if the executor should wait for the
        /// sub-schedule to finish or not.
        /// </summary>
        private readonly bool m_ShouldWaitForFinish;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSubScheduleVertex"/> class.
        /// </summary>
        /// <param name="subSchedule">The ID of the schedule that should be executed.</param>
        /// <param name="waitForFinish">Indicates if the executor should wait for the sub-schedule to finish executing or not.</param>
        internal EditableSubScheduleVertex(ScheduleId subSchedule, bool waitForFinish)
        {
            {
                Debug.Assert(subSchedule != null, "The ID of the sub-schedule cannot be a null reference.");
            }

            m_SubSchedule = subSchedule;
            m_ShouldWaitForFinish = waitForFinish;
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

        /// <summary>
        /// Gets a value indicating whether the executor should wait for the sub-schedule
        /// to finish executing or not.
        /// </summary>
        public bool ShouldWaitForFinish
        {
            get
            {
                return m_ShouldWaitForFinish;
            }
        }
    }
}
