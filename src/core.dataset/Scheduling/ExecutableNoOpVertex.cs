//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines an <see cref="IExecutableScheduleVertex"/> which indicates that no operation should take place at
    /// the current point in the schedule.
    /// </summary>
    /// <remarks>
    /// No-op nodes are used as fill in nodes, normally used to replace the <see cref="EditableInsertVertex"/>
    /// in the editable schedule.
    /// </remarks>
    internal sealed class ExecutableNoOpVertex : IExecutableScheduleVertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableNoOpVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        public ExecutableNoOpVertex(int index)
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
