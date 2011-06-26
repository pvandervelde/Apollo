//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Defines commands that allow dataset applications to get data from the hosting application.
    /// </summary>
    public sealed class HostCommands : IHostCommands
    {
        /// <summary>
        /// Requests the transfer of a dataset with a specific <see cref="DatasetId"/> to 
        /// the machine where the request came from.
        /// </summary>
        /// <param name="id">The ID of the dataset which should be transferred.</param>
        /// <returns>A task which will complete once the transfer is complete.</returns>
        public Task TransferDataset(DatasetId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Requests URI to the plugin repository.
        /// </summary>
        /// <returns>A task which returns the requested URI once it completes.</returns>
        public Task<Uri> PluginRepository()
        {
            throw new NotImplementedException();
        }
    }
}
