//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Extensions.Properties;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// A vertex for the schedule which marks the position where for a set of variables
    /// the revision numbers should be marked.
    /// </summary>
    [Serializable]
    public sealed class EditableSynchronizationStartVertex : IScheduleVertex
    {
        /// <summary>
        /// The collection of variables which should be synchronized at the end of the block.
        /// </summary>
        private readonly IEnumerable<IScheduleVariable> m_Variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSynchronizationStartVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        /// <param name="variables">The collection of variables which need to be synchronized at the end of the block.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="variables"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotCreateASynchronizationBlockWithoutVariablesException">
        ///     Thrown if <paramref name="variables"/> is an empty collection.
        /// </exception>
        public EditableSynchronizationStartVertex(int index, IEnumerable<IScheduleVariable> variables)
        {
            {
                Lokad.Enforce.Argument(() => variables);
                Lokad.Enforce.With<CannotCreateASynchronizationBlockWithoutVariablesException>(
                    variables.Any(),
                    Resources.Exceptions_Messages_CannotCreateASynchronizationBlockWithoutVariables);
            }

            Index = index;
            m_Variables = variables;
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
