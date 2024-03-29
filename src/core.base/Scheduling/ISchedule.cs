﻿//-----------------------------------------------------------------------
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
    /// Defines the interface for objects that represent a schedule of actions which can be executed
    /// in the order given by the schedule.
    /// </summary>
    public interface ISchedule
    {
        /// <summary>
        /// Gets the start vertex for the schedule.
        /// </summary>
        IScheduleVertex Start
        {
            get;
        }

        /// <summary>
        /// Gets the end vertex for the schedule.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "End",
            Justification = "It is really the end of the schedule.")]
        IScheduleVertex End
        {
            get;
        }

        /// <summary>
        /// Gets a collection that contains all the known vertices for the schedule.
        /// </summary>
        IEnumerable<IScheduleVertex> Vertices
        {
            get;
        }

        /// <summary>
        /// Returns the number of vertices which connect to the current vertex.
        /// </summary>
        /// <param name="origin">The vertex for which the number of inbound connections should be returned.</param>
        /// <returns>The number of inbound connections for the current vertex.</returns>
        int NumberOfInboundConnections(IScheduleVertex origin);

        /// <summary>
        /// Returns the number of vertices to which the current vertex connects.
        /// </summary>
        /// <param name="origin">The vertex for which the number of outbound connections should be returned.</param>
        /// <returns>The number of outbound connections for the current vertex.</returns>
        int NumberOfOutboundConnections(IScheduleVertex origin);

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
        /// <param name="traverseViaOutboundVertices">
        /// A flag indicating if the schedule should be traversed via the outbound edges of the vertices, or the inbound ones.
        /// </param>
        void TraverseAllScheduleVertices(
            IScheduleVertex start, 
            Func<IScheduleVertex, IEnumerable<Tuple<ScheduleElementId, IScheduleVertex>>, bool> vertexAction, 
            bool traverseViaOutboundVertices = true);

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
        void TraverseSchedule(
            IScheduleVertex start,
            Func<IScheduleVertex, bool> vertexAction,
            Func<IEnumerable<Tuple<ScheduleElementId, IScheduleVertex>>, IScheduleVertex> directionAction,
            bool traverseViaOutboundVertices = true);
    }
}
