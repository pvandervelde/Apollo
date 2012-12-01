//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    /// <summary>
    /// Defines the actions taken when a <see cref="ExecutingActionVertex"/> is encountered while processing
    /// an executable schedule.
    /// </summary>
    internal sealed class ActionVertexProcessor : IProcesExecutableScheduleVertices
    {
        /// <summary>
        /// The object that stores the actions.
        /// </summary>
        private readonly IStoreScheduleActions m_Storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionVertexProcessor"/> class.
        /// </summary>
        /// <param name="storage">The object that stores the actions.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storage"/> is <see langword="null" />.
        /// </exception>
        public ActionVertexProcessor(IStoreScheduleActions storage)
        {
            {
                Lokad.Enforce.Argument(() => storage);
            }

            m_Storage = storage;
        }

        /// <summary>
        /// Gets the type of the <see cref="IScheduleVertex"/> that will be processed by
        /// this processor.
        /// </summary>
        public Type VertexTypeToProcess
        {
            get
            {
                return typeof(ExecutingActionVertex);
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
            var actionVertex = vertex as ExecutingActionVertex;
            if (actionVertex == null)
            {
                Debug.Assert(false, "The vertex is of the incorrect type.");
                return ScheduleExecutionState.IncorrectProcessorForVertex;
            }

            if (executionInfo.Cancellation.IsCancellationRequested)
            {
                return ScheduleExecutionState.Canceled;
            }

            if (executionInfo.PauseHandler.IsPaused)
            {
                executionInfo.PauseHandler.WaitForUnPause(executionInfo.Cancellation);
            }

            var id = actionVertex.ActionToExecute;
            if (!m_Storage.Contains(id))
            {
                throw new UnknownScheduleActionException();
            }

            var action = m_Storage.Action(id);
            action.Execute(executionInfo.Cancellation);
            return ScheduleExecutionState.Executing;
        }
    }
}
