//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using Nuclei.Communication;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Generates dataset loading proposals for the local machine.
    /// </summary>
    internal sealed class DistributionCalculator : ICalculateDistributionParameters
    {
        /// <summary>
        /// The ID number of the local endpoint.
        /// </summary>
        private readonly EndpointId m_LocalEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionCalculator"/> class.
        /// </summary>
        public DistributionCalculator()
        {
            m_LocalEndpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
        }

        /// <summary>
        /// Returns a dataset loading proposal for the given expected load.
        /// </summary>
        /// <param name="expectedLoad">The load the dataset is expected to put on the machine.</param>
        /// <returns>
        ///     A proposal that indicates if the machine can load the dataset and what the
        ///     expected loading performance will be like.
        /// </returns>
        public DatasetActivationProposal ProposeForLocalMachine(ExpectedDatasetLoad expectedLoad)
        {
            // Grab the information about the local machine. Do this here so that we get
            // the most up to date information we can.
            var machine = new Machine();

            // RAM memory
            var requiredMemory = expectedLoad.InMemorySizeInBytes * expectedLoad.RelativeMemoryExpansionWhileRunning;
            var maximumMemoryPercentage = requiredMemory / (machine.Specification.PerProcessMemoryInKilobytes * 1024) * 100;

            // Virtual memory
            var maximumVirtualMemoryPercentage = requiredMemory / (machine.Specification.TotalVirtualMemoryInKilobytes * 1024) * 100;

            // Disk
            var requiredDisk = expectedLoad.OnDiskSizeInBytes * expectedLoad.RelativeOnDiskExpansionAfterRunning;
            var maximumFreeDisk = (from disk in machine.Specification.Disks()
                                   select disk.AvailableSpaceInBytes).Max();
            var maximumDiskPercentage = requiredDisk / maximumFreeDisk * 100;

            // Loading time
            // Currently can't get this because we don't know how fast the disk is. And even if we know that
            // then we still don't know how fast we can unpack the information ... so we just fake it.
            var loadTime = new TimeSpan(0, 1, 0);

            // Transfer time
            // And we can't get this one either because we don't know the speed of the connection we're going to use ...
            var transferTime = new TimeSpan(0, 1, 0);

            return new DatasetActivationProposal 
                {
                    Endpoint = m_LocalEndpoint,
                    IsAvailable = false,
                    ActivationTime = loadTime,
                    TransferTime = transferTime,
                    PercentageOfAvailableDisk = (int)Math.Ceiling(maximumDiskPercentage),
                    PercentageOfMaximumMemory = (int)Math.Ceiling(maximumVirtualMemoryPercentage),
                    PercentageOfPhysicalMemory = (int)Math.Ceiling(maximumMemoryPercentage),
                };
        }
    }
}
