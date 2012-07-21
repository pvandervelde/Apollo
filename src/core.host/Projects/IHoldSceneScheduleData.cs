//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that store information about a specific schedule.
    /// </summary>
    internal interface IHoldSceneScheduleData
    {
        /// <summary>
        /// Gets the ID of the schedule.
        /// </summary>
        ScheduleId Id
        {
            get;
        }

        /// <summary>
        /// Gets or sets the name of the schedule.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the summary of the schedule.
        /// </summary>
        string Summary
        {
            get;
            set;
        }
    }
}
