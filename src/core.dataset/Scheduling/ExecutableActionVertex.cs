//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines an <see cref="IExecutableScheduleVertex"/> which has an action that should be executed.
    /// </summary>
    internal sealed class ExecutableActionVertex : IExecutableScheduleVertex
    {
        /// <summary>
        /// The ID of the action which should be executed.
        /// </summary>
        private readonly ScheduleElementId m_Action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableActionVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        /// <param name="actionToExecute">The ID of the action that should be executed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actionToExecute"/> is <see langword="null" />.
        /// </exception>
        public ExecutableActionVertex(int index, ScheduleElementId actionToExecute)
        {
            {
                Lokad.Enforce.Argument(() => actionToExecute);
            }

            Index = index;
            m_Action = actionToExecute;
        }

        /// <summary>
        /// Gets the index of the vertex in the graph.
        /// </summary>
        public int Index
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID of the action that should be executed.
        /// </summary>
        public ScheduleElementId ActionToExecute
        {
            get
            {
                return m_Action;
            }
        }
    }
}
