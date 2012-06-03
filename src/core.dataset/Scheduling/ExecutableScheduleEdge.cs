//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Extensions.Scheduling;
using QuickGraph;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines a directional connection between two <see cref="IExecutableScheduleVertex"/> objects.
    /// </summary>
    internal sealed class ExecutableScheduleEdge : Edge<IExecutableScheduleVertex>
    {
        /// <summary>
        /// The ID of the condition that determines if this edge can be traversed or not.
        /// </summary>
        private readonly ScheduleElementId m_Condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableScheduleEdge"/> class.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="traversingCondition">The ID of the condition that determines if this edge can be traversed or not.</param>
        public ExecutableScheduleEdge(IExecutableScheduleVertex source, IExecutableScheduleVertex target, ScheduleElementId traversingCondition)
            : base(source, target)
        {
            m_Condition = traversingCondition;
        }

        /// <summary>
        /// Gets the ID of the condition that determines if this edge can be traversed or not.
        /// </summary>
        public ScheduleElementId TraversingCondition
        {
            get
            {
                return m_Condition;
            }
        }
    }
}
