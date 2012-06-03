//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Extensions.Properties;
using QuickGraph;

namespace Apollo.Core.Extensions.Scheduling
{
    internal sealed class EditableSchedule : IEditableSchedule
    {
        /// <summary>
        /// The graph of schedule nodes.
        /// </summary>
        private readonly BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge> m_Graph;

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
        private readonly EditableStartVertex m_Start;

        /// <summary>
        /// The vertex that forms the end of the schedule.
        /// </summary>
        /// <remarks>
        /// If the current schedule is inserted in another schedule as a schedule-block then this end vertex will
        /// be connected to the insert node.
        /// </remarks>
        private readonly EditableEndVertex m_End;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSchedule"/> class.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <param name="graph">The graph that describes the current schedule.</param>
        /// <param name="start">The start node for the schedule.</param>
        /// <param name="end">The end node for the schedule.</param>
        public EditableSchedule(
            ScheduleId id, 
            BidirectionalGraph<IEditableScheduleVertex, EditableScheduleEdge> graph,
            EditableStartVertex start,
            EditableEndVertex end)
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
        public EditableStartVertex Start
        {
            get
            {
                return m_Start;
            }
        }

        /// <summary>
        /// Gets the end vertex for the schedule.
        /// </summary>
        public EditableEndVertex End
        {
            get
            {
                return m_End;
            }
        }

        /// <summary>
        /// Traverses the schedule and applies an action to each vertex visited.
        /// </summary>
        /// <param name="start">The vertex where the traverse should be started.</param>
        /// <param name="traverseViaOutBoundVertices">
        /// A flag indicating if the schedule should be traversed via the outbound edges of the vertices, or the inbound ones.
        /// </param>
        /// <param name="vertexAction">
        /// The action taken for each vertex that is encountered. The action is provided with the current vertex and
        /// a collection of all outbound, if <paramref name="traverseViaOutBoundVertices"/> is <see langword="true" />,
        /// or inbound vertices and the ID of the traversing condition. The function should return <see langword="false" />
        /// to terminate the traverse.
        /// </param>
        public void TraverseSchedule(
            IEditableScheduleVertex start,
            bool traverseViaOutBoundVertices,
            Func<IEditableScheduleVertex, IEnumerable<Tuple<ScheduleElementId, IEditableScheduleVertex>>, bool> vertexAction)
        {
            {
                Lokad.Enforce.Argument(() => start);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Graph.ContainsVertex(start), 
                    Resources.Exceptions_Messages_UnknownScheduleVertex);

                Lokad.Enforce.Argument(() => vertexAction);
            }

            var nodeCounter = new List<IEditableScheduleVertex>();

            var uncheckedVertices = new Queue<IEditableScheduleVertex>();
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
                                  select new Tuple<ScheduleElementId, IEditableScheduleVertex>(
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
        /// Returns a collection that contains all the known vertices for the schedule.
        /// </summary>
        /// <returns>The collection that contains all the known vertices for the schedule.</returns>
        public IEnumerable<IEditableScheduleVertex> Vertices()
        {
            return m_Graph.Vertices;
        }

        /// <summary>
        /// Returns the number of vertices which connect to the current vertex.
        /// </summary>
        /// <param name="origin">The vertex for which the number of inbound connections should be returned.</param>
        /// <returns>The number of inbound connections for the current vertex.</returns>
        public int NumberOfInboundConnections(IEditableScheduleVertex origin)
        {
            return m_Graph.InDegree(origin);
        }

        /// <summary>
        /// Returns the number of vertices to which the current vertex connects.
        /// </summary>
        /// <param name="origin">The vertex for which the number of outbound connections should be returned.</param>
        /// <returns>The number of outbound connections for the current vertex.</returns>
        public int NumberOfOutboundConnections(IEditableScheduleVertex origin)
        {
            return m_Graph.OutDegree(origin);
        }

        /// <summary>
        /// Returns a collection containing all the insert points for the schedule.
        /// </summary>
        /// <returns>The collection that contains all the insert points for the schedule.</returns>
        public IEnumerable<EditableInsertVertex> InsertPoints()
        {
            var result = from vertex in m_Graph.Vertices
                         where vertex is EditableInsertVertex
                         select vertex as EditableInsertVertex;

            return result;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var schedule = obj as IEditableSchedule;
            return Equals(schedule);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return m_Id.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}",
                m_Id);
        }
    }
}
