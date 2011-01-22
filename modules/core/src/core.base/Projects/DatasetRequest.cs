//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Projects
{
    /// <summary>
    /// Stores the information that describes a loading request for a dataset.
    /// </summary>
    public sealed class DatasetRequest
    {
        /// <summary>
        /// Gets a value indicating which dataset should be loaded.
        /// </summary>
        public DatasetOfflineInformation DatasetToLoad
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating which dataset is the parent 
        /// of the dataset that should be loaded.
        /// </summary>
        public DatasetId Parent
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the parent dataset needs 
        /// to be able to communicate with the dataset that should be
        /// loaded.
        /// </summary>
        public bool ShouldProvideParentWithHubConnection
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating how much load the dataset is expected
        /// to place on a given machine.
        /// </summary>
        public ExpectedDatasetLoad ExpectedLoadPerMachine
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating what the preferred location is for the
        /// dataset.
        /// </summary>
        public LoadingLocation PreferredLoadingLocation
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating over how many machines the dataset
        /// can be distributed.
        /// </summary>
        public MachineDistributionRange PreferredDistributionRange
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
