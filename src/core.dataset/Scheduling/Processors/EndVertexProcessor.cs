//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    /// <summary>
    /// Defines the actions taken when a <see cref="ExecutableEndVertex"/> is encountered while processing
    /// an executable schedule.
    /// </summary>
    internal sealed class EndVertexProcessor : IProcesExecutableScheduleVertices
    {
        /// <summary>
        /// Gets the type of the <see cref="IScheduleVertex"/> that will be processed by
        /// this processor.
        /// </summary>
        public Type VertexTypeToProcess
        {
            get
            {
                return typeof(ExecutableEndVertex);
            }
        }

        /// <summary>
        /// Processes action provided by the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="executionInfo">The object that stores the information about the execution of the schedule.</param>
        /// <returns>A value indicating if the execution of the schedule should continue.</returns>
        public ScheduleExecutionState Process(IScheduleVertex vertex, ScheduleExecutionInfo executionInfo)
        {
            var endVertex = vertex as ExecutableEndVertex;
            if (endVertex == null)
            {
                Debug.Assert(false, "The vertex is of the incorrect type.");
                return ScheduleExecutionState.IncorrectProcessorForVertex;
            }

            // We don't check for pause or cancel here, we're done anyway
            return ScheduleExecutionState.Completed;
        }
    }
}
