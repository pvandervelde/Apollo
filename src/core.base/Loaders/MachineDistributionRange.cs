//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Core.Base.Properties;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the number of machines over which a dataset can be distributed.
    /// </summary>
    public sealed class MachineDistributionRange : IEquatable<MachineDistributionRange>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MachineDistributionRange first, MachineDistributionRange second)
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
        public static bool operator !=(MachineDistributionRange first, MachineDistributionRange second)
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
        /// Initializes a new instance of the <see cref="MachineDistributionRange"/> class for
        /// a distribution to exactly 1 machine.
        /// </summary>
        public MachineDistributionRange()
            : this(1, 1)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineDistributionRange"/> class for
        /// a distribution between 1 and <paramref name="maximum"/> number of machines.
        /// </summary>
        /// <param name="maximum">The maximum number of machines on which the dataset can be distributed.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <paramref name="maximum"/> is smaller than 1.
        /// </exception>
        public MachineDistributionRange(int maximum)
            : this(1, maximum)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineDistributionRange"/> class for
        /// a distribution between <paramref name="minimum"/> and <paramref name="maximum"/> number of machines.
        /// </summary>
        /// <param name="minimum">The mimimum number of machines on which the dataset should be distributed.</param>
        /// <param name="maximum">The maximum number of machines on which the dataset can be distributed.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <paramref name="maximum"/> is smaller than 1.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <paramref name="maximum"/> is smaller than <paramref name="minimum"/>.
        /// </exception>
        public MachineDistributionRange(int minimum, int maximum)
        {
            {
                Enforce.With<ArgumentOutOfRangeException>(minimum > 0, Resources.Exceptions_Messages_MimimumDistributionRangeMustBeLargerThanOne_WithValue, minimum);
                Enforce.With<ArgumentOutOfRangeException>(maximum >= minimum, Resources.Exceptions_Messages_MaximumDistributionRangeMustBeLargerThanMinimum_WithValues, minimum, maximum);
            }

            Minimum = minimum;
            Maximum = maximum;
        }

        /// <summary>
        /// Gets a vale indicating the minimum number of machines on which the
        /// dataset should be distributed.
        /// </summary>
        public int Minimum
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the maximum number of machines on which the 
        /// dataset can be distributed.
        /// </summary>
        public int Maximum
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the specified <see cref="MachineDistributionRange"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="MachineDistributionRange"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="MachineDistributionRange"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(MachineDistributionRange other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Minimum.Equals(other.Minimum) && Maximum.Equals(other.Maximum);
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var range = obj as MachineDistributionRange;
            return Equals(range);
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
                hash = (hash * 23) ^ Minimum.GetHashCode();
                hash = (hash * 23) ^ Maximum.GetHashCode();

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
            return string.Format(CultureInfo.InvariantCulture, "Distribute within range [{0}, {1}]", Minimum, Maximum);
        }
    }
}
