//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Lokad;

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Stores information about the start time and sequence of a validation sequence.
    /// </summary>
    internal struct ValidationSequence : IEquatable<ValidationSequence>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ValidationSequence first, ValidationSequence second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ValidationSequence first, ValidationSequence second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// The start date for the validation sequence.
        /// </summary>
        private readonly DateTimeOffset m_StartDate;

        /// <summary>
        /// The repeat sequence for the sequence.
        /// </summary>
        private readonly TimePeriod m_Period;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationSequence"/> struct.
        /// </summary>
        /// <param name="period">The period after which the license check should be repeated.</param>
        public ValidationSequence(TimePeriod period)
            : this(period, DateTimeOffset.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationSequence"/> struct.
        /// </summary>
        /// <param name="period">The period after which the license check should be repeated.</param>
        /// <param name="startDate">The date on which the validation sequence starts.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="startDate"/> is smaller equal to <see cref="DateTimeOffset.MinValue"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="startDate"/> is smaller equal to <see cref="DateTimeOffset.MaxValue"/>.
        /// </exception>
        public ValidationSequence(TimePeriod period, DateTimeOffset startDate)
        {
            {
                Enforce.With<ArgumentOutOfRangeException>(!startDate.Equals(DateTimeOffset.MinValue), SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, startDate);
                Enforce.With<ArgumentOutOfRangeException>(!startDate.Equals(DateTimeOffset.MaxValue), SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, startDate);
            }

            m_Period = period;
            m_StartDate = startDate;
        }

        /// <summary>
        /// Gets the sequence for the sequence.
        /// </summary>
        public TimePeriod Period
        {
            get 
            {
                return m_Period;
            }
        }

        /// <summary>
        /// Gets the start date for the sequence.
        /// </summary>
        public DateTimeOffset StartDate
        {
            get 
            {
                return m_StartDate;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="ValidationSequence"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ValidationSequence"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="ValidationSequence"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(ValidationSequence other)
        {
            return m_StartDate.Equals(other.m_StartDate) && m_Period.Equals(other.m_Period);
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
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (!(obj is ValidationSequence))
            {
                return false;
            }

            return Equals((ValidationSequence)obj);
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
                hash = (hash * 23) ^ m_StartDate.GetHashCode();
                hash = (hash * 23) ^ m_Period.GetHashCode();

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
            return string.Format(CultureInfo.InvariantCulture, "Repeat validation starting on: {0} every {1}", m_StartDate, m_Period);
        }
    }
}
