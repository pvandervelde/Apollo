//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Reports the registration of new commands on remote endpoints.
    /// </summary>
    internal sealed class ManualCommandRegistrationReporter : IReportNewCommands, IAceptExternalCommandInformation
    {
        /// <summary>
        /// An event raised when a new remote command is registered.
        /// </summary>
        public event EventHandler<CommandInformationEventArgs> OnNewCommandRegistered;

        private void RaiseOnNewCommandRegistered(EndpointId endpoint, ISerializedType command)
        {
            var local = OnNewCommandRegistered;
            if (local != null)
            {
                local(this, new CommandInformationEventArgs(endpoint, command));
            }
        }

        /// <summary>
        /// Stores or forwards information about a command that has recently been
        /// registered at a remote endpoint.
        /// </summary>
        /// <param name="id">The ID of the endpoint.</param>
        /// <param name="command">The recently registered command.</param>
        public void RecentlyRegisteredCommand(EndpointId id, ISerializedType command)
        {
            RaiseOnNewCommandRegistered(id, command);
        }
    }
}
