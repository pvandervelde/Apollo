//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Core.Extensions.Properties;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// A vertex for the schedule which allows inserting other vertices or schedules in
    /// its position.
    /// </summary>
    /// <remarks>
    /// All editable schedule vertices should be immutable because a schedule is copied
    /// by reusing the vertices.
    /// </remarks>
    [Serializable]
    public sealed class EditableInsertVertex : IEditableScheduleVertex
    {
        /// <summary>
        /// The number of times the current vertex can be replaced with another vertex.
        /// </summary>
        private readonly int m_RemainingInserts;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableInsertVertex"/> class with
        /// an unlimited number of inserts.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        public EditableInsertVertex(int index)
        {
            Index = index;
            m_RemainingInserts = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableInsertVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        /// <param name="maximumNumberOfInserts">The maximum number of times this node can be replaced with another node.</param>
        /// <exception cref="CannotCreateInsertVertexWithLessThanOneInsertException">
        /// Thrown when <paramref name="maximumNumberOfInserts"/> is not a positive integer larger than zero.
        /// </exception>
        public EditableInsertVertex(int index, int maximumNumberOfInserts)
        {
            {
                Lokad.Enforce.With<CannotCreateInsertVertexWithLessThanOneInsertException>(
                    maximumNumberOfInserts == -1 || maximumNumberOfInserts > 0, 
                    Resources.Exceptions_Messages_CannotCreateInsertVertexWithLessThanOneInsert);
            }

            Index = index;
            m_RemainingInserts = maximumNumberOfInserts;
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
        /// Gets the number of times the current vertex can be replaced with another vertex.
        /// </summary>
        public int RemainingInserts
        {
            get
            {
                return m_RemainingInserts;
            }
        }
    }
}
