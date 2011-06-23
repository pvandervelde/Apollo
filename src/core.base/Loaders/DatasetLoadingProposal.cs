﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Communication;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Stores information about a proposal to load a given dataset onto a given machine.
    /// </summary>
    [Serializable]
    public sealed class DatasetLoadingProposal
    {
        /// <summary>
        /// Gets or sets the ID number of the endpoint that made the proposal.
        /// </summary>
        /// <remarks>
        /// Note that this is not the ID of the endpoint that will eventually
        /// load the dataset. It is the ID of the endpoint which will handle the
        /// start-up of the dataset application that loads the dataset.
        /// </remarks>
        public EndpointId Endpoint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current endpoint is available at all.
        /// </summary>
        public bool IsAvailable
        {
            get;
            set;
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
        /// Gets or sets the estimated time necessary to load the dataset from the disk
        /// into the dataset application.
        /// </summary>
        public TimeSpan LoadingTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the estimated percentage of the available diskspace that the
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
