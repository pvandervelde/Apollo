//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines an <see cref="IScheduleVertex"/> which indicates the start point of a schedule.
    /// </summary>
    internal sealed class ExecutableStartVertex : IScheduleVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableStartVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        public ExecutableStartVertex(int index)
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
