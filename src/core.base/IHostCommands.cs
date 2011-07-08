//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Apollo.Core.Base.Communication;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines a set of commands that provide functionality used by the dataset application
    /// to get data from the hosting application.
    /// </summary>
    [InternalCommand]
    public interface IHostCommands : ICommandSet
    {
        /// <summary>
        /// Requests the transfer of a dataset with a specific <see cref="DatasetId"/> to 
        /// the machine where the request came from.
        /// </summary>
        /// <param name="id">The ID of the dataset which should be transferred.</param>
        /// <returns>A task which will complete once the transfer is complete.</returns>
        Task TransferDataset(DatasetId id);

        /// <summary>
        /// Requests URI to the plugin repository.
        /// </summary>
        /// <returns>A task which returns the requested URI once it completes.</returns>
        Task<Uri> PluginRepository();
    }
}
