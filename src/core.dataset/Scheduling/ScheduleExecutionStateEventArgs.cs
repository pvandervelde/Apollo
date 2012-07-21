//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// An <see cref="EventArgs"/> class which stores information about the completion state of a schedule execution.
    /// </summary>
    internal sealed class ScheduleExecutionStateEventArgs : EventArgs
    {
        /// <summary>
        /// The final execution state.
        /// </summary>
        private ScheduleExecutionState m_State;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleExecutionStateEventArgs"/> class.
        /// </summary>
        /// <param name="state">The final execution state.</param>
        public ScheduleExecutionStateEventArgs(ScheduleExecutionState state)
        {
            m_State = state;
        }

        /// <summary>
        /// Gets the final execution state.
        /// </summary>
        public ScheduleExecutionState State
        {
            get
            {
                return m_State;
            }
        }
    }
}
