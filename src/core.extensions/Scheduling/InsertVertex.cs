//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
    public sealed class InsertVertex : IScheduleVertex
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(InsertVertex first, InsertVertex second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(InsertVertex first, InsertVertex second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// The number of times the current vertex can be replaced with another vertex.
        /// </summary>
        private readonly int m_RemainingInserts;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertVertex"/> class with
        /// an unlimited number of inserts.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        public InsertVertex(int index)
        {
            Index = index;
            m_RemainingInserts = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertVertex"/> class.
        /// </summary>
        /// <param name="index">The index of the vertex in the graph.</param>
        /// <param name="maximumNumberOfInserts">The maximum number of times this node can be replaced with another node.</param>
        /// <exception cref="CannotCreateInsertVertexWithLessThanOneInsertException">
        /// Thrown when <paramref name="maximumNumberOfInserts"/> is not a positive integer larger than zero.
        /// </exception>
        public InsertVertex(int index, int maximumNumberOfInserts)
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

        /// <summary>
        /// Determines whether the specified <see cref="IScheduleVertex"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="IScheduleVertex"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="IScheduleVertex"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(IScheduleVertex other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && Index == other.Index 
                && GetType().Equals(other.GetType());
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

            var id = obj as IScheduleVertex;
            return Equals(id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ Index.GetHashCode();
                hash = (hash * 23) ^ GetType().GetHashCode();

                return hash;
            }
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
                "{0}: {1}",
                GetType().Name,
                Index);
        }
    }
}
