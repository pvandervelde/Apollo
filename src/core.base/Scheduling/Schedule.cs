//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Apollo.Core.Base.Properties;
using Apollo.Core.Extensions.Scheduling;
using QuickGraph;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Stores a schedule in editable format.
    /// </summary>
    [Serializable]
    internal sealed class Schedule : ISchedule, ISerializable
    {
        /// <summary>
        /// The tag used during serialization to indicate how many vertices there are in the schedule graph.
        /// </summary>
        private const string SerializationVertexCount = "Schedule.VertexCount";

        /// <summary>
        /// The tag used during serialization to indicate how many edges there are in the schedule graph.
        /// </summary>
        private const string SerializationEdgeCount = "Schedule.EdgeCount";

        /// <summary>
        /// The tag used during serialization to indicate the type of a given vertex.
        /// </summary>
        private const string SerializationVertexType = "Schedule.VertexType_{0}";

        /// <summary>
        /// The tag used during serialization to indicate the data of a given vertex.
        /// </summary>
        private const string SerializationVertex = "Schedule.Vertex_{0}";

        /// <summary>
        /// The tag used during serialization to indicate the data of a given edge.
        /// </summary>
        private const string SerializationEdge = "Schedule.Edge_{0}";

        /// <summary>
        /// The tag used during serialization to indicate the index of the graph start vertex.
        /// </summary>
        private const string SerializationStartVertex = "Schedule.StartVertex";

        /// <summary>
        /// The tag used during serialization to indicate the index of the graph end vertex.
        /// </summary>
        private const string SerializationEndVertex = "Schedule.EndVertex";

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
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="information">The serialization information containing the data for the schedule.</param>
        /// <param name="context">The serialization context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="information"/> is <see langword="null" />.
        /// </exception>
        public Schedule(SerializationInfo information, StreamingContext context)
        {
            {
                Lokad.Enforce.Argument(() => information);
            }

            m_Graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();
            var vertices = new List<IScheduleVertex>();

            var vertexCount = information.GetInt32(SerializationVertexCount);
            for (int i = 0; i < vertexCount; i++)
            {
                var type = (Type)information.GetValue(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SerializationVertexType,
                        i),
                    typeof(Type));

                var vertex = (IScheduleVertex)information.GetValue(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SerializationVertex,
                        i),
                    type);

                vertices.Add(vertex);
                m_Graph.AddVertex(vertex);
            }

            var edgeCount = information.GetInt32(SerializationEdgeCount);
            for (int i = 0; i < edgeCount; i++)
            {
                var tuple = (Tuple<int, int, ScheduleElementId>)information.GetValue(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SerializationEdge,
                        i),
                    typeof(Tuple<int, int, ScheduleElementId>));

                m_Graph.AddEdge(
                    new ScheduleEdge(
                        vertices[tuple.Item1],
                        vertices[tuple.Item2],
                        tuple.Item3));
            }

            var startIndex = information.GetInt32(SerializationStartVertex);
            m_Start = vertices[startIndex];

            var endIndex = information.GetInt32(SerializationEndVertex);
            m_End = vertices[endIndex];
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Split the graph into parts
            var vertices = m_Graph.Vertices.ToList();
            var connections = new List<Tuple<int, int, ScheduleElementId>>();
            foreach (var edge in m_Graph.Edges)
            {
                var start = vertices.FindIndex(v => ReferenceEquals(v, edge.Source));
                Debug.Assert(start > -1, "There should be an index for the start of the edge.");

                var end = vertices.FindIndex(v => ReferenceEquals(v, edge.Target));
                Debug.Assert(end > -1, "There should be an index for the end of the edge.");

                connections.Add(new Tuple<int, int, ScheduleElementId>(start, end, edge.TraversingCondition));
            }

            info.AddValue(SerializationVertexCount, vertices.Count);
            for (int i = 0; i < vertices.Count; i++)
            {
                info.AddValue(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SerializationVertexType,
                        i),
                    vertices[i].GetType());
                info.AddValue(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SerializationVertex,
                        i),
                    vertices[i]);
            }

            info.AddValue(SerializationEdgeCount, connections.Count);
            for (int i = 0; i < connections.Count; i++)
            {
                info.AddValue(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SerializationEdge,
                        i),
                    connections[i]);
            }

            var startIndex = vertices.FindIndex(s => s.Equals(Start));
            Debug.Assert(startIndex > -1, "There should be an index for the graph start vertex.");

            var endIndex = vertices.FindIndex(s => s.Equals(End));
            Debug.Assert(startIndex > -1, "There should be an index for the graph end vertex.");

            info.AddValue(SerializationStartVertex, startIndex);
            info.AddValue(SerializationEndVertex, endIndex);
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
