//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Apollo.Core.Messaging;

namespace Apollo.Core.Projects
{
    /// <content>
    /// Defines the methods used for working with messages.
    /// </content>
    internal sealed partial class ProjectService
    {
        /// <summary>
        /// Stores the different message types and their connected actions.
        /// </summary>
        /// <param name="processor">The processor.</param>
        protected override void StoreMessageActions(IHelpMessageProcessing processor)
        {
            // Define the response to a shutdown capability request
            processor.RegisterAction(
                typeof(ServiceShutdownCapabilityRequestMessage),
                message =>
                {
                    var request = message.Body as ServiceShutdownCapabilityRequestMessage;
                    Debug.Assert(request != null, "Message type mapping failed for ServiceShutdownCapabilityRequestMessage");

                    HandleShutdownCapabilityRequest(message.Header.Sender, message.Header.Id);
                });

            // Which other actions do we need?
            // - Project related:
            //   - LoadProject
            //   - SaveProject
            //   - CloseProject
            // - Dataset related:
            //   - Get information
            //   - 
        }

        private void HandleShutdownCapabilityRequest(DnsName originalSender, MessageId id)
        {
            Debug.Assert(
                IsFullyFunctional,
                string.Format("The service tried to perform an action but wasn't in the correct startup state. The actual state was: {0}", GetStartupState()));

            // @todo: Check with the project if we can shutdown. This should only be a project value, not the system value.
            // For now just send a message saying that we can shutdown.
            SendMessage(originalSender, new ServiceShutdownCapabilityResponseMessage(true), id);
        }
    }
}
