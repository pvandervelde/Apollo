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
    /// Defines the interface for a data structure that stores a schedule in editable form.
    /// </summary>
    public interface IEditableSchedule : ISchedule
    {
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
    }
}
