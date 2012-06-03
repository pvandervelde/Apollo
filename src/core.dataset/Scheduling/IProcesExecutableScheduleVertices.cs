//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that process actions provided by a <see cref="IExecutableScheduleVertex"/>.
    /// </summary>
    internal interface IProcesExecutableScheduleVertices
    {
        /// <summary>
        /// Gets the type of the <see cref="IExecutableScheduleVertex"/> that will be processed by
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
        /// <param name="executionState">A value indicating if the execution of the schedule should continue.</param>
        void Process(
            IExecutableScheduleVertex vertex, 
            ScheduleExecutionInfo executionInfo, 
            out ScheduleExecutionState executionState);
    }
}
