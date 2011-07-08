//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the interface for objects that store or forward information about
    /// commands that were recently registered on a remote endpoint.
    /// </summary>
    internal interface IAceptExternalCommandInformation
    {
        /// <summary>
        /// Stores or forwards information about a command that has recently been
        /// registered at a remote endpoint.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        /// <param name="command">The recently registered command.</param>
        void RecentlyRegisteredCommand(EndpointId id, ISerializedType command);
    }
}
