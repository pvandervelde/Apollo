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
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "End",
            Justification = "It is really the end of the schedule.")]
        EditableEndVertex End
        {
            get;
        }

        /// <summary>
        /// Gets a collection that contains all the known vertices for the schedule.
        /// </summary>
        IEnumerable<IEditableScheduleVertex> Vertices
        {
            get;
        }

        /// <summary>
        /// Traverses the schedule and applies an action to each vertex visited.
        /// </summary>
        /// <param name="start">The vertex where the traverse should be started.</param>
        /// <param name="traverseViaOutboundVertices">
        /// A flag indicating if the schedule should be traversed via the outbound edges of the vertices, or the inbound ones.
        /// </param>
        /// <param name="vertexAction">
        /// The action taken for each vertex that is encountered. The action is provided with the current vertex and
        /// a collection of all outbound, if <paramref name="traverseViaOutboundVertices"/> is <see langword="true" />,
        /// or inbound vertices and the ID of the traversing condition. The function should return <see langword="false" />
        /// to terminate the traverse.
        /// </param>
        void TraverseSchedule(
            IEditableScheduleVertex start, 
            bool traverseViaOutboundVertices, 
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
