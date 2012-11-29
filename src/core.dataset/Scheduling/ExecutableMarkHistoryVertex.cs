//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities.History;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines an <see cref="IScheduleVertex"/> which indicates that the changes should be 
    /// stored in a <see cref="Timeline"/>.
    /// </summary>
    internal sealed class ExecutableMarkHistoryVertex : IScheduleVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableMarkHistoryVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        public ExecutableMarkHistoryVertex(int index)
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
