//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Host.Projects
{
    internal interface IHoldSchedulingData
    {
        /// <summary>
        /// Gets the collection of actions.
        /// </summary>
        IEnumerable<IHoldSceneScheduleActionData> Actions
        {
            get;
        }

        /// <summary>
        /// Gets the collection of conditions.
        /// </summary>
        IEnumerable<IHoldSceneScheduleConditionData> Conditions
        {
            get;
        }
        
        /// <summary>
        /// Gets the collection of schedules.
        /// </summary>
        IEnumerable<IHoldSceneScheduleData> Schedules
        {
            get;
        }
    }
}
