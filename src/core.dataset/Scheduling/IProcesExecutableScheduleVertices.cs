//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that process actions provided by a <see cref="IScheduleVertex"/>.
    /// </summary>
    internal interface IProcesExecutableScheduleVertices
    {
        /// <summary>
        /// Gets the type of the <see cref="IScheduleVertex"/> that will be processed by
        /// this processor.
        /// </summary>
        Type VertexTypeToProcess
        {
            get;
        }

        /// <summary>
        /// Processes action provided by the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="executionInfo">The object that stores the information about the execution of the schedule.</param>
        /// <returns>A value indicating if the execution of the schedule should continue.</returns>
        ScheduleExecutionState Process(IScheduleVertex vertex, ScheduleExecutionInfo executionInfo);
    }
}
