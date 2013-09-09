//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Nuclei.Communication;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines the methods for an <see cref="ICommandSet"/> that provides commands for
    /// dataset loading.
    /// </summary>
    internal interface IDatasetActivationCommands : ICommandSet
    {
        /// <summary>
        /// Generates a loading proposal for the given dataset.
        /// </summary>
        /// <param name="expectedLoad">The expected load the dataset will place on the machine.</param>
        /// <returns>
        ///     A proposal for activation of the given dataset on the current machine.
        /// </returns>
        Task<DatasetActivationProposal> ProposeFor(ExpectedDatasetLoad expectedLoad);

        /// <summary>
        /// Activates the dataset.
        /// </summary>
        /// <param name="endpointId">The endpoint ID for the owner.</param>
        /// <param name="channelType">The type of channel on which the owner can be contacted.</param>
        /// <param name="address">The address on which the owner can be contacted.</param>
        /// <param name="dataset">The ID of the dataset that should be activated.</param>
        /// <returns>The connection information for the newly created application.</returns>
        Task<EndpointId> Activate(EndpointId endpointId, ChannelType channelType, Uri address, DatasetId dataset);
    }
}
