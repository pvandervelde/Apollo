//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Apollo.Core.Extensions.Scheduling;
using QuickGraph;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Stores a collection of schedule actions and the relation between these actions.
    /// </summary>
    internal sealed class ExecutableSchedule
    {
        /// <summary>
        /// The graph of schedule nodes. Traversable from parent to child only.
        /// </summary>
        private readonly AdjacencyGraph<IExecutableScheduleVertex, ExecutableScheduleEdge> m_Graph;

        /// <summary>
        /// The ID of the current schedule.
        /// </summary>
        private readonly ScheduleId m_Id;

        /// <summary>
        /// The vertex that is forms the start of the schedule.
        /// </summary>
        /// <remarks>
        /// If the current schedule is inserted in another schedule as a schedule-block then this end vertex will
        /// be connected to the insert node.
        /// </remarks>
        private readonly ExecutableStartVertex m_Start;

        /// <summary>
        /// The vertex that forms the end of the schedule.
        /// </summary>
        /// <remarks>
        /// If the current schedule is inserted in another schedule as a schedule-block then this end vertex will
        /// be connected to the insert node.
        /// </remarks>
        private readonly ExecutableEndVertex m_End;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableSchedule"/> class.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <param name="graph">The graph that describes the current schedule.</param>
        /// <param name="start">The start node for the schedule.</param>
        /// <param name="end">The end node for the schedule.</param>
        public ExecutableSchedule(
            ScheduleId id,
            AdjacencyGraph<IExecutableScheduleVertex, ExecutableScheduleEdge> graph,
            ExecutableStartVertex start,
            ExecutableEndVertex end)
        {
            {
                Debug.Assert(id != null, "The ID should not be a null reference.");
                Debug.Assert(graph != null, "The graph should not be a null reference.");

                Debug.Assert(start != null, "The start vertex should not be a null reference.");
                Debug.Assert(graph.ContainsVertex(start), "The start vertex should be part of the graph.");

                Debug.Assert(end != null, "The end vertex should not be a null reference.");
                Debug.Assert(graph.ContainsVertex(end), "The end vertex should be part of the graph.");
            }

            m_Id = id;
            m_Start = start;
            m_End = end;

            m_Graph = graph;
        }

        /// <summary>
        /// Gets the ID of the current schedule.
        /// </summary>
        public ScheduleId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the start vertex for the schedule.
        /// </summary>
        public ExecutableStartVertex Start
        {
            get
            {
                return m_Start;
            }
        }

        /// <summary>
        /// Gets the end vertex for the schedule.
        /// </summary>
        public ExecutableEndVertex End
        {
            get
            {
                return m_End;
            }
        }

        /// <summary>
        /// Gets the graph for the current schedule.
        /// </summary>
        public IVertexListGraph<IExecutableScheduleVertex, ExecutableScheduleEdge> Graph
        {
            get
            {
                return m_Graph;
            }
        }
    }
}
