//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines extension methods for the <see cref="DatasetActivationProposal"/> class.
    /// </summary>
    public static class DatasetActivationProposalExtensions
    {
        /// <summary>
        /// Rates a proposal based on the amount of time necessary to transfer and load the
        /// dataset to the machine that created the proposal.
        /// </summary>
        /// <param name="proposal">The proposal.</param>
        /// <returns>
        ///     A positive number that describes the 'usefulness' of the proposal. Lower numbers are better.
        /// </returns>
        public static double Rate(this DatasetActivationProposal proposal)
        {
            if (proposal == null)
            {
                return double.NaN;
            }

            // If the dataset takes up more disk space than the slave has available then we 
            // won't distribute to that slave, otherwise the amount of disk space is irrelevant.
            double multiplier = (proposal.PercentageOfAvailableDisk > 100) ? double.PositiveInfinity : 1.0;

            // If the dataset takes up more than 80% of the maximum memory then 
            // we won't distribute to that slave, otherwise the maximum memory is irrelevant
            // The 80% number is based on the idea that the OS and services will need some memory,
            // other than that is just a random high-ish number.
            multiplier *= (proposal.PercentageOfMaximumMemory >= 80) ? double.PositiveInfinity : 1.0;

            // If the datset takes up more space than the available memory then we get worse
            // performance but it should still be possible to load it. We have a small preference
            // for taking up more of the memory so that we favor machines that are better matched
            // to the current dataset.
            multiplier *= MemoryMultiplier(proposal.PercentageOfPhysicalMemory);

            // Turn the time into some floating point number, getting bigger as time gets bigger
            // but not linearly so that we can still handle large time spans.
            var timeValue = TimeToNumber(proposal.TransferTime + proposal.ActivationTime);
            return timeValue * multiplier;
        }

        private static double MemoryMultiplier(int value)
        {
            // Normal distribution function, inverted and centered
            // at 80% memory usage. This penalizes using a machine that has 
            // large amounts of free memory while running the dataset (that is good
            // because we could use that machine for a bigger dataset later on) and it
            // also penalizes machines which need more than 100% memory (which is good
            // because swapping is evil).
            return Math.Pow(Math.E, Math.Pow((value - 80) / 100, 2) / (2 * 0.4 * 0.4));
        }

        private static double TimeToNumber(TimeSpan time)
        {
            return Math.Log(time.Ticks);
        }
    }
}
