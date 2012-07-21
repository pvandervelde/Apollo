//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines an <see cref="IExecutableScheduleVertex"/> which indicates the endpoint of a schedule.
    /// </summary>
    internal sealed class ExecutableEndVertex : IExecutableScheduleVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableEndVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        public ExecutableEndVertex(int index)
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
