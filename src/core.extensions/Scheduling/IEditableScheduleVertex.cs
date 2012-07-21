//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that are stored in an editable schedule as vertices.
    /// </summary>
    /// <remarks>
    /// All editable schedule vertices should be immutable because a schedule is copied
    /// by reusing the vertices.
    /// </remarks>
    public interface IEditableScheduleVertex
    {
        /// <summary>
        /// Gets the index of the vertex in the graph.
        /// </summary>
        int Index
        {
            get;
        }
    }
}
