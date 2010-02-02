//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Apollo.Core.Messaging;

namespace Apollo.Core.UserInterfaces
{
    /// <content>
    /// Defines the methods used for working with messages.
    /// </content>
    internal sealed partial class UserInterfaceService
    {
        /// <summary>
        /// Stores the different message types and their connected actions.
        /// </summary>
        /// <param name="processor">The processor.</param>
        protected override void StoreMessageActions(IHelpMessageProcessing processor)
        {
            // Define the response to a shutdown request
            processor.RegisterAction(
                typeof(ServiceShutdownCapabilityRequestMessage),
                message =>
                {
                    var request = message.Body as ServiceShutdownCapabilityRequestMessage;
                    Debug.Assert(request != null, "Message type mapping failed for ServiceShutdownCapabilityRequestMessage");

                    HandleShutdownCapabilityRequest(message.Header.Sender, message.Header.Id);
                });
        }

        private void HandleShutdownCapabilityRequest(DnsName originalSender, MessageId id)
        {
            if (!IsFullyFunctional)
            {
                return;
            }

            // @todo: Check with the UI if we can shutdown. This should only be a UI value, not the system value.
            // For now just send a message saying that we can shutdown.
            SendMessage(originalSender, new ServiceShutdownCapabilityResponseMessage(true), id);
        }
    }
}
