//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Stores the information that describes an activation request for a dataset.
    /// </summary>
    public sealed class DatasetActivationRequest
    {
        /// <summary>
        /// Gets or sets the dataset that should be activated.
        /// </summary>
        public IDatasetOfflineInformation DatasetToActivate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the preferred distribution locations.
        /// </summary>
        public DistributionLocations PreferredLocations
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating how much load the dataset is expected
        /// to place on a given machine.
        /// </summary>
        public ExpectedDatasetLoad ExpectedLoadPerMachine
        {
            get;
            set;
        }
    }
}
