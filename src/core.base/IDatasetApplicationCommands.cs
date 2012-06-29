﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Apollo.Core.Base.Communication;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Provides commands that have effect on a dataset application.
    /// </summary>
    [InternalCommand]
    public interface IDatasetApplicationCommands : ICommandSet
    {
        /// <summary>
        /// Loads the dataset into the dataset application.
        /// </summary>
        /// <param name="ownerId">The ID of the endpoint which requested the load of the dataset.</param>
        /// <param name="token">The token that indicates which file should be uploaded.</param>
        /// <returns>A task that will finish once the dataset is loaded.</returns>
        Task Load(EndpointId ownerId, UploadToken token);

        /// <summary>
        /// Closes the remote dataset application.
        /// </summary>
        /// <returns>A task that will finish once the application is closed.</returns>
        Task Close();

        /// <summary>
        /// Indicates if the current dataset has changed since the last time it
        /// has been saved.
        /// </summary>
        /// <returns>
        ///     A task that will return a value indicating if the dataset has been
        ///     changed since the last save.
        /// </returns>
        Task<bool> HasChanged();

        /// <summary>
        /// Requests that the current dataset is saved and the saved copy is transfered
        /// back to the host machine.
        /// </summary>
        /// <returns>A task which will complete once the transfer is complete.</returns>
        Task Save();

        /// <summary>
        /// Indicates what the lock state of the the dataset is.
        /// </summary>
        /// <returns>A task that will return the information about the lock state of the dataset.</returns>
        Task<DatasetLockInformation> LockedState();
    }
}
