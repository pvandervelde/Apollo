//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Stores the information that describes a loading request for a dataset.
    /// </summary>
    /// <design>
    /// <para>
    /// For the moment we only allow loading multiple dataset in a request if they
    /// hav the same machine demands and distribution range. This should cover most
    /// cases because for parameter studies all the datasets should be equivalent and
    /// for domain decomposition the goal is to make the machine loads equivalent. Hence
    /// this approach should work.
    /// </para>
    /// <para>
    /// Note that in the domain decomposition case we might also need the ability to reserve
    /// nodes between when we get a proposal and when we apply it. Obviously this can get 
    /// troublesome because of the untrustworthiness of the network etc.
    /// </para>
    /// </design>
    [ExcludeFromCodeCoverage()]
    public sealed class DatasetRequest
    {
        /// <summary>
        /// The collection that holds the information about all the datasets that should be loaded.
        /// </summary>
        private readonly List<DatasetOfflineInformation> m_DatasetsToLoad = new List<DatasetOfflineInformation>();

        /// <summary>
        /// Gets the collection that holds the information about all the datasets should be loaded.
        /// </summary>
        public IList<DatasetOfflineInformation> DatasetsToLoad
        {
            get
            {
                return m_DatasetsToLoad;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating which dataset is the parent 
        /// of the dataset that should be loaded.
        /// </summary>
        public DatasetId Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the parent dataset needs 
        /// to be able to communicate with the dataset that should be
        /// loaded.
        /// </summary>
        public bool ShouldProvideParentWithHubConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is a need for
        /// communication between the datasets.
        /// </summary>
        public bool IsIntraDatasetCommunicationRequired
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

        /// <summary>
        /// Gets or sets a value indicating what the preferred location is for the
        /// dataset.
        /// </summary>
        public LoadingLocation PreferredLoadingLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating over how many machines the dataset
        /// can be distributed.
        /// </summary>
        public MachineDistributionRange PreferredDistributionRange
        {
            get;
            set;
        }
    }
}
