//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// A vertex for the schedule which provides the ID number of an action that should be executed.
    /// </summary>
    /// <remarks>
    /// All editable schedule vertices should be immutable because a schedule is copied
    /// by reusing the vertices.
    /// </remarks>
    [Serializable]
    public sealed class EditableExecutingActionVertex : IScheduleVertex
    {
        /// <summary>
        /// The ID of the action that should be executed.
        /// </summary>
        private readonly ScheduleElementId m_Action;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableExecutingActionVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        /// <param name="actionToExecute">The ID of the action that should be executed.</param>
        public EditableExecutingActionVertex(int index, ScheduleElementId actionToExecute)
        {
            {
                Debug.Assert(actionToExecute != null, "The ID of the action should not be a null reference.");
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
