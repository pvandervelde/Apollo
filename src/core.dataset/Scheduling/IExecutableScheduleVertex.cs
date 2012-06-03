//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines the interface for objects that form the vertices in a <see cref="ExecutableSchedule"/>.
    /// </summary>
    internal interface IExecutableScheduleVertex
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
