//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that store information about a specific schedule action.
    /// </summary>
    internal interface IHoldSceneScheduleActionData
    {
        /// <summary>
        /// Gets the ID of the action.
        /// </summary>
        ScheduleElementId Id
        {
            get;
        }
    }
}
