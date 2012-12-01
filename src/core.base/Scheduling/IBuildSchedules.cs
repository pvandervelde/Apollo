//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Scheduling
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
        ISchedule Build();
    }
}
