//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Stores information about a machine.
    /// </summary>
    [Serializable]
    public sealed class Machine : IEquatable<Machine>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Machine first, Machine second)
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
        public static bool operator !=(Machine first, Machine second)
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
        /// The collection of baselines for the current machine.
        /// </summary>
        private readonly Dictionary<BaseLineId, BaseLineResult> m_Baselines
            = new Dictionary<BaseLineId, BaseLineResult>();

        /// <summary>
        /// Defines the location of the machine through the 
        /// network address.
        /// </summary>
        private readonly NetworkIdentifier m_Location;

        /// <summary>
        /// Defines the hardware specification for the given machine.
        /// </summary>
        private readonly HardwareSpecification m_Specification;

        /// <summary>
        /// Initializes a new instance of the <see cref="Machine"/> class with the given
        /// baselines for the local machine.
        /// </summary>
        /// <param name="baselines">The baselines for the local machines.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="baselines"/> is <see langword="null" />.
        /// </exception>
        public Machine(IDictionary<BaseLineId, BaseLineResult> baselines)
            : this(NetworkIdentifier.ForLocalMachine(), HardwareSpecification.ForLocalMachine(), baselines)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Machine"/> class with the given
        /// baselines for the given machine.
        /// </summary>
        /// <param name="location">The network location for the given machine.</param>
        /// <param name="specification">The hardware specification for the given machine.</param>
        /// <param name="baselines">The baselines for the given machines.</param>
        public Machine(
            NetworkIdentifier location, 
            HardwareSpecification specification, 
            IDictionary<BaseLineId, BaseLineResult> baselines)
        {
            {
                Enforce.Argument(() => location);
                Enforce.Argument(() => specification);
                Enforce.Argument(() => baselines);
            }

            m_Location = location;
            m_Specification = specification;
            m_Baselines.AddRange(baselines);
        }

        /// <summary>
        /// Gets a value indicating the network location of the given machine.
        /// </summary>
        public NetworkIdentifier Location
        {
            get
            {
                return m_Location;
            }
        }

        /// <summary>
        /// Gets a value indicating what the hardware specifications are for the
        /// given machine.
        /// </summary>
        public HardwareSpecification Specification
        {
            get
            {
                return m_Specification;
            }
        }

        /// <summary>
        /// Returns the collection of baseline results for the given machine.
        /// </summary>
        /// <returns>The collection of baseline results for the given machine.</returns>
        public IEnumerable<BaseLineResult> BaseLines()
        {
            return m_Baselines.Values;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Machine"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Machine"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="Machine"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(Machine other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && Location.Equals(other.Location);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            var id = obj as Machine;
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
            // As obtained from the Jon Skeet answer to:  http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ Location.GetHashCode();

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
            return Location.ToString();
        }
    }
}
