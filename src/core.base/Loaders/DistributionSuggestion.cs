//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Stores a distribution plan and its rating.
    /// </summary>
    public sealed class DistributionSuggestion : IComparable<DistributionSuggestion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionSuggestion"/> class.
        /// </summary>
        /// <param name="plan">The suggested plan.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="plan"/> is <see langword="null" />.
        /// </exception>
        public DistributionSuggestion(DistributionPlan plan)
        {
            {
                Enforce.Argument(() => plan);
            }

            Rating = plan.Proposal.Rate();
            Plan = plan;
        }

        /// <summary>
        /// Gets the rating for the suggested plan. Lower numbers indicate a 'better'
        /// proposal.
        /// </summary>
        public double Rating
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the suggested distribution plan.
        /// </summary>
        public DistributionPlan Plan
        {
            get;
            private set;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that
        /// indicates whether the current instance precedes, follows, or occurs in the same position in the
        /// sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="other"/>.
        /// Zero
        /// This instance is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(DistributionSuggestion other)
        {
            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return ReferenceEquals(other, null) ? 1 : Rating.CompareTo(other.Rating);
        }
    }
}
