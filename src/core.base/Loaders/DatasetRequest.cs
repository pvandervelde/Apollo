//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Stores the information that describes a loading request for a dataset.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class DatasetRequest
    {
        /// <summary>
        /// Gets or sets the dataset that should be loaded.
        /// </summary>
        public IDatasetOfflineInformation DatasetToLoad
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the preferred loading locations.
        /// </summary>
        public LoadingLocations PreferredLocations
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
