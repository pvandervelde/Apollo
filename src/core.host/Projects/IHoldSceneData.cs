//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that store information about the dataset 'scene', i.e. the
    /// collection of regions, boundaries, equations, schedules, components and other objects that define what data
    /// a dataset contains and what actions it is capable of taking.
    /// </summary>
    internal interface IHoldSceneData
    {
        /// <summary>
        /// Gets the schedule information which is registered for the current scene.
        /// </summary>
        IHoldSchedulingData Schedules
        {
            get;
        }
    }
}
