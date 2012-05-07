//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Utilities.History;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that build schedules that are independant of the
    /// variables which are determined by that schedule.
    /// </summary>
    public interface IBuildFixedSchedules : IBuildSchedules
    {
        /// <summary>
        /// Adds the executing action with the specified ID to the schedule.
        /// </summary>
        /// <param name="action">The ID of the action that should be added.</param>
        /// <returns>The vertex that contains the information about the given action.</returns>
        EditableExecutingActionVertex AddExecutingAction(ScheduleElementId action);

        /// <summary>
        /// Adds the schedule with the specified ID as a sub-schedule to the current schedule.
        /// </summary>
        /// <param name="schedule">The ID of the sub-schedule.</param>
        /// <param name="waitForFinish">
        /// A flag that indicates if the schedule execution should immediately wait for the sub-schedule to finish executing, or
        /// if there is a suitable synchronization point later on.
        /// </param>
        /// <returns>The vertex that contains the information about the given sub-schedule.</returns>
        EditableSubScheduleVertex AddSubSchedule(ScheduleId schedule, bool waitForFinish);

        /// <summary>
        /// Adds a vertex that indicates the start of a synchronization block over which the given variables 
        /// should be synchronized when the block ends.
        /// </summary>
        /// <param name="variables">The collection of variables that should be synchronized.</param>
        /// <returns>The vertex that contains the synchronization information.</returns>
        EditableSynchronizationStartVertex AddSynchronizationStartPoint(IEnumerable<IScheduleVariable> variables);

        /// <summary>
        /// Adds a vertex that indicates the end of a synchronization block.
        /// </summary>
        /// <param name="startPoint">The vertex that forms the start point of the block.</param>
        /// <returns>The vertex that indicates the end of a synchronization block.</returns>
        EditableSynchronizationEndVertex AddSynchronizationEndPoint(EditableSynchronizationStartVertex startPoint);

        /// <summary>
        /// Adds a vertex which indicates that the current values of all history-enabled data should
        /// be stored in the <see cref="Timeline"/> so that it is possible to revert to the
        /// current point in time later on.
        /// </summary>
        /// <returns>The vertex that indicates that the current state should be stored in the <see cref="Timeline"/>.</returns>
        EditableMarkHistoryVertex AddHistoryMarkingPoint();

        /// <summary>
        /// Adds a vertex which can be replaced by another set of vertices.
        /// </summary>
        /// <returns>The vertex that indicates a place in the schedule where new vertices can be inserted.</returns>
        EditableInsertVertex AddInsertPoint();
        
        /// <summary>
        /// Inserts the given vertex in the position of the given insert vertex. The insert vertex will
        /// be removed if it has no more inserts left.
        /// </summary>
        /// <param name="insertVertex">The vertex which will be replaced.</param>
        /// <param name="vertexToInsert">The new vertex.</param>
        /// <returns>A tuple containing the insert vertices that were place before and after the newly inserted vertex.</returns>
        Tuple<EditableInsertVertex, EditableInsertVertex> InsertIn(
            EditableInsertVertex insertVertex,
            IEditableScheduleVertex vertexToInsert);

        /// <summary>
        /// Inserts the given schedule in the position of the insert vertex. The given schedule
        /// will be connected via its start and end vertices. The insert vertex will be removed
        /// if it has no more inserts left.
        /// </summary>
        /// <param name="insertVertex">The vertex which will be replaced.</param>
        /// <param name="scheduleToInsert">The schedule that will be inserted.</param>
        /// <param name="waitForFinish">
        /// A flag that indicates if the schedule execution should immediately wait for the sub-schedule to finish executing, or
        /// if there is a suitable synchronization point later on.
        /// </param>
        /// <returns>
        /// A tuple containing newly created sub-schedule vertex and the insert vertices that were place before and after 
        /// the newly inserted sub-schedule vertex.
        /// </returns>
        Tuple<EditableInsertVertex, EditableSubScheduleVertex, EditableInsertVertex> InsertIn(
            EditableInsertVertex insertVertex,
            IEditableSchedule scheduleToInsert,
            bool waitForFinish);

        /// <summary>
        /// Links the given start vertex to the end vertex.
        /// </summary>
        /// <param name="start">The start vertex.</param>
        /// <param name="end">The end vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from <paramref name="start"/> to <paramref name="end"/>.
        /// </param>
        void LinkTo(IEditableScheduleVertex start, IEditableScheduleVertex end, ScheduleElementId traverseCondition);

        /// <summary>
        /// Links the start point of the schedule to the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from the start point to <paramref name="vertex"/>.
        /// </param>
        void LinkFromStart(IEditableScheduleVertex vertex, ScheduleElementId traverseCondition);

        /// <summary>
        /// Links the given vertex to the end point of the schedule.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from <paramref name="vertex"/> to the end point.
        /// </param>
        void LinkToEnd(IEditableScheduleVertex vertex, ScheduleElementId traverseCondition);
    }
}
