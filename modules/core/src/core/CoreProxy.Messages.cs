//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Threading.Tasks;
using Apollo.Core.Messaging;
using Apollo.Core.Utils;

namespace Apollo.Core
{
    /// <content>
    /// Defines the methods necessary for the handling of messages.
    /// </content>
    internal sealed partial class CoreProxy
    {
        /// <summary>
        /// Stores the different message types and their connected actions.
        /// </summary>
        /// <param name="processor">The processor.</param>
        protected override void StoreMessageActions(IHelpMessageProcessing processor)
        {
            // Define the response to a shutdown request
            processor.RegisterAction(
                typeof(ShutdownRequestMessage),
                message =>
                    {
                        var request = message.Body as ShutdownRequestMessage;
                        Debug.Assert(request != null, "Message type mapping failed for ShutdownRequestMessage");

                        HandleShutdownRequest(message.Header.Sender, message.Header.Id, request);
                    });

            processor.RegisterAction(
               typeof(ApplicationShutdownCapabilityRequestMessage),
               message =>
                   {
                       var request = message.Body as ApplicationShutdownCapabilityRequestMessage;
                       Debug.Assert(request != null, "Message type mapping failed for ApplicationShutdownCapabilityRequestMessage");

                       HandleShutdownCapabilityRequest(message.Header.Sender, message.Header.Id);
                   });

            // Create new AppDomain request
            // Request for system information --> send an entire block in one go
        }

        /// <summary>
        /// Handles the request to shutdown the application.
        /// </summary>
        /// <param name="originalSender">The <see cref="DnsName"/> of the requesting service.</param>
        /// <param name="id">The ID number of the requesting message.</param>
        /// <param name="request">The request information.</param>
        private void HandleShutdownRequest(DnsName originalSender, MessageId id, ShutdownRequestMessage request)
        {
            if (!IsFullyFunctional)
            {
                return;
            }

            bool canShutdown = request.IsShutdownForced ? true : m_Owner.CanShutdown();
            if (request.IsResponseRequired)
            {
                SendMessage(originalSender, new ShutdownResponseMessage(canShutdown), id);
            }

            if (canShutdown)
            {
                // Elevate to full trust. This is required because System.Threading.Parallel.Invoke() is in a
                // full-trust assembly. In order to invoke it we'll need to run it in that environment (full trust).
                // @Todo: Does this elevation drop off the stack before or after we finish with the thread, i.e. is the thread running full-trust?
                SecurityHelpers.Elevate(
                    new PermissionSet(
                        PermissionState.Unrestricted),
                        () => Parallel.Invoke(() => m_Owner.Shutdown()));
            }
        }

        /// <summary>
        /// Handles the shutdown capability request for the application.
        /// </summary>
        /// <param name="originalSender">The <see cref="DnsName"/> of the requesting service.</param>
        /// <param name="id">The ID number of the requesting message.</param>
        private void HandleShutdownCapabilityRequest(DnsName originalSender, MessageId id)
        {
            if (!IsFullyFunctional)
            {
                return;
            }

            bool canShutdown = m_Owner.CanShutdown();
            SendMessage(originalSender, new ApplicationShutdownCapabilityResponseMessage(canShutdown), id);
        }
    }
}
