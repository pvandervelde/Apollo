//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// A vertex for the schedule which indicates that the schedule graph starts at this position.
    /// </summary>
    /// <remarks>
    /// All editable schedule vertices should be immutable because a schedule is copied
    /// by reusing the vertices.
    /// </remarks>
    [Serializable]
    public sealed class EditableStartVertex : IScheduleVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditableStartVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        public EditableStartVertex(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Gets the index of the vertex in the graph.
        /// </summary>
        public int Index
        {
            get;
            private set;
        }
    }
}
