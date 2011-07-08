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

        /// <summary>
        /// An event raised when the endpoint to which the commandset belongs
        /// becomes available or unavailable.
        /// </summary>
        /// <remarks>
        /// Note that changes in availability do not mean that the endpoint has
        /// permanently been terminated (although that may be the case). It merely
        /// means that the endpoint is temporarily not available.
        /// </remarks>
        public event EventHandler<EventArgs> OnAvailabilityChange;

        private void RaiseOnAvailabilityChange()
        {
            var local = OnAvailabilityChange;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event raised when the endpoint to which the command set belongs
        /// becomes invalid.
        /// </summary>
        public event EventHandler<EventArgs> OnTerminated;

        private void RaiseOnTerminated()
        {
            var local = OnTerminated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
