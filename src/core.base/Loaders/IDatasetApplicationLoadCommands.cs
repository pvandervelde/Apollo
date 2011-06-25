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
    /// Defines the commands that are used during the loading of a dataset.
    /// </summary>
    public interface IDatasetApplicationLoadCommands : ICommandSet
    {
        /// <summary>
        /// Provides the dataset application with the information necessary to connect to the application
        /// that requested the dataset to be loaded.
        /// </summary>
        /// <param name="ownerChannel">The connection information for the owners channel.</param>
        /// <param name="token">The conversation token that can be used to acquire the necessary information from the owner.</param>
        /// <returns>
        /// A task that indicates if the connection to the owner has been established.
        /// </returns>
        Task ConnectToOwner(ChannelConnectionInformation ownerChannel, ConversationToken token);
    }
}
