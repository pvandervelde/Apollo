//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    /// <summary>
    /// Defines the actions taken when a <see cref="ExecutableStartVertex"/> is encountered while processing
    /// an executable schedule.
    /// </summary>
    internal sealed class StartVertexProcessor : IProcesExecutableScheduleVertices
    {
        /// <summary>
        /// Gets the type of the <see cref="IExecutableScheduleVertex"/> that will be processed by
        /// this processor.
        /// </summary>
        public Type VertexTypeToProcess
        {
            get
            {
                return typeof(ExecutableStartVertex);
            }
        }

        /// <summary>
        /// Processes action provided by the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="executionInfo">The object that stores the information about the execution of the schedule.</param>
        /// <returns>A value indicating if the execution of the schedule should continue.</returns>
        public ScheduleExecutionState Process(IExecutableScheduleVertex vertex, ScheduleExecutionInfo executionInfo)
        {
            var startVertex = vertex as ExecutableStartVertex;
            if (startVertex == null)
            {
                Debug.Assert(false, "The vertex is of the incorrect type.");
                return ScheduleExecutionState.IncorrectProcessorForVertex;
            }

            if (executionInfo.Cancellation.IsCancellationRequested)
            {
                return ScheduleExecutionState.Canceled;
            }

            return ScheduleExecutionState.Executing;
        }
    }
}
