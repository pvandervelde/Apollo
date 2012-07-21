//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset.Scheduling.Processors
{
    /// <summary>
    /// Defines the actions taken when a <see cref="ExecutableMarkHistoryVertex"/> is encountered while processing
    /// an executable schedule.
    /// </summary>
    internal sealed class MarkHistoryVertexProcessor : IProcesExecutableScheduleVertices
    {
        /// <summary>
        /// The object that tracks history.
        /// </summary>
        private readonly ITimeline m_Timeline;

        /// <summary>
        /// The function used to store any generated markers.
        /// </summary>
        private readonly Action<TimeMarker> m_OnMarkerStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkHistoryVertexProcessor"/> class.
        /// </summary>
        /// <param name="timeline">The timeline that stores the history marks.</param>
        /// <param name="onMarkerStorage">The function used to store the generated time markers.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="timeline"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="onMarkerStorage"/> is <see langword="null" />.
        /// </exception>
        public MarkHistoryVertexProcessor(ITimeline timeline, Action<TimeMarker> onMarkerStorage)
        {
            {
                Lokad.Enforce.Argument(() => timeline);
                Lokad.Enforce.Argument(() => onMarkerStorage);
            }

            m_Timeline = timeline;
            m_OnMarkerStorage = onMarkerStorage;
        }

        /// <summary>
        /// Gets the type of the <see cref="IExecutableScheduleVertex"/> that will be processed by
        /// this processor.
        /// </summary>
        public Type VertexTypeToProcess
        {
            get
            {
                return typeof(ExecutableMarkHistoryVertex);
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
            var markVertex = vertex as ExecutableMarkHistoryVertex;
            if (markVertex == null)
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

            var marker = m_Timeline.Mark();
            m_OnMarkerStorage(marker);
            return ScheduleExecutionState.Executing;
        }
    }
}
