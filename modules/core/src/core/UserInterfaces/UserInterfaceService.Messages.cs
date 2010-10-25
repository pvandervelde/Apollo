//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.Properties;

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
            // Define the response to a shutdown capability request
            processor.RegisterAction(
                typeof(ServiceShutdownCapabilityRequestMessage),
                message =>
                {
                    var request = message.Body as ServiceShutdownCapabilityRequestMessage;
                    Debug.Assert(request != null, "Message type mapping failed for ServiceShutdownCapabilityRequestMessage");

                    HandleShutdownCapabilityRequest(message.Header.Sender, message.Header.Id);
                });

            // Define the response to a shutdown capability request
            processor.RegisterAction(
                typeof(ApplicationStartupCompleteMessage),
                message =>
                {
                    HandleStartupCompleteMessage();
                });

            // @TODO: Doesn't need to answer to a shutdown message?
        }

        private void HandleShutdownCapabilityRequest(DnsName originalSender, MessageId id)
        {
            Debug.Assert(IsFullyFunctional, "For some reason we managed to register the message actions before being fully functional.");

            // @todo: Check with the UI if we can shutdown. This should only be a UI value, not the system value.
            // For now just send a message saying that we can shutdown.
            SendMessage(originalSender, new ServiceShutdownCapabilityResponseMessage(true), id);
        }

        private void HandleStartupCompleteMessage()
        {
            if (!m_Notifications.ContainsKey(m_NotificationNames.StartupComplete))
            {
                return;
            }

            var action = m_Notifications[m_NotificationNames.StartupComplete];
            try
            {
                action(null);
            }
            catch (Exception e)
            {
                // Log the fact that we failed
                SendMessage(
                    m_DnsNames.AddressOfLogger,
                    new LogEntryRequestMessage(
                        new LogMessage(
                            Name.ToString(),
                            LevelToLog.Error,
                            string.Format(CultureInfo.InvariantCulture, Resources_NonTranslatable.UserInterrface_LogMessage_StartupCompleteNotificationFailed, e)),
                        LogType.Debug),
                    MessageId.None);

                // Now get the hell out of here.
                throw;
            }
        }
    }
}
