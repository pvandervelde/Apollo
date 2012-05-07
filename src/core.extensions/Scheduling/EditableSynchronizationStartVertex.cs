//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// A vertex for the <see cref="IEditableSchedule"/> which marks the position where for a set of variables
    /// the revision numbers should be marked.
    /// </summary>
    [Serializable]
    public sealed class EditableSynchronizationStartVertex : IEditableScheduleVertex
    {
        /// <summary>
        /// The collection of variables which should be synchronized at the end of the block.
        /// </summary>
        private readonly IEnumerable<IScheduleVariable> m_Variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSynchronizationStartVertex"/> class.
        /// </summary>
        /// <param name="variables">The collection of variables which need to be synchronized at the end of the block.</param>
        internal EditableSynchronizationStartVertex(IEnumerable<IScheduleVariable> variables)
        {
            {
                Debug.Assert(variables != null, "The collection of synchronization variables should not be a null reference.");
                Debug.Assert(variables.Any(), "There should at least be one variable to synchronize on.");
            }

            m_Variables = variables;
        }

        /// <summary>
        /// Gets the collection of variables which need to be synchronized at the end of the block.
        /// </summary>
        public IEnumerable<IScheduleVariable> VariablesToSynchronizeOn
        {
            get 
            {
                return m_Variables;
            }
        }
    }
}
