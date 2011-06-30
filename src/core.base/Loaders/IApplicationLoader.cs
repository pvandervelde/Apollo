//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Core.Base.Communication;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the interface for objects that load datasets into an external application
    /// and provide the required information to the user to connect to that application
    /// via the communication system.
    /// </summary>
    internal interface IApplicationLoader
    {
        /// <summary>
        /// Loads the dataset into an external application and returns when the dataset application has started.
        /// </summary>
        /// <param name="ownerConnection">
        ///     The channel connection information for the owner.
        /// </param>
        /// <returns>The ID number of the newly created endpoint.</returns>
        EndpointId LoadDataset(ChannelConnectionInformation ownerConnection);
    }
}
