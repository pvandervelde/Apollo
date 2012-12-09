//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Verifies if an <see cref="ISchedule"/> is complete and legal.
    /// </summary>
    internal sealed class ScheduleVerifier : IVerifyScheduleIntegrity
    {
        private static bool VerifyStartVertexOnlyHasOutboundEdges(
            ISchedule schedule,
            Action<ScheduleIntegrityFailureType, IScheduleVertex> onValidationFailure)
        {
            var result = (schedule.NumberOfInboundConnections(schedule.Start) == 0) && (schedule.NumberOfOutboundConnections(schedule.Start) >= 1);
            if (!result)
            {
                onValidationFailure(ScheduleIntegrityFailureType.ScheduleIsMissingStart, schedule.Start);
            }

            return result;
        }

        private static bool VerifyEndVertexOnlyHasInboundEdges(
            ISchedule schedule,
            Action<ScheduleIntegrityFailureType, IScheduleVertex> onValidationFailure)
        {
            var result = (schedule.NumberOfOutboundConnections(schedule.End) == 0) && (schedule.NumberOfInboundConnections(schedule.End) >= 1);
            if (!result)
            {
                onValidationFailure(ScheduleIntegrityFailureType.ScheduleIsMissingEnd, schedule.End);
            }

            return result;
        }

        private static bool VerifyTrackForwardsFromStart(
            ISchedule schedule,
            Action<ScheduleIntegrityFailureType, IScheduleVertex> onValidationFailure)
        {
            var unvisitedNodes = new List<IScheduleVertex>(schedule.Vertices);
            schedule.TraverseAllScheduleVertices(
                schedule.Start,
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

        private static bool VerifyTrackBackwardsFromEnd(
            ISchedule schedule,
            Action<ScheduleIntegrityFailureType, IScheduleVertex> onValidationFailure)
        {
            var unvisitedNodes = new List<IScheduleVertex>(schedule.Vertices);
            schedule.TraverseAllScheduleVertices(
                schedule.End,
                (node, edges) =>
                {
                    if (unvisitedNodes.Contains(node))
                    {
                        unvisitedNodes.Remove(node);
                    }

                    return true;
                },
                false);

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

        private static bool VerifyVerticesAreOnlyConnectedByOneEdgeInGivenDirection(
            ISchedule schedule,
            Action<ScheduleIntegrityFailureType, IScheduleVertex> onValidationFailure)
        {
            bool result = true;
            schedule.TraverseAllScheduleVertices(
                schedule.Start,
                (node, edges) =>
                {
                    var outNodes = new List<IScheduleVertex>();

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
            ISchedule schedule,
            Action<ScheduleIntegrityFailureType, IScheduleVertex> onValidationFailure)
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
            
            // result &= VerifySynchronizationBlockHasStartAndEnd(schedule, onValidationFailure);
            // result &= VerifySynchronizationVariablesAreUpdatedInBlock(schedule, onValidationFailure);
            result &= VerifySubSchedulesDoNotLinkBackToParentSchedule(id, schedule, onValidationFailure);

            return result;
        }

        private bool VerifySubSchedulesDoNotLinkBackToParentSchedule(
            ScheduleId id,
            ISchedule schedule,
            Action<ScheduleIntegrityFailureType, IScheduleVertex> onValidationFailure)
        {
            bool result = true;
            schedule.TraverseAllScheduleVertices(
                schedule.Start, 
                (node, edges) =>
                    {
                        var scheduleNode = node as SubScheduleVertex;
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

        private bool DoesSubScheduleLinkTo(ScheduleId scheduleId, ISchedule schedule)
        {
            var result = false;
            schedule.TraverseAllScheduleVertices(
                schedule.Start, 
                (node, edges) =>
                    {
                        var scheduleNode = node as SubScheduleVertex;
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
