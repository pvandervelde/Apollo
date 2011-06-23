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

        Task<ChannelConnectionInformation> Load(DatasetId dataset);
    }
}
