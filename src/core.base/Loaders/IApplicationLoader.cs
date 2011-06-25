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
        /// Loads the dataset into an external application and returns the requested communication
        /// information.
        /// </summary>
        /// <param name="ownerConnection">
        ///     The channel connection information for the owner.
        /// </param>
        /// <param name="ownerToken">
        ///     The conversation token that the application which requested the loading of the dataset 
        ///     has provided.
        /// </param>
        void LoadDataset(ChannelConnectionInformation ownerConnection, ConversationToken ownerToken);
    }
}
