//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for a data structure that stores a schedule in editable form.
    /// </summary>
    public interface IEditableSchedule
    {
        /// <summary>
        /// Gets the start vertex for the schedule.
        /// </summary>
        EditableStartVertex Start
        {
            get;
        }

        /// <summary>
        /// Gets the end vertex for the schedule.
        /// </summary>
        EditableEndVertex End
        {
            get;
        }

        /// <summary>
        /// Returns a collection that contains all the known vertices for the schedule.
        /// </summary>
        /// <returns>The collection that contains all the known vertices for the schedule.</returns>
        IEnumerable<IEditableScheduleVertex> Vertices();

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
        void TraverseSchedule(
            IEditableScheduleVertex start, 
            bool traverseViaOutBoundVertices, 
            Func<IEditableScheduleVertex, IEnumerable<Tuple<ScheduleElementId, IEditableScheduleVertex>>, bool> vertexAction);

        /// <summary>
        /// Returns the number of vertices which connect to the current vertex.
        /// </summary>
        /// <param name="origin">The vertex for which the number of inbound connections should be returned.</param>
        /// <returns>The number of inbound connections for the current vertex.</returns>
        int NumberOfInboundConnections(IEditableScheduleVertex origin);

        /// <summary>
        /// Returns the number of vertices to which the current vertex connects.
        /// </summary>
        /// <param name="origin">The vertex for which the number of outbound connections should be returned.</param>
        /// <returns>The number of outbound connections for the current vertex.</returns>
        int NumberOfOutboundConnections(IEditableScheduleVertex origin);
    }
}
