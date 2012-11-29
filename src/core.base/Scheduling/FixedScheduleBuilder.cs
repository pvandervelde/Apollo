//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Base.Properties;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities.History;
using QuickGraph;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Defines the methods for creating a fixed schedule.
    /// </summary>
    internal sealed class FixedScheduleBuilder : IBuildFixedSchedules
    {
        /// <summary>
        /// The collection that maps an editable <see cref="IScheduleVertex"/> type to a function that can create the
        /// equivalent executable <see cref="IScheduleVertex"/>.
        /// </summary>
        private static readonly Dictionary<Type, Func<IScheduleVertex, int, IScheduleVertex>> s_VertexBuilder
            = new Dictionary<Type, Func<IScheduleVertex, int, IScheduleVertex>>
            {
                { 
                    typeof(EditableStartVertex), 
                    (vertex, index) => new EditableStartVertex(index) 
                },
                { 
                    typeof(EditableEndVertex), 
                    (vertex, index) => new EditableEndVertex(index) 
                },
                { 
                    typeof(EditableInsertVertex), 
                    (vertex, index) => new EditableInsertVertex(index) 
                },
                { 
                    typeof(EditableMarkHistoryVertex), 
                    (vertex, index) => new EditableMarkHistoryVertex(index) 
                },
                { 
                    typeof(EditableSynchronizationStartVertex), 
                    (vertex, index) => new EditableSynchronizationStartVertex(
                        index, 
                        ((EditableSynchronizationStartVertex)vertex).VariablesToSynchronizeOn)
                },
                { 
                    typeof(EditableSynchronizationEndVertex), 
                    (vertex, index) => new EditableSynchronizationEndVertex(index) 
                },
                {
                    typeof(EditableExecutingActionVertex),
                    (vertex, index) => new EditableExecutingActionVertex(
                        index,
                        ((EditableExecutingActionVertex)vertex).ActionToExecute)
                },
                { 
                    typeof(EditableSubScheduleVertex), 
                    (vertex, index) => new EditableSubScheduleVertex(
                        index, 
                        ((EditableSubScheduleVertex)vertex).ScheduleToExecute) 
                },
            };

        private static IScheduleVertex CloneVertex(IScheduleVertex vertex, int newVertexIndex)
        {
            return s_VertexBuilder[vertex.GetType()](vertex, newVertexIndex);
        }

        private static Tuple<BidirectionalGraph<IScheduleVertex, ScheduleEdge>, IScheduleVertex, IScheduleVertex> CopyGraph(
            BidirectionalGraph<IScheduleVertex, ScheduleEdge> graph,
            IScheduleVertex start)
        {
            var map = new Dictionary<IScheduleVertex, IScheduleVertex>();
            var newGraph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>(false);

            var startVertex = CloneVertex(start, newGraph.VertexCount);
            newGraph.AddVertex(startVertex);
            map.Add(start, startVertex);

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

                var outEdges = graph.OutEdges(source);
                foreach (var outEdge in outEdges)
                {
                    var target = outEdge.Target;
                    if (!map.ContainsKey(target))
                    {
                        var targetVertex = CloneVertex(target, newGraph.VertexCount);
                        newGraph.AddVertex(targetVertex);
                        map.Add(target, targetVertex);
                    }

                    var edgeSource = map[source];
                    var edgeTarget = map[target];
                    newGraph.AddEdge(new ScheduleEdge(edgeSource, edgeTarget, outEdge.TraversingCondition));

                    uncheckedVertices.Enqueue(outEdge.Target);
                }
            }

            var endVertex = map.First(p => p.Value is EditableEndVertex).Value;
            return new Tuple<BidirectionalGraph<IScheduleVertex, ScheduleEdge>, IScheduleVertex, IScheduleVertex>(newGraph, startVertex, endVertex);
        }

        private static Tuple<BidirectionalGraph<IScheduleVertex, ScheduleEdge>, IScheduleVertex, IScheduleVertex> CopySchedule(
            IEditableSchedule schedule)
        {
            var map = new Dictionary<IScheduleVertex, IScheduleVertex>();
            var newSchedule = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = CloneVertex(schedule.Start, newSchedule.VertexCount);
            newSchedule.AddVertex(start);
            map.Add(schedule.Start, start);

            var end = CloneVertex(schedule.End, newSchedule.VertexCount);
            newSchedule.AddVertex(end);
            map.Add(schedule.End, end);

            schedule.TraverseSchedule(
                schedule.Start,
                true,
                (vertex, edges) =>
                {
                    foreach (var pair in edges)
                    {
                        var target = pair.Item2;
                        if (!map.ContainsKey(target))
                        {
                            var executableVertex = CloneVertex(target, newSchedule.VertexCount);
                            map.Add(target, executableVertex);
                            newSchedule.AddVertex(executableVertex);
                        }

                        var executableSource = map[vertex];
                        var executableTarget = map[target];
                        newSchedule.AddEdge(new ScheduleEdge(executableSource, executableTarget, pair.Item1));
                    }

                    return true;
                });

            return new Tuple<BidirectionalGraph<IScheduleVertex, ScheduleEdge>, IScheduleVertex, IScheduleVertex>(newSchedule, start, end);
        }

        /// <summary>
        /// The schedule that is being edited.
        /// </summary>
        private readonly BidirectionalGraph<IScheduleVertex, ScheduleEdge> m_Schedule;

        /// <summary>
        /// The start point of the schedule.
        /// </summary>
        private readonly IScheduleVertex m_Start;

        /// <summary>
        /// The end point of the schedule.
        /// </summary>
        private readonly IScheduleVertex m_End;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedScheduleBuilder"/> class.
        /// </summary>
        public FixedScheduleBuilder()
        {
            m_Schedule = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>(false);

            m_Start = new EditableStartVertex(m_Schedule.VertexCount);
            m_Schedule.AddVertex(m_Start);

            m_End = new EditableEndVertex(m_Schedule.VertexCount);
            m_Schedule.AddVertex(m_End);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedScheduleBuilder"/> class.
        /// </summary>
        /// <param name="scheduleToStartWith">The schedule that will be copied as base schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduleToStartWith"/> is <see langword="null" />.
        /// </exception>
        public FixedScheduleBuilder(IEditableSchedule scheduleToStartWith)
        {
            {
                Lokad.Enforce.Argument(() => scheduleToStartWith);
            }

            var tuple = CopySchedule(scheduleToStartWith);
            m_Schedule = tuple.Item1;
            m_Start = tuple.Item2;
            m_End = tuple.Item3;
        }

        /// <summary>
        /// Adds the executing action with the specified ID to the schedule.
        /// </summary>
        /// <param name="action">The ID of the action that should be added.</param>
        /// <returns>The vertex that contains the information about the given action.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="action"/> is <see langword="null" />.
        /// </exception>
        public EditableExecutingActionVertex AddExecutingAction(ScheduleElementId action)
        {
            {
                Lokad.Enforce.Argument(() => action);
            }

            var result = new EditableExecutingActionVertex(m_Schedule.VertexCount, action);
            m_Schedule.AddVertex(result);

            return result;
        }

        /// <summary>
        /// Adds the schedule with the specified ID as a sub-schedule to the current schedule.
        /// </summary>
        /// <param name="schedule">The ID of the sub-schedule.</param>
        /// <returns>The vertex that contains the information about the given sub-schedule.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        public EditableSubScheduleVertex AddSubSchedule(ScheduleId schedule)
        {
            {
                Lokad.Enforce.Argument(() => schedule);
            }

            var result = new EditableSubScheduleVertex(m_Schedule.VertexCount, schedule);
            m_Schedule.AddVertex(result);

            return result;
        }

        /// <summary>
        /// Adds a vertex that indicates the start of a synchronization block over which the given variables 
        /// should be synchronized when the block ends.
        /// </summary>
        /// <param name="variables">The collection of variables that should be synchronized.</param>
        /// <returns>The vertex that contains the synchronization information.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="variables"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotCreateASynchronizationBlockWithoutVariablesException">
        ///     Thrown if <paramref name="variables"/> is an empty collection.
        /// </exception>
        public EditableSynchronizationStartVertex AddSynchronizationStart(IEnumerable<IScheduleVariable> variables)
        {
            {
                Lokad.Enforce.Argument(() => variables);
                Lokad.Enforce.With<CannotCreateASynchronizationBlockWithoutVariablesException>(
                    variables.Any(),
                    Resources.Exceptions_Messages_CannotCreateASynchronizationBlockWithoutVariables);
            }

            var result = new EditableSynchronizationStartVertex(m_Schedule.VertexCount, variables);
            m_Schedule.AddVertex(result);

            return result;
        }

        /// <summary>
        /// Adds a vertex that indicates the end of a synchronization block.
        /// </summary>
        /// <param name="startPoint">The vertex that forms the start point of the block.</param>
        /// <returns>The vertex that indicates the end of a synchronization block.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="startPoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleVertexException">
        ///     Thrown if <paramref name="startPoint"/> is not part of the current schedule.
        /// </exception>
        public EditableSynchronizationEndVertex AddSynchronizationEnd(EditableSynchronizationStartVertex startPoint)
        {
            {
                Lokad.Enforce.Argument(() => startPoint);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Schedule.ContainsVertex(startPoint),
                    Resources.Exceptions_Messages_UnknownScheduleVertex);
            }

            var result = new EditableSynchronizationEndVertex(m_Schedule.VertexCount);
            m_Schedule.AddVertex(result);

            return result;
        }

        /// <summary>
        /// Adds a vertex which indicates that the current values of all history-enabled data should
        /// be stored in the <see cref="Timeline"/> so that it is possible to revert to the
        /// current point in time later on.
        /// </summary>
        /// <returns>The vertex that indicates that the current state should be stored in the <see cref="Timeline"/>.</returns>
        public EditableMarkHistoryVertex AddHistoryMarkingPoint()
        {
            var result = new EditableMarkHistoryVertex(m_Schedule.VertexCount);
            m_Schedule.AddVertex(result);

            return result;
        }

        /// <summary>
        /// Adds a vertex which can be replaced by another set of vertices.
        /// </summary>
        /// <returns>The vertex that indicates a place in the schedule where new vertices can be inserted.</returns>
        public EditableInsertVertex AddInsertPoint()
        {
            var result = new EditableInsertVertex(m_Schedule.VertexCount);
            m_Schedule.AddVertex(result);

            return result;
        }

        /// <summary>
        /// Adds a vertex which can be replaced by another set of vertices.
        /// </summary>
        /// <param name="maximumNumberOfInserts">The maximum number of times another vertex can be inserted in place of the insert vertex.</param>
        /// <returns>The vertex that indicates a place in the schedule where new vertices can be inserted.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="maximumNumberOfInserts"/> is zero or smaller.
        /// </exception>
        public EditableInsertVertex AddInsertPoint(int maximumNumberOfInserts)
        {
            {
                Lokad.Enforce.With<ArgumentOutOfRangeException>(
                    maximumNumberOfInserts > 0,
                    Resources.Exceptions_Messages_CannotCreateInsertVertexWithLessThanOneInsert);
            }

            var result = new EditableInsertVertex(m_Schedule.VertexCount, maximumNumberOfInserts);
            m_Schedule.AddVertex(result);

            return result;
        }

        /// <summary>
        /// Inserts the given vertex in the position of the given insert vertex. The insert vertex will
        /// be removed if it has no more inserts left.
        /// </summary>
        /// <param name="insertVertex">The vertex which will be replaced.</param>
        /// <param name="vertexToInsert">The new vertex.</param>
        /// <returns>A tuple containing the insert vertices that were place before and after the newly inserted vertex.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="insertVertex"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleVertexException">
        ///     Thrown if <paramref name="insertVertex"/> does not exist in the current schedule.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="vertexToInsert"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotInsertExistingVertexException">
        ///     Thrown if <paramref name="vertexToInsert"/> already exists in the schedule.
        /// </exception>
        /// <exception cref="NoInsertsLeftOnVertexException">
        ///     Thrown if <paramref name="insertVertex"/> has no more inserts left.
        /// </exception>
        public Tuple<EditableInsertVertex, EditableInsertVertex> InsertIn(
            EditableInsertVertex insertVertex,
            IScheduleVertex vertexToInsert)
        {
            {
                Lokad.Enforce.Argument(() => insertVertex);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Schedule.ContainsVertex(insertVertex),
                    Resources.Exceptions_Messages_UnknownScheduleVertex);

                Lokad.Enforce.Argument(() => vertexToInsert);
                Lokad.Enforce.With<CannotInsertExistingVertexException>(
                    !m_Schedule.ContainsVertex(vertexToInsert),
                    Resources.Exceptions_Messages_CannotInsertExistingVertex);

                Lokad.Enforce.With<NoInsertsLeftOnVertexException>(
                    (insertVertex.RemainingInserts == -1) || (insertVertex.RemainingInserts > 0),
                    Resources.Exceptions_Messages_NoInsertsLeftOnVertex);
            }

            // Find the inbound and outbound edges
            var inbound = from edge in m_Schedule.InEdges(insertVertex)
                          select edge;

            var outbound = from edge in m_Schedule.OutEdges(insertVertex)
                           select edge;

            // Add the new node
            m_Schedule.AddVertex(vertexToInsert);

            // Create two new insert vertices to be placed on either side of the new vertex
            var count = (insertVertex.RemainingInserts != -1) ? insertVertex.RemainingInserts - 1 : -1;

            EditableInsertVertex inboundInsert = null;
            EditableInsertVertex outboundInsert = null;
            if ((count == -1) || (count > 0))
            {
                inboundInsert = new EditableInsertVertex(m_Schedule.VertexCount, count);
                m_Schedule.AddVertex(inboundInsert);
                m_Schedule.AddEdge(new ScheduleEdge(inboundInsert, vertexToInsert, null));

                outboundInsert = new EditableInsertVertex(m_Schedule.VertexCount, count);
                m_Schedule.AddVertex(outboundInsert);
                m_Schedule.AddEdge(new ScheduleEdge(vertexToInsert, outboundInsert, null));
            }

            // Reconnect all the edges
            var inboundTarget = inboundInsert ?? vertexToInsert;
            var outboundSource = outboundInsert ?? vertexToInsert;

            foreach (var inboundEdge in inbound)
            {
                m_Schedule.AddEdge(new ScheduleEdge(inboundEdge.Source, inboundTarget, inboundEdge.TraversingCondition));
            }

            foreach (var outboundEdge in outbound)
            {
                m_Schedule.AddEdge(new ScheduleEdge(outboundSource, outboundEdge.Target, outboundEdge.TraversingCondition));
            }

            // Lastly remove the current insert node, which also destroys all the edges that are 
            // connected to it.
            m_Schedule.RemoveVertex(insertVertex);

            return new Tuple<EditableInsertVertex, EditableInsertVertex>(inboundInsert, outboundInsert);
        }

        /// <summary>
        /// Inserts the given schedule in the position of the insert vertex. The given schedule
        /// will be connected via its start and end vertices. The insert vertex will be removed
        /// if it has no more inserts left.
        /// </summary>
        /// <param name="insertVertex">The vertex which will be replaced.</param>
        /// <param name="scheduleToInsert">The ID of the schedule that will be inserted.</param>
        /// <returns>
        /// A tuple containing newly created sub-schedule vertex and the insert vertices that were place before and after 
        /// the newly inserted sub-schedule vertex.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="insertVertex"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleVertexException">
        ///     Thrown if <paramref name="insertVertex"/> does not exist in the current schedule.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduleToInsert"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NoInsertsLeftOnVertexException">
        ///     Thrown if <paramref name="insertVertex"/> has no more inserts left.
        /// </exception>
        public Tuple<EditableInsertVertex, EditableSubScheduleVertex, EditableInsertVertex> InsertIn(
            EditableInsertVertex insertVertex,
            ScheduleId scheduleToInsert)
        {
            var subScheduleVertex = new EditableSubScheduleVertex(m_Schedule.VertexCount, scheduleToInsert);
            var internalResult = InsertIn(insertVertex, subScheduleVertex);

            return new Tuple<EditableInsertVertex, EditableSubScheduleVertex, EditableInsertVertex>(
                internalResult.Item1,
                subScheduleVertex,
                internalResult.Item2);
        }

        /// <summary>
        /// Links the given start vertex to the end vertex.
        /// </summary>
        /// <param name="start">The start vertex.</param>
        /// <param name="end">The end vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from <paramref name="start"/> to <paramref name="end"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="start"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleVertexException">
        ///     Thrown if <paramref name="start"/> does not exist in the current schedule.
        /// </exception>
        /// <exception cref="CannotExplicitlyLinkStartVertexException">
        ///     Thrown if <paramref name="start"/> is equal to the start vertex of the schedule.
        /// </exception>
        /// <exception cref="CannotExplicitlyLinkStartVertexException">
        ///     Thrown if <paramref name="start"/> is equal to the end vertex of the schedule.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="end"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleVertexException">
        ///     Thrown if <paramref name="end"/> does not exist in the current schedule.
        /// </exception>
        /// <exception cref="CannotExplicitlyLinkStartVertexException">
        ///     Thrown if <paramref name="end"/> is equal to the start vertex of the schedule.
        /// </exception>
        /// <exception cref="CannotExplicitlyLinkStartVertexException">
        ///     Thrown if <paramref name="end"/> is equal to the end vertex of the schedule.
        /// </exception>
        /// <exception cref="CannotLinkAVertexToItselfException">
        ///     Thrown if <paramref name="start"/> and <paramref name="end"/> are the same vertex.
        /// </exception>
        public void LinkTo(IScheduleVertex start, IScheduleVertex end, ScheduleElementId traverseCondition = null)
        {
            {
                Lokad.Enforce.Argument(() => start);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Schedule.ContainsVertex(start),
                    Resources.Exceptions_Messages_UnknownScheduleVertex);
                Lokad.Enforce.With<CannotExplicitlyLinkStartVertexException>(
                    !ReferenceEquals(m_Start, start),
                    Resources.Exceptions_Messages_CannotExplicitlyLinkStartVertex);
                Lokad.Enforce.With<CannotExplicitlyLinkEndVertexException>(
                    !ReferenceEquals(m_End, start),
                    Resources.Exceptions_Messages_CannotExplicitlyLinkEndVertex);

                Lokad.Enforce.Argument(() => end);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Schedule.ContainsVertex(end),
                    Resources.Exceptions_Messages_UnknownScheduleVertex);
                Lokad.Enforce.With<CannotExplicitlyLinkStartVertexException>(
                    !ReferenceEquals(m_Start, end),
                    Resources.Exceptions_Messages_CannotExplicitlyLinkStartVertex);
                Lokad.Enforce.With<CannotExplicitlyLinkEndVertexException>(
                    !ReferenceEquals(m_End, end),
                    Resources.Exceptions_Messages_CannotExplicitlyLinkEndVertex);

                Lokad.Enforce.With<CannotLinkAVertexToItselfException>(
                    !ReferenceEquals(start, end),
                    Resources.Exceptions_Messages_CannotLinkAVertexToItself);
            }

            m_Schedule.AddEdge(new ScheduleEdge(start, end, traverseCondition));
        }

        /// <summary>
        /// Links the start point of the schedule to the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from the start point to <paramref name="vertex"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="vertex"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleVertexException">
        ///     Thrown if <paramref name="vertex"/> does not exist in the current schedule.
        /// </exception>
        /// <exception cref="CannotExplicitlyLinkStartVertexException">
        ///     Thrown if <paramref name="vertex"/> is equal to the end vertex of the schedule.
        /// </exception>
        /// <exception cref="CannotLinkAVertexToItselfException">
        ///     Thrown if the start vertex of the schedule and <paramref name="vertex"/> are the same vertex.
        /// </exception>
        public void LinkFromStart(IScheduleVertex vertex, ScheduleElementId traverseCondition = null)
        {
            {
                Lokad.Enforce.Argument(() => vertex);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Schedule.ContainsVertex(vertex),
                    Resources.Exceptions_Messages_UnknownScheduleVertex);

                Lokad.Enforce.With<CannotExplicitlyLinkEndVertexException>(
                    !ReferenceEquals(m_End, vertex),
                    Resources.Exceptions_Messages_CannotExplicitlyLinkEndVertex);
                Lokad.Enforce.With<CannotLinkAVertexToItselfException>(
                    !ReferenceEquals(m_Start, vertex),
                    Resources.Exceptions_Messages_CannotLinkAVertexToItself);
            }

            m_Schedule.AddEdge(new ScheduleEdge(m_Start, vertex, traverseCondition));
        }

        /// <summary>
        /// Links the given vertex to the end point of the schedule.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from <paramref name="vertex"/> to the end point.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="vertex"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownScheduleVertexException">
        ///     Thrown if <paramref name="vertex"/> does not exist in the current schedule.
        /// </exception>
        /// <exception cref="CannotExplicitlyLinkStartVertexException">
        ///     Thrown if <paramref name="vertex"/> is equal to the start vertex of the schedule.
        /// </exception>
        /// <exception cref="CannotLinkAVertexToItselfException">
        ///     Thrown if the end vertex of the schedule and <paramref name="vertex"/> are the same vertex.
        /// </exception>
        public void LinkToEnd(IScheduleVertex vertex, ScheduleElementId traverseCondition = null)
        {
            {
                Lokad.Enforce.Argument(() => vertex);
                Lokad.Enforce.With<UnknownScheduleVertexException>(
                    m_Schedule.ContainsVertex(vertex),
                    Resources.Exceptions_Messages_UnknownScheduleVertex);
                Lokad.Enforce.With<CannotExplicitlyLinkStartVertexException>(
                    !ReferenceEquals(m_Start, vertex),
                    Resources.Exceptions_Messages_CannotExplicitlyLinkStartVertex);
                Lokad.Enforce.With<CannotLinkAVertexToItselfException>(
                    !ReferenceEquals(m_End, vertex),
                    Resources.Exceptions_Messages_CannotLinkAVertexToItself);
            }

            m_Schedule.AddEdge(new ScheduleEdge(vertex, m_End, traverseCondition));
        }

        /// <summary>
        /// Builds the schedule from the stored information.
        /// </summary>
        /// <returns>
        /// A new schedule with all the information that was stored.
        /// </returns>
        public IEditableSchedule Build()
        {
            var tuple = CopyGraph(m_Schedule, m_Start);
            return new EditableSchedule(
                tuple.Item1,
                tuple.Item2,
                tuple.Item3);
        }
    }
}
