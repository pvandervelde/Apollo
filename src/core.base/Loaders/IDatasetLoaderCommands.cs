//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Apollo.Core.Base.Communication;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the methods for an <see cref="ICommandSet"/> that provides commands for
    /// dataset loading.
    /// </summary>
    internal interface IDatasetLoaderCommands : ICommandSet
    {
        /// <summary>
        /// Generates a loading proposal for the given dataset.
        /// </summary>
        /// <param name="expectedLoad">The expected load the dataset will place on the machine.</param>
        /// <returns>
        ///     A proposal for loading the given dataset on the current machine.
        /// </returns>
        Task<DatasetLoadingProposal> ProposeFor(ExpectedDatasetLoad expectedLoad);

        /// <summary>
        /// Loads the dataset into an external application and returns when the dataset is completely loaded.
        /// </summary>
        /// <param name="ownerConnection">
        ///     The channel connection information for the owner.
        /// </param>
        /// <param name="dataset">The ID of the dataset that should be loaded.</param>
        /// <returns>The connection information for the newly created application.</returns>
        Task<EndpointId> Load(ChannelConnectionInformation ownerConnection, DatasetId dataset);
    }
}
