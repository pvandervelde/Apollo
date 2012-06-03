//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines an <see cref="IExecutableScheduleVertex"/> which indicates the end of a synchronization block.
    /// </summary>
    internal sealed class ExecutableSynchronizationEndVertex : IExecutableScheduleVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableSynchronizationEndVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        public ExecutableSynchronizationEndVertex(int index)
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
