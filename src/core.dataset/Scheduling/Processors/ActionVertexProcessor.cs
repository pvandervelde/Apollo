//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    /// <summary>
    /// Defines the actions taken when a <see cref="ExecutableActionVertex"/> is encountered while processing
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
        /// Gets the type of the <see cref="IExecutableScheduleVertex"/> that will be processed by
        /// this processor.
        /// </summary>
        public Type VertexTypeToProcess
        {
            get
            {
                return typeof(ExecutableActionVertex);
            }
        }

        /// <summary>
        /// Processes action provided by the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="executionInfo">The object that stores the information about the execution of the schedule.</param>
        /// <param name="executionState">A value indicating if the execution of the schedule should continue.</param>
        public void Process(
            IExecutableScheduleVertex vertex,
            ScheduleExecutionInfo executionInfo,
            out ScheduleExecutionState executionState)
        {
            var actionVertex = vertex as ExecutableActionVertex;
            if (actionVertex == null)
            {
                Debug.Assert(false, "The vertex is of the incorrect type.");
                executionState = ScheduleExecutionState.IncorrectProcessorForVertex;
                return;
            }

            if (executionInfo.Cancellation.IsCancellationRequested)
            {
                executionState = ScheduleExecutionState.Cancelled;
                return;
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

            var action = m_Storage[id];
            action.Execute(executionInfo.Cancellation);
            executionState = ScheduleExecutionState.Executing;
        }
    }
}
