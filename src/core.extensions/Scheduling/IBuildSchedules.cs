//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that build editable schedules.
    /// </summary>
    public interface IBuildSchedules
    {
        /// <summary>
        /// Builds the schedule from the stored information.
        /// </summary>
        /// <returns>
        /// A new schedule with all the information that was stored.
        /// </returns>
        IEditableSchedule Build();
    }
}
