//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using QuickGraph;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Verifies if an <see cref="IEditableSchedule"/> is complete and legal.
    /// </summary>
    internal sealed class ScheduleVerifier : IVertifyScheduleIntegrity
    {
        /// <summary>
        /// The collection that contains all the schedules.
        /// </summary>
        private readonly IStoreSchedules m_Schedules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleVerifier"/> class.
        /// </summary>
        /// <param name="scheduleStorage">The collection that stores all the known schedules.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduleStorage"/> is <see langword="null" />.
        /// </exception>
        public ScheduleVerifier(IStoreSchedules scheduleStorage)
        {
            {
                Lokad.Enforce.Argument(() => scheduleStorage);
            }

            m_Schedules = scheduleStorage;
        }

        /// <summary>
        /// Determines if the given schedule is a valid schedule.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <param name="schedule">The schedule that should be verified.</param>
        /// <param name="onValidationFailure">The action which is invoked for each validation failure.</param>
        /// <remarks>
        /// <para>
        /// The schedule should have the following properties:
        /// </para>
        /// <list type="number">
        /// <item>There is one start node that only has edges pointing away from it.</item>
        /// <item>There is one end node that only has edges pointing towards it.</item>
        /// <item>It is possible to get from the start node to every other node, including the end node.</item>
        /// <item>It is possible to get to the end node from every other node, including the start node.</item>
        /// <item>Two directly connected nodes can only be connected in one way.</item>
        /// <item>A synchronization block consists of a start and end.</item>
        /// <item>Every variable declared in a synchronization start block is updated inside the synchronization block.</item>
        /// <item>A sub-schedule does not link back to the original parent schedule.</item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if the schedule is valid; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="onValidationFailure"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsValid(
            ScheduleId id, 
            IEditableSchedule schedule, 
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            {
                Lokad.Enforce.Argument(() => schedule);
                Lokad.Enforce.Argument(() => onValidationFailure);
            }

            var result = VerifyStartVertexOnlyHasOutboundEdges(schedule, onValidationFailure);
            result &= VerifyEndVertexOnlyHasInboundEdges(schedule, onValidationFailure);
            result &= VerifyTrackForwardsFromStart(schedule, onValidationFailure);
            result &= VerifyTrackBackwardsFromEnd(schedule, onValidationFailure);
            result &= VerifyVerticesAreOnlyConnectedByOneEdgeInGivenDirection(schedule, onValidationFailure);
            result &= VerifySynchronizationBlockHasStartAndEnd(schedule, onValidationFailure);
            result &= VerifySynchronizationVariablesAreUpdatedInBlock(schedule, onValidationFailure);
            result &= VerifySubSchedulesDoNotLinkBackToParentSchedule(id, schedule, onValidationFailure);

            return result;
        }

        private bool VerifyStartVertexOnlyHasOutboundEdges(
            IEditableSchedule schedule,
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            var result = (schedule.NumberOfInboundConnections(schedule.Start) == 0) && (schedule.NumberOfOutboundConnections(schedule.Start) >= 1);
            if (!result)
            {
                onValidationFailure(ScheduleIntegrityFailureType.ScheduleIsMissingStart, schedule.Start);
            }

            return result;
        }

        private bool VerifyEndVertexOnlyHasInboundEdges(
            IEditableSchedule schedule,
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            var result = (schedule.NumberOfOutboundConnections(schedule.End) == 0) && (schedule.NumberOfInboundConnections(schedule.End) >= 1);
            if (!result)
            {
                onValidationFailure(ScheduleIntegrityFailureType.ScheduleIsMissingEnd, schedule.End);
            }

            return result;
        }

        private bool VerifyTrackForwardsFromStart(
            IEditableSchedule schedule,
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            var unvisitedNodes = new List<IEditableScheduleVertex>(schedule.Vertices());
            schedule.TraverseSchedule(
                schedule.Start, 
                true, 
                (node, edges) =>
                    {
                        if (unvisitedNodes.Contains(node))
                        {
                            unvisitedNodes.Remove(node);
                        }

                        return true;
                    });

            var result = unvisitedNodes.Count == 0;
            if (!result)
            {
                foreach (var node in unvisitedNodes)
                {
                    onValidationFailure(ScheduleIntegrityFailureType.ScheduleVertexIsNotReachableFromStart, node);
                }
            }

            return result;
        }

        private bool VerifyTrackBackwardsFromEnd(
            IEditableSchedule schedule,
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            var unvisitedNodes = new List<IEditableScheduleVertex>(schedule.Vertices());
            schedule.TraverseSchedule(
                schedule.End,
                false,
                (node, edges) =>
                {
                    if (unvisitedNodes.Contains(node))
                    {
                        unvisitedNodes.Remove(node);
                    }

                    return true;
                });

            var result = unvisitedNodes.Count == 0;
            if (!result)
            {
                foreach (var node in unvisitedNodes)
                {
                    onValidationFailure(ScheduleIntegrityFailureType.ScheduleEndIsNotReachableFromVertex, node);
                }
            }

            return result;
        }

        private bool VerifyVerticesAreOnlyConnectedByOneEdgeInGivenDirection(
            IEditableSchedule schedule,
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            bool result = true;
            schedule.TraverseSchedule(
                schedule.Start, 
                true, 
                (node, edges) =>
                    {
                        var outNodes = new List<IEditableScheduleVertex>();

                        var outEdgeCount = 0;
                        foreach (var pair in edges)
                        {
                            var outNode = pair.Item2;
                            if (!outNodes.Contains(outNode))
                            {
                                outNodes.Add(outNode);
                            }

                            outEdgeCount++;
                        }

                        var internalResult = outEdgeCount == outNodes.Count;
                        result &= internalResult;
                        if (!internalResult)
                        {
                            onValidationFailure(ScheduleIntegrityFailureType.VertexLinksToOtherVertexInMultipleWays, node);
                        }

                        return true;
                    });

            return result;
        }

        private bool VerifySynchronizationBlockHasStartAndEnd(
            IEditableSchedule schedule,
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            // @TODO: Implement VerifySynchronizationBlockHasStartAndEnd
            return true;
        }

        private bool VerifySynchronizationVariablesAreUpdatedInBlock(
            IEditableSchedule schedule,
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            // @TODO: Implement VerifySynchronizationVariablesAreUpdatedInBlock
            return true;
        }

        private bool VerifySubSchedulesDoNotLinkBackToParentSchedule(
            ScheduleId id,
            IEditableSchedule schedule,
            Action<ScheduleIntegrityFailureType, IEditableScheduleVertex> onValidationFailure)
        {
            bool result = true;
            schedule.TraverseSchedule(
                schedule.Start, 
                true, 
                (node, edges) =>
                    {
                        var scheduleNode = node as EditableSubScheduleVertex;
                        if (scheduleNode == null)
                        {
                            return true;
                        }

                        var subScheduleId = scheduleNode.ScheduleToExecute;
                        var isKnownSchedule = m_Schedules.Contains(subScheduleId);
                        if (!isKnownSchedule)
                        {
                            result = false;
                            onValidationFailure(ScheduleIntegrityFailureType.UnknownSubSchedule, node);
                            return false;
                        }

                        var doesSubScheduleLink = DoesSubScheduleLinkTo(id, m_Schedules.Schedule(subScheduleId));
                        if (doesSubScheduleLink)
                        {
                            result = false;
                            onValidationFailure(ScheduleIntegrityFailureType.SubScheduleLinksBackToParentSchedule, node);
                        }

                        return true;
                    });

            return result;
        }

        private bool DoesSubScheduleLinkTo(ScheduleId scheduleId, IEditableSchedule schedule)
        {
            var result = false;
            schedule.TraverseSchedule(
                schedule.Start, 
                true, 
                (node, edges) =>
                    {
                        var scheduleNode = node as EditableSubScheduleVertex;
                        if (scheduleNode == null)
                        {
                            return true;
                        }

                        var subScheduleId = scheduleNode.ScheduleToExecute;
                        if (subScheduleId.Equals(scheduleId))
                        {
                            result = true;
                            return false;
                        }

                        var isKnownSchedule = m_Schedules.Contains(subScheduleId);
                        if (!isKnownSchedule)
                        {
                            result = false;
                            return true;
                        }

                        var doesSubScheduleLink = DoesSubScheduleLinkTo(scheduleId, m_Schedules.Schedule(subScheduleId));
                        if (doesSubScheduleLink)
                        {
                            result = true;
                            return false;
                        }

                        return true;
                    });

            return result;
        }
    }
}
