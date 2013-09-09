//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines the interface for objects that activates datasets into an external application
    /// and provide the required information to the user to connect to that application
    /// via the communication system.
    /// </summary>
    internal interface IDatasetActivator
    {
        /// <summary>
        /// Activates the dataset into an external application and returns when the dataset application has started.
        /// </summary>
        /// <param name="endpointId">The endpoint ID for the owner.</param>
        /// <param name="channelType">The type of channel on which the dataset should be contacted.</param>
        /// <param name="address">The channel address for the owner.</param>
        /// <returns>The ID number of the newly created endpoint.</returns>
        EndpointId ActivateDataset(EndpointId endpointId, ChannelType channelType, Uri address);
    }
}
