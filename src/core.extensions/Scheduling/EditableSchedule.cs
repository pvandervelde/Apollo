//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Extensions.Properties;
using QuickGraph;

namespace Apollo.Core.Extensions.Scheduling
{
    internal sealed class EditableSchedule : IEditableSchedule
    {
        /// <summary>
        /// The graph of schedule nodes. Traversable from parent to child only.
        /// </summary>
        private readonly AdjacencyGraph<IEditableScheduleVertex, EditableScheduleEdge> m_Graph;

        /// <summary>
        /// The ID of the current schedule.
        /// </summary>
        private readonly ScheduleId m_Id;

        /// <summary>
        /// The vertex that is forms the start of the schedule.
        /// </summary>
        /// <remarks>
        /// If the current schedule is inserted in another schedule as a schedule-block then this end vertex will
        /// be connected to the insert node.
        /// </remarks>
        private readonly EditableStartVertex m_Start;

        /// <summary>
        /// The vertex that forms the end of the schedule.
        /// </summary>
        /// <remarks>
        /// If the current schedule is inserted in another schedule as a schedule-block then this end vertex will
        /// be connected to the insert node.
        /// </remarks>
        private readonly EditableEndVertex m_End;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableSchedule"/> class.
        /// </summary>
        /// <param name="id">The ID of the schedule.</param>
        /// <param name="graph">The graph that describes the current schedule.</param>
        /// <param name="start">The start node for the schedule.</param>
        /// <param name="end">The end node for the schedule.</param>
        public EditableSchedule(
            ScheduleId id, 
            AdjacencyGraph<IEditableScheduleVertex, EditableScheduleEdge> graph,
            EditableStartVertex start,
            EditableEndVertex end)
        {
            {
                Debug.Assert(id != null, "The ID should not be a null reference.");
                Debug.Assert(graph != null, "The graph should not be a null reference.");

                Debug.Assert(start != null, "The start vertex should not be a null reference.");
                Debug.Assert(graph.ContainsVertex(start), "The start vertex should be part of the graph.");

                Debug.Assert(end != null, "The end vertex should not be a null reference.");
                Debug.Assert(graph.ContainsVertex(end), "The end vertex should be part of the graph.");
            }

            m_Id = id;
            m_Start = start;
            m_End = end;

            m_Graph = graph;
        }

        /// <summary>
        /// Gets the ID of the current schedule.
        /// </summary>
        public ScheduleId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the start vertex for the schedule.
        /// </summary>
        public EditableStartVertex Start
        {
            get
            {
                return m_Start;
            }
        }

        /// <summary>
        /// Gets the end vertex for the schedule.
        /// </summary>
        public EditableEndVertex End
        {
            get
            {
                return m_End;
            }
        }

        /// <summary>
        /// Gets the internal graph for the schedule.
        /// </summary>
        /// <remarks>
        /// Do not make changes to the returned graph because it will change the schedule!
        /// </remarks>
        internal AdjacencyGraph<IEditableScheduleVertex, EditableScheduleEdge> Graph
        {
            get
            {
                return m_Graph;
            }
        }

        /// <summary>
        /// Returns a collection containing all the insert points for the schedule.
        /// </summary>
        /// <returns>The collection that contains all the insert points for the schedule.</returns>
        public IEnumerable<EditableInsertVertex> InsertPoints()
        {
            var result = from vertex in m_Graph.Vertices
                         where vertex is EditableInsertVertex
                         select vertex as EditableInsertVertex;

            return result;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var schedule = obj as IEditableSchedule;
            return Equals(schedule);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return m_Id.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}",
                m_Id);
        }
    }
}
