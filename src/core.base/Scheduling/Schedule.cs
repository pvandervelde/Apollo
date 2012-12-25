﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base.Properties;
using Apollo.Core.Extensions.Scheduling;
using QuickGraph;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Stores a schedule in editable format.
    /// </summary>
    [Serializable]
    internal sealed class Schedule : ISchedule
    {
        /// <summary>
        /// The graph of schedule nodes.
        /// </summary>
        private readonly BidirectionalGraph<IScheduleVertex, ScheduleEdge> m_Graph;

        /// <summary>
        /// The vertex that is forms the start of the schedule.
        /// </summary>
        /// <remarks>
        /// If the current schedule is inserted in another schedule as a schedule-block then this end vertex will
        /// be connected to the insert node.
        /// </remarks>
        private readonly IScheduleVertex m_Start;

        /// <summary>
        /// The vertex that forms the end of the schedule.
        /// </summary>
        /// <remarks>
        /// If the current schedule is inserted in another schedule as a schedule-block then this end vertex will
        /// be connected to the insert node.
        /// </remarks>
        private readonly IScheduleVertex m_End;

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="graph">The graph that describes the current schedule.</param>
        /// <param name="start">The start node for the schedule.</param>
        /// <param name="end">The end node for the schedule.</param>
        public Schedule(
            BidirectionalGraph<IScheduleVertex, ScheduleEdge> graph,
            IScheduleVertex start,
            IScheduleVertex end)
        {
            {
                Debug.Assert(graph != null, "The graph should not be a null reference.");

                Debug.Assert(start != null, "The start vertex should not be a null reference.");
                Debug.Assert(start is StartVertex, "The start vertex should be an editable start vertex.");
                Debug.Assert(graph.ContainsVertex(start), "The start vertex should be part of the graph.");

                Debug.Assert(end != null, "The end vertex should not be a null reference.");
                Debug.Assert(end is EndVertex, "The end vertex should be an editable end vertex.");
                Debug.Assert(graph.ContainsVertex(end), "The end vertex should be part of the graph.");
            }

            m_Start = start;
            m_End = end;

            m_Graph = graph;
        }

        /// <summary>
        /// Gets the start vertex for the schedule.
        /// </summary>
        public IScheduleVertex Start
        {
            get
            {
                return m_Start;
            }
        }

        /// <summary>
        /// Gets the end vertex for the schedule.
        /// </summary>
        public IScheduleVertex End
        {
            get
            {
                return m_End;
            }
        }

        /// <summary>
        /// Gets a collection that contains all the known vertices for the schedule.
        /// </summary>
        public IEnumerable<IScheduleVertex> Vertices
        {
            get
            {
                return m_Graph.Vertices;
            }
        }

        /// <summary>
        /// Traverses the schedule and applies an action to each vertex visited.
        /// </summary>
        /// <param name="start">The vertex where the traverse should be started.</param>
        /// <param name="vertexAction">
        /// The action taken for each vertex that is encountered. The action is provided with the current vertex and
        /// a collection of all outbound, if <paramref name="traverseViaOutBoundVertices"/> is <see langword="true" />,
        /// or inbound vertices and the ID of the traversing condition. The function should return <see langword="false" />
        /// to terminate the traverse.
        /// </param>
        /// <param name="traverseViaOutBoundVertices">
        /// A flag indicating if the schedule should be traversed via the outbound edges of the vertices, or the inbound ones.
        /// </param>
        public void TraverseAllScheduleVertices(
            IScheduleVertex start, 
            Func<IScheduleVertex, IEnumerable<Tuple<ScheduleElementId, IScheduleVertex>>, bool> vertexAction, 
            bool traverseViaOutBoundVertices = true)
        {
            {
                Lokad.Enforce.Argument(() => start);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Graph.ContainsVertex(start), 
                    Resources.Exceptions_Messages_UnknownScheduleVertex);

                Lokad.Enforce.Argument(() => vertexAction);
            }

            var nodeCounter = new List<IScheduleVertex>();

            var uncheckedVertices = new Queue<IScheduleVertex>();
            uncheckedVertices.Enqueue(start);
            while (uncheckedVertices.Count > 0)
            {
                var source = uncheckedVertices.Dequeue();
                if (nodeCounter.Contains(source))
                {
                    continue;
                }

                nodeCounter.Add(source);

                var outEdges = traverseViaOutBoundVertices ? m_Graph.OutEdges(source) : m_Graph.InEdges(source);
                var traverseMap = from edge in outEdges
                                  select new Tuple<ScheduleElementId, IScheduleVertex>(
                                      edge.TraversingCondition,
                                      traverseViaOutBoundVertices ? edge.Target : edge.Source);

                var result = vertexAction(source, traverseMap);
                if (!result)
                {
                    return;
                }

                foreach (var outEdge in outEdges)
                {
                    uncheckedVertices.Enqueue(traverseViaOutBoundVertices ? outEdge.Target : outEdge.Source);
                }
            }
        }

        /// <summary>
        /// Traverses the schedule and applies an action to each vertex visited.
        /// </summary>
        /// <param name="start">The vertex where the traverse should be started.</param>
        /// <param name="vertexAction">
        /// The action taken for each vertex that is encountered. The function is provided with the current vertex and
        /// a collection of all outbound, if <paramref name="traverseViaOutboundVertices"/> is <see langword="true" />,
        /// or inbound vertices and the ID of the traversing condition. The function should return <see langword="false" />
        /// to terminate the traverse.
        /// </param>
        /// <param name="directionAction">
        /// The function that determines in which direction the traverse should proceed. The function is provided with
        /// a collection of all outbound, if <paramref name="traverseViaOutboundVertices"/> is <see langword="true" />,
        /// or inbound vertices and the ID of the traversing condition. The function should return the next vertex
        /// that should be traversed, or <see langword="null" /> if the traverse should be terminated.
        /// </param>
        /// <param name="traverseViaOutboundVertices">
        /// A flag indicating if the schedule should be traversed via the outbound edges of the vertices, or the inbound ones.
        /// </param>
        public void TraverseSchedule(
            IScheduleVertex start,
            Func<IScheduleVertex, bool> vertexAction,
            Func<IEnumerable<Tuple<ScheduleElementId, IScheduleVertex>>, IScheduleVertex> directionAction,
            bool traverseViaOutboundVertices = true)
        {
            {
                Lokad.Enforce.Argument(() => start);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Graph.ContainsVertex(start),
                    Resources.Exceptions_Messages_UnknownScheduleVertex);

                Lokad.Enforce.Argument(() => vertexAction);
                Lokad.Enforce.Argument(() => directionAction);
            }

            var source = start;
            while (source != null)
            {
                var result = vertexAction(source);
                if (!result)
                {
                    return;
                }

                var outEdges = traverseViaOutboundVertices ? m_Graph.OutEdges(source) : m_Graph.InEdges(source);
                var traverseMap = from edge in outEdges
                                  select new Tuple<ScheduleElementId, IScheduleVertex>(
                                      edge.TraversingCondition,
                                      traverseViaOutboundVertices ? edge.Target : edge.Source);

                source = directionAction(traverseMap);
            }
        }

        /// <summary>
        /// Returns the number of vertices which connect to the current vertex.
        /// </summary>
        /// <param name="origin">The vertex for which the number of inbound connections should be returned.</param>
        /// <returns>The number of inbound connections for the current vertex.</returns>
        public int NumberOfInboundConnections(IScheduleVertex origin)
        {
            return m_Graph.InDegree(origin);
        }

        /// <summary>
        /// Returns the number of vertices to which the current vertex connects.
        /// </summary>
        /// <param name="origin">The vertex for which the number of outbound connections should be returned.</param>
        /// <returns>The number of outbound connections for the current vertex.</returns>
        public int NumberOfOutboundConnections(IScheduleVertex origin)
        {
            return m_Graph.OutDegree(origin);
        }

        /// <summary>
        /// Returns a collection containing all the insert points for the schedule.
        /// </summary>
        /// <returns>The collection that contains all the insert points for the schedule.</returns>
        public IEnumerable<InsertVertex> InsertPoints()
        {
            var result = from vertex in m_Graph.Vertices
                         where vertex is InsertVertex
                         select vertex as InsertVertex;

            return result;
        }
    }
}