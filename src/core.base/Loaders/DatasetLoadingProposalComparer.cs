//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines a method for comparing <see cref="DatasetLoadingProposal"/> instances based
    /// on the suitability of the proposal.
    /// </summary>
    internal sealed class DatasetLoadingProposalComparer : IComparer<DatasetLoadingProposal>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition 
        /// Less than zero x is less than y.
        /// Zero x equals y.
        /// Greater than zero x is greater than y.
        /// </returns>
        public int Compare(DatasetLoadingProposal x, DatasetLoadingProposal y)
        {
            if ((x == null) && (y == null))
            {
                throw new ArgumentNullException("x");
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            var xRating = x.Rate();
            var yRating = y.Rate();
            return xRating.CompareTo(yRating);
        }
    }
}
