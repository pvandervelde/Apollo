//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Activation;

namespace Apollo.Core.Scripting.Projects
{
    /// <summary>
    /// Stores information about a given distribution suggestion.
    /// </summary>
    [Serializable]
    public sealed class DistributionSuggestionProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionSuggestionProxy"/> class.
        /// </summary>
        /// <param name="suggestion">The suggestion for which the current object is a proxy.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="suggestion"/> is <see langword="null" />.
        /// </exception>
        public DistributionSuggestionProxy(DistributionSuggestion suggestion)
        {
            {
                Lokad.Enforce.Argument(() => suggestion);
            }

            Rating = suggestion.Rating;
            MachineToDistributeTo = suggestion.Plan.MachineToDistributeTo;
            TransferTime = suggestion.Plan.Proposal.TransferTime;
            ActivationTime = suggestion.Plan.Proposal.ActivationTime;
            PercentageOfAvailableDisk = suggestion.Plan.Proposal.PercentageOfAvailableDisk;
            PercentageOfMaximumMemory = suggestion.Plan.Proposal.PercentageOfMaximumMemory;
            PercentageOfPhysicalMemory = suggestion.Plan.Proposal.PercentageOfPhysicalMemory;
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
        /// Gets the identifier of the machine onto which the dataset would be loaded
        /// if this plan is accepted.
        /// </summary>
        public NetworkIdentifier MachineToDistributeTo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the estimated time necessary to transfer the dataset to the machine.
        /// </summary>
        public TimeSpan TransferTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the estimated time necessary to activate the dataset.
        /// </summary>
        public TimeSpan ActivationTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the estimated percentage of the available disk space that the
        /// dataset will take up.
        /// </summary>
        /// <remarks>
        /// This value can go over 100% if the dataset would take up more
        /// space than is available on the disk.
        /// </remarks>
        public int PercentageOfAvailableDisk
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the estimated percentage of the physical memory that the
        /// dataset will take up.
        /// </summary>
        /// <remarks>
        /// This value can go over 100% if the dataset would take up more
        /// space than is available.
        /// </remarks>
        public int PercentageOfPhysicalMemory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the estimated percentage of the maximum (virtual) memory
        /// that the dataset will take up.
        /// </summary>
        /// <remarks>
        /// This value can go over 100% if the dataset would take up more
        /// space than is available.
        /// </remarks>
        public int PercentageOfMaximumMemory
        {
            get;
            set;
        }
    }
}
