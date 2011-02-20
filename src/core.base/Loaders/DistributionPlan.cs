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
    /// Defines the plan for distributing a dataset across one or more machines.
    /// </summary>
    public sealed class DistributionPlan
    {
        /// <summary>
        /// Gets a value indicating the ID number of the dataset for which
        /// this distribution plan is valid.
        /// </summary>
        public DatasetId DistributionFor
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a collection of objects describing the different machines
        /// to which the given dataset will be distributed.
        /// </summary>
        /// <returns>
        /// The collection of machines to which the dataset will be distributed.
        /// </returns>
        public IEnumerable<NetworkIdentifier> MachinesToDistributeTo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating the estimated time necessary for the transfer of
        /// the given dataset to the proposed machines.
        /// </summary>
        public TimeSpan EstimatedTransferTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the estimated time necessary for the loadng of
        /// the given dataset onto the proposed machines.
        /// </summary>
        public TimeSpan EstimatedLoadingTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the estimated time necessary for the running of
        /// the given dataset on the proposed machines.
        /// </summary>
        public TimeSpan EstimatedRunningTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the estimated time necessary for the unloading of
        /// the given dataset from the proposed machines.
        /// </summary>
        public TimeSpan EstimatedUnloadingTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the estimated time necessary for the return transfer of
        /// the given dataset from the proposed machines to the current machine.
        /// </summary>
        public TimeSpan EstimatedReturnTransferTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the earliest time at which the transfer and running process
        /// could start.
        /// </summary>
        public DateTimeOffset EarliestStartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the earliest time at which the resulting dataset could
        /// be completely returned taking into account the earliest starting time and the
        /// estimated transfer, loading, running, unloading and return transfer times.
        /// </summary>
        public DateTimeOffset EarliestEstimatedFinishTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a collection of objects describing the datasets that have 
        /// been loaded through the execution of the current distribution plan.
        /// </summary>
        /// <returns>
        /// The collection of objects describing the activated datasets.
        /// </returns>
        public IObservable<DatasetOnlineInformation> Accept()
        {
            throw new NotImplementedException();
        }
    }
}
