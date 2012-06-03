//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Dataset.Properties;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Scheduling
{
    /// <summary>
    /// Defines an <see cref="IExecutableScheduleVertex"/> which indicates the start of a synchronization block.
    /// </summary>
    internal sealed class ExecutableSynchronizationStartVertex : IExecutableScheduleVertex
    {
        /// <summary>
        /// The collection of variables which should be synchronized at the end of the block.
        /// </summary>
        private readonly IEnumerable<IScheduleVariable> m_Variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableSynchronizationStartVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        /// <param name="variablesToSynchronize">The collection of variables which should be synchronized at the end of the block.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="variablesToSynchronize"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="CannotCreateASynchronizationBlockWithoutVariablesException">
        ///     Thrown if <paramref name="variablesToSynchronize"/> is an empty collection.
        /// </exception>
        public ExecutableSynchronizationStartVertex(int index, IEnumerable<IScheduleVariable> variablesToSynchronize)
        {
            {
                Lokad.Enforce.Argument(() => variablesToSynchronize);
                Lokad.Enforce.With<CannotCreateASynchronizationBlockWithoutVariablesException>(
                    variablesToSynchronize.Any(), 
                    Resources.Exceptions_Messages_CannotCreateASynchronizationBlockWithoutVariables);
            }

            Index = index;
            m_Variables = variablesToSynchronize;
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
        /// Gets the collection of variables which should be synchronized at the end of the block.
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
