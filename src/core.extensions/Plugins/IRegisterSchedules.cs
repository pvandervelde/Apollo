﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Extensions.Plugins
{
    public interface IRegisterSchedules
    {
        /// <summary>
        /// Adds the executing action with the specified ID to the schedule.
        /// </summary>
        /// <param name="action">The ID of the action that should be added.</param>
        /// <returns>The vertex that contains the information about the given action.</returns>
        EditableExecutingActionVertex AddExecutingAction(ScheduleActionRegistrationId actionMethod);

        /// <summary>
        /// Adds the schedule with the specified ID as a sub-schedule to the current schedule.
        /// </summary>
        /// <param name="schedule">The ID of the sub-schedule.</param>
        /// <returns>The vertex that contains the information about the given sub-schedule.</returns>
        EditableSubScheduleVertex AddSubSchedule(ScheduleId schedule);

        /// <summary>
        /// Adds the schedule with the specified ID as a sub-schedule to the current schedule.
        /// </summary>
        /// <param name="schedule">The ID of the sub-schedule.</param>
        /// <returns>The vertex that contains the information about the given sub-schedule.</returns>
        EditableSubScheduleVertex AddSubSchedule(GroupRegistrationId owningGroup, foobar3 schedule);

        /// <summary>
        /// Adds a vertex that indicates the start of a synchronization block over which the given variables
        /// should be synchronized when the block ends.
        /// </summary>
        /// <param name="variables">The collection of variables that should be synchronized.</param>
        /// <returns>The vertex that contains the synchronization information.</returns>
        EditableSynchronizationStartVertex AddSynchronizationStart(IEnumerable<IScheduleVariable> variables);

        /// <summary>
        /// Adds a vertex that indicates the end of a synchronization block.
        /// </summary>
        /// <param name="startPoint">The vertex that forms the start point of the block.</param>
        /// <returns>The vertex that indicates the end of a synchronization block.</returns>
        EditableSynchronizationEndVertex AddSynchronizationEnd(EditableSynchronizationStartVertex startPoint);

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
        /// Adds a vertex which can be replaced by another set of vertices.
        /// </summary>
        /// <param name="maximumNumberOfInserts">The maximum number of times another vertex can be inserted in place of the insert vertex.</param>
        /// <returns>The vertex that indicates a place in the schedule where new vertices can be inserted.</returns>
        EditableInsertVertex AddInsertPoint(int maximumNumberOfInserts);

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
        /// <param name="scheduleToInsert">The ID of the schedule that will be inserted.</param>
        /// <returns>
        /// A tuple containing newly created sub-schedule vertex and the insert vertices that were place before and after
        /// the newly inserted sub-schedule vertex.
        /// </returns>
        Tuple<EditableInsertVertex, EditableSubScheduleVertex, EditableInsertVertex> InsertIn(
            EditableInsertVertex insertVertex,
            ScheduleId scheduleToInsert);

        /// <summary>
        /// Links the given start vertex to the end vertex.
        /// </summary>
        /// <param name="source">The start vertex.</param>
        /// <param name="target">The end vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from <paramref name="source"/> to <paramref name="target"/>.
        /// </param>
        void LinkTo(IEditableScheduleVertex source, IEditableScheduleVertex target, ScheduleConditionRegistrationId traverseConditionMethod = null);

        /// <summary>
        /// Links the start point of the schedule to the given vertex.
        /// </summary>
        /// <param name="target">The vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from the start point to <paramref name="target"/>.
        /// </param>
        void LinkFromStart(IEditableScheduleVertex target, ScheduleConditionRegistrationId traverseConditionMethod = null);

        /// <summary>
        /// Links the given vertex to the end point of the schedule.
        /// </summary>
        /// <param name="source">The vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from <paramref name="source"/> to the end point.
        /// </param>
        void LinkToEnd(IEditableScheduleVertex source, ScheduleConditionRegistrationId traverseConditionMethod = null);

        /// <summary>
        /// Registers the schedule with the system.
        /// </summary>
        /// <returns>The ID of the schedule.</returns>
        ScheduleId Register();
    }
}
