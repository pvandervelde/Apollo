//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for a data structure that stores a schedule in editable form.
    /// </summary>
    public interface IEditableSchedule
    {
        /// <summary>
        /// Gets the ID of the current schedule.
        /// </summary>
        ScheduleId Id
        {
            get;
        }

        /// <summary>
        /// Gets the start vertex for the schedule.
        /// </summary>
        EditableStartVertex Start
        {
            get;
        }

        /// <summary>
        /// Gets the end vertex for the schedule.
        /// </summary>
        EditableEndVertex End
        {
            get;
        }
    }
}
