//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Extensions.Scheduling;
using QuickGraph;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Defines a graph connection between two <see cref="IScheduleVertex"/> objects.
    /// </summary>
    [Serializable]
    [CLSCompliant(false)]
    public sealed class ScheduleEdge : Edge<IScheduleVertex>
    {
        /// <summary>
        /// The ID of the condition that indicates if the current edge can be traversed.
        /// </summary>
        private readonly ScheduleElementId m_TraversingCondition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleEdge"/> class.
        /// </summary>
        /// <param name="source">The start vertex for the edge.</param>
        /// <param name="target">The end vertex for the edge.</param>
        /// <param name="traversingCondition">The ID of the condition that determines if this edge can be traversed.</param>
        public ScheduleEdge(IScheduleVertex source, IScheduleVertex target, ScheduleElementId traversingCondition = null)
            : base(source, target)
        {
            {
                Debug.Assert(source != null, "The source vertex cannot be a null reference.");
                Debug.Assert(target != null, "The target vertex cannot be a null reference.");
            }

            m_TraversingCondition = traversingCondition;
        }

        /// <summary>
        /// Gets the ID number of the condition that indicates if the current edge can be traversed.
        /// </summary>
        public ScheduleElementId TraversingCondition
        {
            get
            {
                return m_TraversingCondition;
            }
        }
    }
}
